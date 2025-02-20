using ServiceDesk.Class;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ServiceDesk.Class.TableDependencies;
using TableDependency.SqlClient.Base.Enums;
using TableDependency.SqlClient.Base.EventArgs;
using TableDependency.SqlClient;
using System.Data;
using System.Collections.Generic;
using System.Threading;
using System.Configuration;

namespace ServiceDesk.Forms
{
    public partial class DashboardForAdminPanel : Form
    {
        private readonly static string _firstDayOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).ToString("yyyy-MM-dd");
        private readonly string _today = DateTime.Today.ToString("yyyy-MM-dd");
        private string _monday = string.Empty;
        private readonly string _fullname = default;
        private Main _mainMenu;
        private SqlTableDependency<TicketTable> _tableDependency_Ticket;
        private SqlTableDependency<StatusTable> _tableDependency_Status;
        private SqlTableDependency<UserTable> _tableDependency_User;
        private SqlTableDependency<RatingTable> _tableDependency_Rating;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private SqlConnection _connection { get; set; } = null;
        private static string _connection_string { get; set; } = null;
        public DashboardForAdminPanel(string fullname, Main mainMenu)
        {
            InitializeComponent();
            _fullname = fullname;
            _mainMenu = mainMenu;
            _=CalculateDateOfWeek();
            _ = DoItAll(_mainMenu.dateFiltering.Text);
            _ = ConnectDependenciesToDatabase();
            StartTableDependency(); // Start listening for table changes
        }
        private async Task ConnectToTheDatabase()
        {
            if (_connection == null)
            {
                _connection = await ConnectionDatabase.ConnectToTheServer(_mainMenu._sessionId);
                await _connection.OpenAsync();
            }
            if (_connection.State == ConnectionState.Closed)
            {
                await _connection.OpenAsync();
            }
        }
        private async Task DoItAll(string date)
        {
            await ConnectToTheDatabase();
            await SearchCSATForUsers();
            await CheckTicketStatus(date);
            await CountOfTicketsForUsers(date);
            await FindTopTasks(date);
            await LastTickets(date);
        }
        private async Task CalculateDateOfWeek()
        {
            try
            {
                DateTime today = DateTime.Today;
                if (today.DayOfWeek == DayOfWeek.Monday)
                    _monday = today.ToString("yyyy-MM-dd");
                else
                    _monday = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday).ToString("yyyy-MM-dd");
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message);
                await Logger.Log(_fullname, $" | Error occured in DashboardForAdmin Panel while running CalculateDateOfWeek. | Error is: {ex.Message}");
            }
            finally
            {
                await Task.Delay(1);
            }
        }
        #region CSAT
        private async Task SearchCSATForUsers()
        {
            if (dgvCSAT.Columns.Count == 0)
            {
                dgvCSAT.Columns.Add("ID", "");
                dgvCSAT.Columns.Add("Users", "");
                dgvCSAT.Columns.Add("Csat", "");
            }
            dgvCSAT.Rows.Clear();
            int i = 1;
            try
            {
                if (_connection == null||_connection.State==ConnectionState.Closed)
                {
                    await ConnectToTheDatabase();
                }
                using var cm = new SqlCommand("SELECT fullname,csat FROM Users WITH (NOLOCK) WHERE type='User' ORDER BY CAST(csat as float) desc", _connection);
                using var dr = await cm.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                while (await dr.ReadAsync())
                {
                    dgvCSAT.Rows.Add(i++, $"{dr["fullname"]}", $"{dr["csat"]}%");
                }
            }
            catch (InvalidOperationException ex)
            {
                await Logger.Log(_fullname, $" | InvalidOperationException in SearchCSATForUser in DasboardForAdminPanel| Error is: {ex.Message}");
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message, "Error occured while calculating rating");
                await Logger.Log(_fullname, $" | Error occured in DashboardForAdmin Panel while running SearchCSATForUsers. | Error is: {ex.Message}");
            }
        }
        #endregion
        #region TicketStatus
        private async Task CheckTicketStatus(string date)
        {
            await AllTickets(date);
            await PendingStatusTickets(date);
            await ResolvedStatusTickets(date);
            await ClosedStatusTickets(date);
            await OverAllRating();
        }
        private async Task AllTickets(string date)
        {
            string _whichTime = _monday;
            if (date == "daily")
            {
                _whichTime = _today;
                progress_All.Maximum_Value = 30;
            }
            if (date == "monthly")
            {
                _whichTime = _firstDayOfMonth;
                progress_All.Maximum_Value = 300;
            }
            string query = @"SELECT COUNT(Status.status) AS alltickets
                            FROM Status
                            INNER JOIN Ticket WITH (NOLOCK) ON Status.ID=Ticket.ID
                            WHERE Status.time BETWEEN @fromDate AND @toDate";
            try
            {
                if (_connection == null || _connection.State == ConnectionState.Closed)
                {
                    await ConnectToTheDatabase();
                }
                using var cm = new SqlCommand(query, _connection);
                cm.Parameters.AddWithValue("@fromDate", _whichTime);
                cm.Parameters.AddWithValue("@toDate", _today);
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
                await Logger.Log(_fullname, $" | Error occured in DashboardForAdmin Panel while AllTickets. | Error is: { ex.Message}");
            }
        }
        private async Task PendingStatusTickets(string _time)
        {
            string query = @"SELECT COUNT(Status.status) AS pending
                            FROM Status
                            INNER JOIN Ticket WITH (NOLOCK) ON Status.ID=Ticket.ID
                            WHERE Status.status='pending' AND Status.time BETWEEN @fromDate AND @toDate ";
            string _whichTime = _monday;
            if (_time == "daily")
            {
                _whichTime = _today;
                progress_pending.Maximum_Value = 10;
            }
            if (_time == "monthly")
            {
                _whichTime = _firstDayOfMonth;
                progress_pending.Maximum_Value = 30;
            }
            try
            {
                if (_connection == null || _connection.State == ConnectionState.Closed)
                {
                    await ConnectToTheDatabase();
                }
                using var cm = new SqlCommand(query, _connection);
                cm.Parameters.AddWithValue("@fromDate", _whichTime);
                cm.Parameters.AddWithValue("@toDate", _today);
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
                await Logger.Log(_fullname, $" | Error occured in DashboardForAdmin Panel while PendingStatusTickets. | Error is: {ex.Message}");
            }
        }
        private async Task ResolvedStatusTickets(string _time)
        {
            string _whichTime = _monday;
            if (_time == "daily")
            {
                _whichTime = _today;
                progress_resolved.Maximum_Value = 10;
            }
            if (_time == "monthly")
            {
                _whichTime = _firstDayOfMonth;
                progress_resolved.Maximum_Value = 50;
            }
            string query = @"SELECT COUNT(Status.status) AS resolved
                            FROM Status
                            INNER JOIN Ticket WITH (NOLOCK) ON Status.ID=Ticket.ID
                            WHERE (Status.status='resolved' OR Status.status='resolving') AND Status.time BETWEEN @fromDate AND @toDate";
            try
            {
                if (_connection == null || _connection.State == ConnectionState.Closed)
                {
                    await ConnectToTheDatabase();
                }
                using var cm = new SqlCommand(query, _connection);
                cm.Parameters.AddWithValue("@fromDate", _whichTime);
                cm.Parameters.AddWithValue("@toDate", _today);
                using var dr = await cm.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                while (await dr.ReadAsync())
                {
                    labelResolvedTickets.Text = dr["resolved"].ToString();
                }
                progress_resolved.Value = int.Parse(labelResolvedTickets.Text);
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message, "Error occured while searching for resolving tickets");
                await Logger.Log(_fullname, $" | Error occured in DashboardForAdmin Panel while ResolvedStatusTickets. | Error is: {ex.Message}");
            }
        }
        private async Task ClosedStatusTickets(string _time)
        {
            string _whichTime = _monday;
            if (_time == "daily")
            {
                _whichTime = _today;
                progress_closed.Maximum_Value = 30;
            }
            if (_time == "monthly")
            {
                _whichTime = _firstDayOfMonth;
                progress_closed.Maximum_Value = 300;
            }
            string query = @"SELECT COUNT(Status.status) AS closed
                            FROM Status
                            INNER JOIN Ticket WITH (NOLOCK) ON Status.ID=Ticket.ID
                            WHERE Status.status='closed' AND Status.time BETWEEN @fromDate AND @toDate ";
            try
            {
                if (_connection == null || _connection.State == ConnectionState.Closed)
                {
                    await ConnectToTheDatabase();
                }
                using var cm = new SqlCommand(query, _connection);
                cm.Parameters.AddWithValue("@fromDate", _whichTime);
                cm.Parameters.AddWithValue("@toDate", _today);
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
                await Logger.Log(_fullname, $" | Error occured in DashboardForAdmin Panel while ClosedStatusTickets. | Error is: {ex.Message}");
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
                    lblOverAll.Text = $"{overall:F2}%";
                }
                else
                {
                    lblOverAll.Text = "0%";
                }
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message);
                await Logger.Log(_fullname, $" | Error occured in DashboardForAdmin Panel while running OverAllRating. | Error is: {ex.Message}");
            }
            finally
            {
                await Task.Delay(1);
            }
        }
        #endregion
        #region TopTicketSolvers
        private async Task CountOfTicketsForUsers(string _time)
        {
            string _whichTime = _monday;
            if (_time == "daily")
            {
                _whichTime = _today;
                progress_All.Maximum_Value = 10;
            }
            if (_time == "monthly")
            {
                _whichTime = _firstDayOfMonth;
                progress_All.Maximum_Value = 100;
            }
            if (dgvTicketSolvers.Columns.Count == 0)
            {
                dgvTicketSolvers.Columns.Add("Solvers", "");
                dgvTicketSolvers.Columns.Add("ClosedTickets", "");
            }
            dgvTicketSolvers.Rows.Clear();
            string query = @"WITH SplitFullnames AS (
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
            try
            {
                if (_connection == null || _connection.State == ConnectionState.Closed)
                {
                    await ConnectToTheDatabase();
                }
                using var cm = new SqlCommand(query, _connection);
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
                await Logger.Log(_fullname, $" | InvalidOperationException in CountOfTicketsForUsers in DasboardForAdminPanel| Error is: {ex.Message}");
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message, "Error occured while calculating for count of tickets");
                await Logger.Log(_fullname, $" | Error occured in DashboardForAdmin Panel while running CountOfTicketsForUsers. | Error is: {ex.Message}");
            }
        }
        #endregion
        #region Top Tasks
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
                LTRIM(RTRIM(value)) AS TaskText 
                FROM Ticket t
                CROSS APPLY STRING_SPLIT(t.task, ',')
                )
                SELECT TOP(10)
                p.task AS Tasks,
                COUNT(tp.TaskText) AS CountOfTickets
                FROM Tasks p
                JOIN SplitTicketTasks tp
                ON p.task = tp.TaskText
                JOIN Status s
                ON s.ID=tp.ID
                WHERE 
                s.time BETWEEN @fromDate AND @toDate 
                GROUP BY p.ID, p.task ORDER BY CountOfTickets DESC";
            try
            {
                if (_connection == null || _connection.State == ConnectionState.Closed)
                {
                    await ConnectToTheDatabase();
                }
                using var cm = new SqlCommand(query, _connection);
                cm.Parameters.AddWithValue("@fromDate", _whichTime);
                cm.Parameters.AddWithValue("@toDate", _today);
                using var dr = await cm.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                while (await dr.ReadAsync())
                {
                    dgvTasks.Rows.Add(i++, dr["Tasks"].ToString(), dr["CountOfTickets"].ToString());
                }
            }
            catch (InvalidOperationException ex)
            {
                await Logger.Log(_fullname, $" | InvalidOperationException in FindTopTasks in DasboardForAdminPanel| Error is: {ex.Message}");
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message, "Error occured while searching for top tasks");
                await Logger.Log(_fullname, $" | Error occured in DashboardForAdmin Panel while running FindTopTasks. | Error is: {ex.Message}");
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
        private string GetAdjustedStatus(string status,string time)
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
            if (dgvLastTickets.Columns.Count == 0)
            {
                dgvLastTickets.Columns.Add("identify", "ID");
                dgvLastTickets.Columns.Add("Date", "Date");
                dgvLastTickets.Columns.Add("Status", "Status");
                dgvLastTickets.Columns.Add("User", "User");
                dgvLastTickets.Columns.Add("Rating", "Rating");
                dgvLastTickets.Columns.Add("Message", "Message");
            }
            string query = @"SELECT Ticket.ID,Ticket.creation_date,Status.status, Ticket.taken_time, Ticket.fullname,Rating.rating,Rating.message
                            FROM Status
                            INNER JOIN Rating WITH (NOLOCK) ON Rating.ID = Status.ID
                            INNER JOIN Ticket WITH (NOLOCK) ON Status.ID=Ticket.ID
                            WHERE Status.time BETWEEN @fromDate AND @toDate ORDER BY ID Desc ";
            try
            {
                if (_connection == null || _connection.State == ConnectionState.Closed)
                {
                    await ConnectToTheDatabase();
                }
                dgvLastTickets.Rows.Clear();    
                using var cm = new SqlCommand(query, _connection);
                cm.Parameters.AddWithValue("@fromDate", _firstDate);
                cm.Parameters.AddWithValue("@toDate", _today);
                using var dr = await cm.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                while (await dr.ReadAsync())
                {
                    dgvLastTickets.Rows.Add(
                        dr["ID"].ToString(),
                        GetAdjustedDate(dr["creation_date"].ToString()),
                        GetAdjustedStatus(dr["status"].ToString(),
                        dr["taken_time"].ToString()),
                        dr["fullname"].ToString(),
                        dr["rating"].ToString(),
                        dr["message"].ToString());
                }
            }
            catch (InvalidOperationException ex)
            {
                await Logger.Log(_fullname, $" | InvalidOperationException in LastTickets in DasboardForAdminPanel| Error is: {ex.Message}");
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message, "Error occured while running for last tickets");
                await Logger.Log(_fullname, $" | Error occured in DashboardForAdmin Panel while running LastTickets. | Error is: {ex.Message}");
            }
        }
        #endregion
        #region SqlTableDependency
        private async void DashboardForAdminPanel_FormClosing(object sender, FormClosingEventArgs e)
        {
            await StopTableDependencyAsync();
        }
        private async void TableDependency_OnError(object sender, ErrorEventArgs e)
        {
            await Logger.Log(_fullname, "Error occurred while running table dependency in DashboardForAdminPanel");
        }
        private void TableDependency_Ticket_OnChanged(object sender, RecordChangedEventArgs<TicketTable> e)
        {
            if (this.IsDisposed || !this.IsHandleCreated) return;
            if (e.ChangeType != ChangeType.None)
            {
                this.BeginInvoke((MethodInvoker)(async () => await DoItAll(_mainMenu.dateFiltering.Text)));
            }
        }
        private void TableDependency_Status_OnChanged(object sender, RecordChangedEventArgs<StatusTable> e)
        {
            if (this.IsDisposed || !this.IsHandleCreated) return;
            if (e.ChangeType != ChangeType.None)
            {
                this.BeginInvoke((MethodInvoker)(async () => await DoItAll(_mainMenu.dateFiltering.Text)));
            }
        }
        private void TableDependency_User_OnChanged(object sender, RecordChangedEventArgs<UserTable> e)
        {
            if (this.IsDisposed || !this.IsHandleCreated) return;
            if (e.ChangeType != ChangeType.None)
            {
                this.BeginInvoke((MethodInvoker)(async () => await SearchCSATForUsers()));
            }
        }
        private void TableDependency_Rating_OnChanged(object sender, RecordChangedEventArgs<RatingTable> e)
        {
            if (this.IsDisposed || !this.IsHandleCreated) return;
            if (e.ChangeType != ChangeType.None)
            {
                this.BeginInvoke((MethodInvoker)(async () => await LastTickets(_mainMenu.dateFiltering.Text)));
                this.BeginInvoke((MethodInvoker)(async () => await SearchCSATForUsers()));
            }
        }
        private void StartTableDependency()
        {
            var cts = new CancellationTokenSource(); // Create a new CancellationTokenSource
            Task.Run(async () =>
            {
                try
                {
                    await TicketTableDependency(cts.Token);
                    await StatusTableDependency(cts.Token);
                    await UserTableDependency(cts.Token);
                    await RatingTableDependency(cts.Token);
                }
                catch (OperationCanceledException ex)
                {
                    Notifications.Error("Table dependencies were canceled.", "Error");
                    await Logger.Log(_fullname, $"Table dependencies were canceled in DashboardForAdminPanel. Error : {ex.Message}");
                }
                catch (Exception ex)
                {
                    await Logger.Log(_fullname, $"An error occurred in DashboardForAdminPanel: {ex.Message}");
                }
            }, cts.Token);
        }
        private async Task ConnectDependenciesToDatabase()
        {
            _connection_string = ConfigurationManager.ConnectionStrings["ServiceDesk"].ConnectionString;
            await Task.Delay(1000);
            _tableDependency_Ticket ??= new SqlTableDependency<TicketTable>(_connection_string, "Ticket");
            await Task.Delay(1000);
            _tableDependency_Status ??= new SqlTableDependency<StatusTable>(_connection_string, "Status");
            await Task.Delay(1000);
            _tableDependency_Rating ??= new SqlTableDependency<RatingTable>(_connection_string, "Rating");
            await Task.Delay(1000);
            _tableDependency_User ??= new SqlTableDependency<UserTable>(_connection_string, "Users");
        }
        private async Task TicketTableDependency(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            // Ensure the previous instance is disposed before creating a new one
            _tableDependency_Ticket?.Stop();
            _tableDependency_Ticket?.Dispose();

            if (_tableDependency_Ticket != null)
            {
                _tableDependency_Ticket.OnChanged += TableDependency_Ticket_OnChanged;
                _tableDependency_Ticket.OnError += TableDependency_OnError;
                _tableDependency_Ticket.Start();
            }

            await Task.Delay(1000, cancellationToken); // Simulate work
        }
        private async Task StatusTableDependency(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            // Ensure the previous instance is disposed before creating a new one
            _tableDependency_Status?.Stop();
            _tableDependency_Status?.Dispose();

            if (_tableDependency_Status != null)
            {
                _tableDependency_Status.OnChanged += TableDependency_Status_OnChanged;
                _tableDependency_Status.OnError += TableDependency_OnError;
                _tableDependency_Status.Start();
            }
            await Task.Delay(1000, cancellationToken); // Simulate work
        }
        private async Task UserTableDependency(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            // Ensure the previous instance is disposed before creating a new one
            _tableDependency_User?.Stop();
            _tableDependency_User?.Dispose();

            if (_tableDependency_User != null)
            {
                _tableDependency_User.OnChanged += TableDependency_User_OnChanged;
                _tableDependency_User.OnError += TableDependency_OnError;
                _tableDependency_User.Start();
            }
            await Task.Delay(1000, cancellationToken); // Simulate work
        }
        private async Task RatingTableDependency(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Ensure the previous instance is disposed before creating a new one
            _tableDependency_Rating?.Stop();
            _tableDependency_Rating?.Dispose();

            if (_tableDependency_Rating != null)
            {
                _tableDependency_Rating.OnChanged += TableDependency_Rating_OnChanged;
                _tableDependency_Rating.OnError += TableDependency_OnError;
                _tableDependency_Rating.Start();
            }
            await Task.Delay(1000, cancellationToken); // Simulate work
        }
        private async Task StopTableDependencyAsync()
        {
            try
            {
                if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
                {
                    _cancellationTokenSource.Cancel();
                }

                // Stop and dispose table dependencies
                _tableDependency_Rating?.Stop();
                _tableDependency_Rating?.Dispose();
                _tableDependency_Rating = null;

                _tableDependency_Status?.Stop();
                _tableDependency_Status?.Dispose();
                _tableDependency_Status = null;

                _tableDependency_Ticket?.Stop();
                _tableDependency_Ticket?.Dispose();
                _tableDependency_Ticket = null;

                _tableDependency_User?.Stop();
                _tableDependency_User?.Dispose();
                _tableDependency_User = null;
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message, "Error occurred while stopping table dependencies");
                await Logger.Log(_fullname, $"Error occurred in DashboardForAdminPanel while stopping table dependencies in DashboardForAdminPanel. Error: {ex.Message}");
            }
            finally
            {
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = new CancellationTokenSource();
            }
        }
        #endregion
    }
}