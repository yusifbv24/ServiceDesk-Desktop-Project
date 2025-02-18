using ServiceDesk.Class;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ServiceDesk.Class.TableDependencies;
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base.Enums;
using TableDependency.SqlClient.Base.EventArgs;
using System.Data;

namespace ServiceDesk.Forms
{
    public partial class Dashboard : Form
    {
        private readonly Connect connect = Connect.Instance;
        private readonly static string _firstDayOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).ToString("yyyy-MM-dd");
        private readonly string _today = DateTime.Today.ToString("yyyy-MM-dd");
        private string _monday = string.Empty;
        private readonly string _fullname = default;
        private Main _mainMenu;
        private SqlTableDependency<TicketTable> _tableDependency_Ticket;
        private SqlTableDependency<StatusTable> _tableDependency_Status;
        private SqlTableDependency<RatingTable> _tableDependency_Rating;
        private SqlConnection connection { get; set; } = null;
        public Dashboard(string _fullname,Main mainMenu)
        {
            InitializeComponent();
            this._fullname = _fullname;
            _mainMenu = mainMenu;
            _=CalculateDateOfWeek();
            _= DoItAll(_mainMenu.dateFiltering.Text);
            StartTableDependency(); // Start listening for table changes
        }
        private async Task CreateConnectionWithDatabase()
        {
            if (connection == null)
            {
                connection = await connect.EstablishConnectionWithServiceDeskAsync(_mainMenu._sessionId).ConfigureAwait(false);
            }
            await connection.OpenAsync();
        }
        private async Task DoItAll(string date)
        {
            await CreateConnectionWithDatabase();
            await CalculateCSATValue();
            await CountOfTicketsForUsers(date);
            await CheckTicketStatus(date);
            await LastTickets(date);
            await FindTopTasks(date);
        }
        private async Task CalculateDateOfWeek()
        {
            try
            {
                DateTime today = DateTime.Today;
                if(today.DayOfWeek== DayOfWeek.Monday)
                    _monday = today.ToString("yyyy-MM-dd");
                else
                    _monday = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday).ToString("yyyy-MM-dd");
            }
            catch (Exception ex)
            {
                Notifications.Error($"{ex.Message}", "Error occured while calculating dateofweek");
                await Logger.Log(_fullname, $" | Error occured in Dashboard Panel while running CalculateDateOfWeek. | Error is: {ex.Message}");
            }
        }
        #region CSAT
        private async Task CalculateCSATValue()
        {
            try
            {
                await Ratings.CalculateCSAT(_fullname,_mainMenu._sessionId);
                await GetCSATValue();
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message,"Error while calculating csat");
                await Logger.Log(_fullname, $" | Error occured in Dashboard Panel while running GetCSATValue. | Error is: {ex.Message}");
            }
        }
        private async Task GetCSATValue()
        {
            try
            {
                if (connection is null || connection.State == ConnectionState.Closed)
                {
                    await CreateConnectionWithDatabase();
                }
                using var cm = new SqlCommand("SELECT csat FROM Users WHERE fullname LIKE @fullname", connection);
                cm.Parameters.AddWithValue("@fullname", _fullname);
                using var dr = await cm.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                while (await dr.ReadAsync())
                {
                    CSATcalculator.Value = Convert.ToInt32(dr["csat"].ToString());
                }
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message);
                await Logger.Log(_fullname, $" | Error occured in Dashboard Panel while running GetCSATValue. | Error is: {ex.Message}");
            }
        }
        #endregion
        #region TopTicketSolvers
        private async Task CountOfTicketsForUsers(string _date)
        {
            string _whichTime = _monday;
            if (_date == "daily")
            {
                _whichTime = _today;
                progress_All.Maximum_Value = 10;
            }
            if (_date == "monthly")
            {
                _whichTime = _firstDayOfMonth;
                progress_All.Maximum_Value = 100;
            }
            try
            {
                if (dgvTicketSolvers.Columns.Count == 0)
                {
                    dgvTicketSolvers.Columns.Add("Solvers", "");
                    dgvTicketSolvers.Columns.Add("ClosedTickets", "");
                }
                dgvTicketSolvers.Rows.Clear();
                string query= @"WITH SplitFullnames AS (
                                SELECT 
                                LTRIM(RTRIM(value)) AS Fullname, -- Clean up whitespace
                                Ticket.ID AS TicketID
                                FROM Ticket
                                CROSS APPLY STRING_SPLIT(Ticket.fullname, ',') -- Split the fullname by commas
                                )
                                SELECT
                                    Users.fullname,
                                    COUNT(Status.status) AS CountOfTickets
                                FROM Status
                                INNER JOIN Ticket WITH (NOLOCK) ON Status.ID = Ticket.ID
                                INNER JOIN SplitFullnames ON Ticket.ID = SplitFullnames.TicketID
                                INNER JOIN Users WITH (NOLOCK) ON SplitFullnames.Fullname = Users.fullname 
                                WHERE 
                                    Status.time BETWEEN @fromDate AND @toDate 
                                    AND Status.status = 'closed' 
                                    GROUP BY Users.fullname 
                                    ORDER BY CountOfTickets DESC";
                if (connection is null || connection.State == ConnectionState.Closed)
                {
                    await CreateConnectionWithDatabase();
                }
                using var cm = new SqlCommand(query, connection);
                cm.Parameters.AddWithValue("@fromDate", _whichTime);
                cm.Parameters.AddWithValue("@toDate", _today);
                using var dr = await cm.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                while (await dr.ReadAsync())
                {
                    dgvTicketSolvers.Rows.Add(dr[0].ToString(), dr[1].ToString());
                }
            }
            catch (InvalidOperationException ex)
            {
                await Logger.Log(_fullname, $" | InvalidOperationException in CountOfTicketsForUsers in Dashboard Panel| Error is: {ex.Message}");
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message, "Error occured while running CountOfTickets For User");
                await Logger.Log(_fullname, $" | Error occured in Dashboard Panel while running CountOfTicketsForUsers. | Error is: {ex.Message}");
            }
        }
        #endregion
        #region TicketStatus
        private async Task CheckTicketStatus(string _date)
        {
            await AllTickets(_date);
            await PendingStatusTickets(_date);
            await ResolvedStatusTickets(_date);
            await ClosedStatusTickets(_date);
            await OverAllRating();
        }
        private async Task AllTickets(string _date)
        {
            string _whichTime = _monday;
            if (_date == "daily")
            {
                _whichTime = _today;
                progress_All.Maximum_Value = 10;
            }
            if (_date == "monthly")
            {
                _whichTime = _firstDayOfMonth;
                progress_All.Maximum_Value = 100;
            }
            try
            {
                string query = @"SELECT COUNT(Status.status) AS alltickets
                            FROM Status
                            INNER JOIN Ticket WITH (NOLOCK) ON Status.ID=Ticket.ID
                            WHERE Status.time BETWEEN @fromDate AND @toDate AND fullname LIKE @fullname";
                if (connection is null || connection.State == ConnectionState.Closed)
                {
                    await CreateConnectionWithDatabase();
                }
                using var cm = new SqlCommand(query, connection);
                cm.Parameters.AddWithValue("@fromDate", _whichTime);
                cm.Parameters.AddWithValue("@toDate", _today);
                cm.Parameters.AddWithValue("@fullname", $"%{_fullname}%");
                using var dr = await cm.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                while (await dr.ReadAsync())
                {
                    labelAllTickets.Text = dr["alltickets"].ToString();
                }
                progress_All.Value = int.Parse(labelAllTickets.Text);
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message, "Error occured while searching for all tickets");
                await Logger.Log(_fullname, $" | Error occured in Dashboard Panel while AllTickets. | Error is: {ex.Message}");
            }
        }
        private async Task PendingStatusTickets(string _date)
        {
            string _whichTime = _monday;
            if (_date == "daily")
            {
                _whichTime = _today;
                progress_pending.Maximum_Value = 10;
            }
            if (_date == "monthly")
            {
                _whichTime = _firstDayOfMonth;
                progress_pending.Maximum_Value = 100;
            }
            try
            {
                string query = @"SELECT COUNT(Status.status) AS pending
                            FROM Status
                            INNER JOIN Ticket WITH (NOLOCK) ON Status.ID=Ticket.ID
                            WHERE Status.status='pending' AND Status.time BETWEEN @fromDate AND @toDate AND fullname LIKE @fullname ";
                if (connection is null || connection.State == ConnectionState.Closed)
                {
                    await CreateConnectionWithDatabase();
                }
                using var cm = new SqlCommand(query, connection);
                cm.Parameters.AddWithValue("@fromDate", _whichTime);
                cm.Parameters.AddWithValue("@toDate", _today);
                cm.Parameters.AddWithValue("@fullname", $"%{_fullname}%");
                using var dr = await cm.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                while (await dr.ReadAsync())
                {
                    labelPendingTickets.Text = dr["pending"].ToString();
                }
                progress_pending.Value = int.Parse(labelPendingTickets.Text);
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message, "Error occured while searching for pending tickets");
                await Logger.Log(_fullname, $" | Error occured in Dashboard Panel while PendingStatusTickets. | Error is: {ex.Message}");
            }
        }
        private async Task ResolvedStatusTickets(string _date)
        {
            string _whichTime = _monday;
            if (_date == "daily")
            {
                _whichTime = _today;
                progress_resolved.Maximum_Value = 10;
            }
            if (_date == "monthly")
            {
                _whichTime = _firstDayOfMonth;
                progress_resolved.Maximum_Value = 100;
            }
            try
            {
                string query = @"SELECT COUNT(Status.status) AS resolved
                            FROM Status
                            INNER JOIN Ticket WITH (NOLOCK) ON Status.ID=Ticket.ID
                            WHERE (Status.status='resolved' OR Status.status='resolving') AND Status.time BETWEEN @fromDate AND @toDate AND fullname LIKE @fullname ";
                if (connection is null || connection.State == ConnectionState.Closed)
                {
                    await CreateConnectionWithDatabase();
                }
                using var cm = new SqlCommand(query, connection);
                cm.Parameters.AddWithValue("@fromDate", _whichTime);
                cm.Parameters.AddWithValue("@toDate", _today);
                cm.Parameters.AddWithValue("@fullname", $"%{_fullname}%");
                using var dr = await cm.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                while (await dr.ReadAsync())
                {
                    labelResolvedTickets.Text = dr["resolved"].ToString();
                }
                progress_resolved.Value = int.Parse(labelResolvedTickets.Text);
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message, "Error occured while searching for resolved tickets");
                await Logger.Log(_fullname, $" | Error occured in Dashboard Panel while ResolvedStatusTickets. | Error is: {ex.Message}");
            }
        }
        private async Task ClosedStatusTickets(string _date)
        {
            string _whichTime = _monday;
            if (_date == "daily")
            {
                _whichTime = _today;
                progress_closed.Maximum_Value = 10;
            }
            if (_date == "monthly")
            {
                _whichTime = _firstDayOfMonth;
                progress_closed.Maximum_Value = 100;
            }
            try
            {
                string query = @"SELECT COUNT(Status.status) AS closed
                            FROM Status
                            INNER JOIN Ticket WITH (NOLOCK) ON Status.ID=Ticket.ID
                            WHERE Status.status='closed' AND Status.time BETWEEN @fromDate AND @toDate AND fullname LIKE @fullname ";
                if (connection is null || connection.State == ConnectionState.Closed)
                {
                    await CreateConnectionWithDatabase();
                }
                using var cm = new SqlCommand(query, connection);
                cm.Parameters.AddWithValue("@fromDate", _whichTime);
                cm.Parameters.AddWithValue("@toDate", _today);
                cm.Parameters.AddWithValue("@fullname", $"%{_fullname}%");
                using var dr = await cm.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                while (await dr.ReadAsync())
                {
                    labelClosedTickets.Text = dr["closed"].ToString();
                }
                progress_closed.Value = int.Parse(labelClosedTickets.Text);
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message, "Error occured while searching for closed tickets");
                await Logger.Log(_fullname, $" | Error occured in Dashboard Panel while ClosedStatusTickets. | Error is: {ex.Message}");
            }
        }
        private async Task OverAllRating()
        {
            try
            {
                double overall = 0;
                // Attempt to parse values from text boxes, defaulting to 0 if the parse fails
                if (!int.TryParse(labelResolvedTickets.Text, out int resolvedTickets))
                {
                    return;
                }
                if (!int.TryParse(labelClosedTickets.Text, out int closedTickets))
                {
                    return;
                }
                // Calculate overall percentage if both values are non-zero
                if (resolvedTickets + closedTickets > 0)
                {
                    overall = (double)closedTickets / (resolvedTickets + closedTickets) * 100;
                    labelOverAll.Text = $"{overall:F2}%";
                }
                else
                {
                    labelOverAll.Text = "0%";
                }
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message, "Error occured while searching for overall rating");
                await Logger.Log(_fullname, $" | Error occured in Dashboard Panel while running OverAllRating. | Error is: {ex.Message}");
            }
        }
        #endregion
        #region Detailed Information about Tickets
        private string GetAdjustedDate(string _data)
        {
            if (!string.IsNullOrEmpty(_data))
            {
                DateTime date = DateTime.Parse(_data);
                return $"{date:dd.MM.yyyy} / {date:HH:mm}";
            }
            else
                return "";
        }
        private string GetAdjustedStatus(string status, string time)
        {
            if (!string.IsNullOrEmpty(time))
            {
                return $"{status} / {time}";
            }
            else
            {
                return $"{status}";
            }
        }
        private async Task LastTickets(string date)
        {
            string _firstDate = _monday;
            if (date == "daily")
            {
                _firstDate = _today;
            }
            if (date == "monthly")
            {
                _firstDate = _firstDayOfMonth;
            }
            try
            {
                if (dgvLastTickets.Columns.Count == 0)
                {
                    dgvLastTickets.Columns.Add("identify", "ID");
                    dgvLastTickets.Columns.Add("Date", "Date");
                    dgvLastTickets.Columns.Add("Status", "Status");
                    dgvLastTickets.Columns.Add("User", "User");
                    dgvLastTickets.Columns.Add("Rating", "Rating");
                    dgvLastTickets.Columns.Add("Message", "Message");
                }
                dgvLastTickets.Rows.Clear();
                string query = @"SELECT Ticket.ID,Ticket.creation_date,Status.status, Ticket.taken_time,Rating.rating,Rating.message
                            FROM Status
                            INNER JOIN Rating WITH (NOLOCK) ON Rating.ID = Status.ID
                            INNER JOIN Ticket WITH (NOLOCK) ON Status.ID=Ticket.ID
                            WHERE Status.time BETWEEN @fromDate AND @toDate AND fullname LIKE @fullname ORDER BY ID DESC";
                if (connection is null || connection.State == ConnectionState.Closed)
                {
                    await CreateConnectionWithDatabase();
                }
                using var cm = new SqlCommand(query, connection);
                cm.Parameters.AddWithValue("@fromDate", _firstDate);
                cm.Parameters.AddWithValue("@toDate", _today);
                cm.Parameters.AddWithValue("@fullname", $"%{_fullname}%");
                using var dr = await cm.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                while (await dr.ReadAsync())
                {
                    dgvLastTickets.Rows.Add(dr["ID"].ToString(), GetAdjustedDate(dr["creation_date"].ToString()), GetAdjustedStatus(dr["status"].ToString(), dr["taken_time"].ToString()), dr["rating"].ToString(), dr["message"].ToString());
                }
            }
            catch (InvalidOperationException ex)
            {
                await Logger.Log(_fullname, $" | InvalidOperationException in LastTickets in Dashboard Panel| Error is: {ex.Message}");
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message, "Error occured while searching for last tickets");
                await Logger.Log(_fullname, $" | Error occured in Dashboard Panel while running LastTickets. | Error is: {ex.Message}");
            }
        }
        #endregion
        #region TopTasks
        private async Task FindTopTasks(string _time)
        {
            string _whichTime = _monday;
            if (_time == "daily")
            {
                _whichTime = _today;
            }
            if (_time == "monthly")
            {
                _whichTime = _firstDayOfMonth;
            }
            try
            {
                if (dgvTasks.Columns.Count == 0)
                {
                    dgvTasks.Columns.Add("identificator", "№");
                    dgvTasks.Columns.Add("tasks", "Tasks");
                    dgvTasks.Columns.Add("CountOf", "Count Of");
                }
                dgvTasks.Rows.Clear();
                int i = 1;
                string query = @"WITH SplitTicketTasks AS (
                SELECT 
                t.ID,
                t.fullname,
                LTRIM(RTRIM(value)) AS TaskText  -- Trim spaces around each split value
                FROM Ticket t
                CROSS APPLY STRING_SPLIT(t.task, ',')
                )
                SELECT TOP(10)
                p.task AS Tasks,
                COUNT(tp.TaskText) AS CountOfTickets
                FROM Tasks p
                JOIN SplitTicketTasks tp
                ON p.task = tp.TaskText  -- Match based on the task text
				JOIN Status s
				ON s.ID = tp.ID
				WHERE 
				s.time BETWEEN @fromDate AND @toDate 
				AND tp.fullname LIKE @fullname 
				GROUP BY tp.fullname, p.ID, p.task 
				ORDER BY tp.fullname, CountOfTickets DESC";
                if (connection is null || connection.State == ConnectionState.Closed)
                {
                    await CreateConnectionWithDatabase();
                }
                using var cm = new SqlCommand(query, connection);
                cm.Parameters.AddWithValue("@fromDate", _whichTime);
                cm.Parameters.AddWithValue("@toDate", _today);
                cm.Parameters.AddWithValue("@fullname", _fullname);
                using var dr = await cm.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                while (await dr.ReadAsync())
                {
                    dgvTasks.Rows.Add(i++, dr["Tasks"].ToString(), dr["CountOfTickets"].ToString());
                }
            }
            catch (InvalidOperationException ex)
            {
                await Logger.Log(_fullname, $" | InvalidOperationException in FindTopTasks in Dashboard Panel| Error is: {ex.Message}");
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message, "Error occured while searching for top tasks");
                await Logger.Log(_fullname, $" | Error occured in Dashboard Panel while running FindTopTasks. | Error is: {ex.Message}");
            }
        }
        #endregion
        #region SqlTableDependency
        private async void Dashboard_FormClosing(object sender, FormClosingEventArgs e)
        {
            await StopTableDependency();
        }
        private void StartTableDependency()
        {
            Task.Run(async () =>
            {
                await Task.Delay(1000);
                await TicketTableDependency();
                await Task.Delay(1000);
                await StatusTableDependency();
                await Task.Delay(1000);
                await RatingTableDependency();
            });
        }
        private async Task TicketTableDependency()
        {
            if (_tableDependency_Ticket == null)
            {
                using (_tableDependency_Ticket = new SqlTableDependency<TicketTable>(connect.ServicedeskConnection, "Ticket"))
                {
                    _tableDependency_Ticket.OnChanged += TableDependency_Ticket_OnChanged;
                    _tableDependency_Ticket.OnError += TableDependency_OnError;
                    _tableDependency_Ticket.Start();
                }
            }
            else
            {
                _tableDependency_Ticket.OnChanged += TableDependency_Ticket_OnChanged;
                _tableDependency_Ticket.OnError += TableDependency_OnError;
                _tableDependency_Ticket.Start();
            }
            await Task.Delay(1);
        }
        private async Task StatusTableDependency()
        {
            if (_tableDependency_Status == null)
            {
                using (_tableDependency_Status = new SqlTableDependency<StatusTable>(connect.ServicedeskConnection, "Status"))
                {
                    _tableDependency_Status.OnChanged += TableDependency_Status_OnChanged;
                    _tableDependency_Status.OnError += TableDependency_OnError;
                    _tableDependency_Status.Start();
                }
            }
            else
            {
                _tableDependency_Status.OnChanged += TableDependency_Status_OnChanged;
                _tableDependency_Status.OnError += TableDependency_OnError;
                _tableDependency_Status.Start();
            }
            await Task.Delay(1);
        }
        private async Task RatingTableDependency()
        {
            if (_tableDependency_Rating == null)
            {
                using (_tableDependency_Rating = new SqlTableDependency<RatingTable>(connect.ServicedeskConnection, "Rating"))
                {
                    _tableDependency_Rating.OnChanged += TableDependency_Rating_OnChanged;
                    _tableDependency_Rating.OnError += TableDependency_OnError;
                    _tableDependency_Rating.Start();
                }
            }
            else
            {
                _tableDependency_Rating.OnChanged += TableDependency_Rating_OnChanged;
                _tableDependency_Rating.OnError += TableDependency_OnError;
                _tableDependency_Rating.Start();
            }
            await Task.Delay(1);
        }
        private async void TableDependency_OnError(object sender, ErrorEventArgs e)
        {
            await Logger.Log(_fullname, "Error occured while running table dependency in Dashboard");
        }
        private async void TableDependency_Ticket_OnChanged(object sender, RecordChangedEventArgs<TicketTable> e)
        {
            if (this.IsDisposed && !this.IsHandleCreated)
                return;
            if (e.ChangeType != ChangeType.None)
            {
                this.BeginInvoke((MethodInvoker)(async () => await DoItAll(_mainMenu.dateFiltering.Text)));
            }
            await Task.Delay(1);
        }
        private async void TableDependency_Status_OnChanged(object sender, RecordChangedEventArgs<StatusTable> e)
        {
            if (this.IsDisposed && !this.IsHandleCreated) return;
            if (e.ChangeType != ChangeType.None)
            {
                this.BeginInvoke((MethodInvoker)(async () => await DoItAll(_mainMenu.dateFiltering.Text)));
            }
            await Task.Delay(1);
        }
        private async void TableDependency_Rating_OnChanged(object sender, RecordChangedEventArgs<RatingTable> e)
        {
            if (this.IsDisposed && !this.IsHandleCreated) return;
            if (e.ChangeType != ChangeType.None)
            {
                this.BeginInvoke((MethodInvoker)(async () => await LastTickets(_mainMenu.dateFiltering.Text)));
            }
            await Task.Delay(1);
        }
        private async Task StopTableDependency()
        {
            await SafeStop(_tableDependency_Ticket, "Ticket");
            await SafeStop(_tableDependency_Status, "Status");
            await SafeStop(_tableDependency_Rating, "Rating");
        }
        private async Task SafeStop<T>(SqlTableDependency<T> dependency, string name) where T : class, new()
        {
            if (dependency != null)
            {
                try
                {
                    dependency.Stop();
                    dependency.Dispose();
                }
                catch (ObjectDisposedException)
                {
                    await Logger.Log(_fullname, $"{name} dependency is already disposed in Dashboard.");
                }
                catch (InvalidOperationException ex)
                {
                    await Logger.Log(_fullname, $"Invalid operation while stopping {name} in Dashboard: {ex.Message}");
                }
                catch (AggregateException ex)
                {
                    await Logger.Log(_fullname, $"AggregateException occurred in {name} in Dashboard: {ex.InnerException?.Message}");
                }
                catch (Exception ex)
                {
                    await Logger.Log(_fullname, $"Error stopping {name} in Dashboard: {ex.Message}");
                }
            }
        }
        #endregion
    }
}