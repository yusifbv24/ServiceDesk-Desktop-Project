using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ServiceDesk.Class;
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base.Enums;
using TableDependency.SqlClient.Base.EventArgs;
using static ServiceDesk.Class.TableDependencies;

namespace ServiceDesk.Forms
{
    public partial class ClosedTicketsForAdminPanel : Form
    {
        private Main _mainMenu;
        private readonly string _fullname = default;
        private SqlTableDependency<TicketTable> _tableDependency_Ticket;
        private SqlTableDependency<StatusTable> _tableDependency_Status;
        private SqlTableDependency<RatingTable> _tableDependency_Rating;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private SqlConnection _connection { get; set; } = null;
        private static string _connection_string { get; set; } = null;
        public ClosedTicketsForAdminPanel(string _fullname, Main mainMenu, out ClosedTicketsForAdminPanel closedTickets)
        {
            InitializeComponent();
            closedTickets = this;
            this._fullname = _fullname;
            _mainMenu = mainMenu;
            _ = LoadTickets();
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
        private string RemoveStringFromTime(string text)
        {
            if (text.Contains("ago"))
            {
                text = text.Replace("ago", "");
                return text;
            }
            else
                return text;
        }
        public async Task LoadTickets()
        {
            dgvTicket.Rows.Clear();

            string query = @"SELECT Ticket.*, Rating.rating, Rating.message 
                 FROM Rating 
                 INNER JOIN Ticket WITH (NOLOCK) ON Rating.ID = Ticket.ID 
                 INNER JOIN Status WITH (NOLOCK) ON Status.ID = Ticket.ID 
                 WHERE (Status.status='closed' OR Status.status='resolved') ";
            if (!string.IsNullOrEmpty(_mainMenu.fromDate) && !string.IsNullOrEmpty(_mainMenu.toDate))
            {
                query += " AND Status.time BETWEEN @fromDate AND @toDate";
            }
            if (!string.IsNullOrEmpty(_mainMenu.txtSearch.Text))
            {
                query += @" AND (Ticket.ID LIKE @searchText 
                  OR code LIKE @searchText 
                  OR dep_name LIKE @searchText 
                  OR worker LIKE @searchText 
                  OR device LIKE @searchText 
                  OR task LIKE @searchText 
                  OR solution LIKE @searchText 
                  OR creation_date LIKE @searchText 
                  OR fullname LIKE @searchText 
                  OR finished_time LIKE @searchText 
                  OR taken_time LIKE @searchText
                  OR rating LIKE @searchText
                  OR message LIKE @searchText )";
            }
            query += " ORDER BY Status.ID DESC";
            try
            {
                if (_connection == null || _connection.State == ConnectionState.Closed)
                {
                    await ConnectToTheDatabase();
                }
                using SqlCommand cm = new(query, _connection);
                cm.Parameters.AddWithValue("@fromDate", _mainMenu.fromDate);
                cm.Parameters.AddWithValue("@toDate", _mainMenu.toDate);
                // Add search parameter only if searchText is not empty
                if (!string.IsNullOrEmpty(_mainMenu.txtSearch.Text))
                {
                    cm.Parameters.AddWithValue("@searchText", $"%{_mainMenu.txtSearch.Text}%");
                }
                using (var dr = await cm.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        dgvTicket.Rows.Add(
                            dr["ID"].ToString(),
                            dr["code"].ToString(),
                            dr["dep_name"].ToString(),
                            dr["worker"].ToString(),
                            dr["device"].ToString(),
                            dr["task"].ToString(),
                            dr["solution"].ToString(),
                            dr["creation_date"].ToString(),
                            dr["finished_time"].ToString(),
                            RemoveStringFromTime(dr["taken_time"].ToString()),
                            dr["fullname"].ToString(),
                            dr["rating"].ToString(),
                            dr["message"].ToString()
                    );
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                await Logger.Log(_fullname, $" | InvalidOperationException in ClosedTicketForAdminPanel while loading tickets| Error is: {ex.Message}");
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message, "Error occured while loading data");
                await Logger.Log(_fullname, $" | Error occured in ClosedTicketForAdmin Panel when loading tickets. | Error is: {ex.Message}");
            }
            finally
            {
                _mainMenu.lblTotalResult.Text = dgvTicket.Rows.Count.ToString();
            }
        }
        private float ConvertRatingValue(string value)
        {
            if (float.TryParse(value, out float rating))
            {
                return rating;
            }
            else
            {
                return 0; // Default value if parsing fails
            }
        }
        private async Task EditingCell(DataGridViewCellEventArgs e)
        {
            var task = dgvTicket.Rows[e.RowIndex].Cells[5].Value.ToString();

            TicketModule ticktModule = new(_fullname, _mainMenu._userType, _mainMenu);
            ticktModule.ticketID = Convert.ToInt32(dgvTicket.Rows[e.RowIndex].Cells[0].Value);
            ticktModule.txtCode.Text = dgvTicket.Rows[e.RowIndex].Cells[1].Value.ToString();
            ticktModule.txtDep.Text = dgvTicket.Rows[e.RowIndex].Cells[2].Value.ToString();
            ticktModule.txtWorker.Text = dgvTicket.Rows[e.RowIndex].Cells[3].Value.ToString();
            ticktModule.txtDevice.Text = dgvTicket.Rows[e.RowIndex].Cells[4].Value.ToString();
            ticktModule.txtSolution.Text = dgvTicket.Rows[e.RowIndex].Cells[6].Value.ToString();
            if (!string.IsNullOrEmpty(task))
            {
                foreach (var item in task.Split(','))
                {
                    await ticktModule.AddingTasksToTable(item);
                }
            }
            foreach (var item in dgvTicket.Rows[e.RowIndex].Cells[10].Value.ToString().Split(','))
            {
                ticktModule.cmbSelectedUsers.Items.Add(item);
                if (string.IsNullOrEmpty(item))
                {
                    ticktModule.cmbSelectedUsers.Items.Clear();
                }
            }
            ticktModule.btnUpdate.Visible = true;
            ticktModule.btnClose.Visible = true;
            ticktModule.btnClose.Enabled = false;
            ticktModule.btnUpdate.Location = new Point(398, 454);
            await ticktModule.SettingsWhileUpdating();
            ticktModule.ShowDialog();
            await LoadTickets();
        }
        private async Task EvaluateCell(DataGridViewCellEventArgs e)
        {
            string users = dgvTicket.Rows[e.RowIndex].Cells[10].Value.ToString();
            try
            {
                Feedback evaluating = new(_fullname, _mainMenu._sessionId);
                evaluating.ID = dgvTicket.Rows[e.RowIndex].Cells[0].Value.ToString();
                evaluating.Rating.Value = ConvertRatingValue(dgvTicket.Rows[e.RowIndex].Cells[11].Value.ToString());
                evaluating.txtMessage.Text = dgvTicket.Rows[e.RowIndex].Cells[12].Value.ToString();
                evaluating.ShowDialog();
            }
            catch (Exception ex)
            {
                Notifications.Error($"{ex.Message}", "Error occured while evaluating ticket");
                await Logger.Log(_fullname, $" | Error occured in ClosedTicketForAdminPanel while running evaluating in EditClosedTicketsByAdmin. | Error is: {ex.Message}");
            }
            finally
            {
                await LoadTickets();
                await CalculateRatings(users);
            }
        }
        private async Task ResolveCell(DataGridViewCellEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Are you sure you want to resolve this ticket?", "Restore Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string query = @"UPDATE Ticket 
                                            SET finished_time=@finished_time,
                                            taken_time=@taken_time 
                                            WHERE ID=@ID

                                            UPDATE Rating 
                                            SET rating=@rating
                                            WHERE ID=@ID

                                            UPDATE Status 
                                            SET status=@status 
                                            WHERE ID=@ID";
                    var ID = dgvTicket.Rows[e.RowIndex].Cells[0].Value.ToString();
                    if (_connection == null || _connection.State == ConnectionState.Closed)
                    {
                        await ConnectToTheDatabase();
                    }
                    using SqlCommand cm = new(query, _connection);
                    cm.Parameters.AddWithValue("@ID", ID);
                    cm.Parameters.AddWithValue("@finished_time", "");
                    cm.Parameters.AddWithValue("@taken_time", "");
                    cm.Parameters.AddWithValue("@rating", "");
                    cm.Parameters.AddWithValue("@status", "resolving");
                    await cm.ExecuteNonQueryAsync();
                    if (cm != null)
                    {
                        Notifications.Information("You have restored a ticket succesfully", "Succesful");
                        await Logger.Log(_fullname, $" restored a ticket with Ticket_ID [{ID}] from Ticket Table");
                    }
                }
            }
            catch (Exception ex)
            {
                Notifications.Error($"{ex.Message}", "Error occured while resolving ticket");
                await Logger.Log(_fullname, $" | Error occured in ClosedTicketForAdminPanel while running resolving in EditClosedTicketsByAdmin. | Error is: {ex.Message}");
            }
            finally
            {
                _connection.Close();
                await LoadTickets();
            }
        }
        private async Task DeleteCell(DataGridViewCellEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Are you sure you want to delete this ticket?", "Delete Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string query = @"DELETE FROM TICKET WHERE ID=@ID
                                             DELETE FROM Status WHERE ID=@ID
                                             DELETE FROM Rating WHERE ID=@ID";
                    if (_connection == null || _connection.State == ConnectionState.Closed)
                    {
                        await ConnectToTheDatabase();
                    }
                    var ID = dgvTicket.Rows[e.RowIndex].Cells[0].Value.ToString();
                    using SqlCommand cm = new(query, _connection);
                    cm.Parameters.AddWithValue("@ID", ID);
                    cm.ExecuteNonQuery();
                    if (cm != null)
                    {
                        Notifications.Information("Ticket has been deleted succesfully", "Succesful");
                        await Logger.Log(_fullname, $" deleted a ticket with Ticket_ID [{ID}] from Ticket Table");
                    }
                }
            }
            catch (Exception ex)
            {
                Notifications.Error($"{ex.Message}", "Error occured while deleting ticket");
                await Logger.Log(_fullname, $" | Error occured in ClosedTicketForAdminPanel while running deleting in EditClosedTicketsByAdmin. | Error is: {ex.Message}");
            }
            finally
            {
                _connection.Close();
                await LoadTickets();
            }
        }
        private async void DgvTicket_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                string colName = dgvTicket.Columns[e.ColumnIndex].Name;
                // Check if the click is on a valid cell (not the header)
                if (e.RowIndex < 0 || e.ColumnIndex < 0)
                {
                    return; // Ignore header clicks
                }

                // Ensure the DataGridView has rows and columns
                if (dgvTicket.Rows.Count == 0 || dgvTicket.Columns.Count == 0)
                {
                    return; // No data to process
                }
                if (colName == "Evaluate")
                {
                    await EvaluateCell(e);
                    return;
                }
                if (colName == "Resolve")
                {
                    await ResolveCell(e);
                    return;
                }
                if (colName == "Delete")
                {
                    await DeleteCell(e);
                    return;
                }
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    var cellValue = dgvTicket.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString();
                    if (!string.IsNullOrEmpty(cellValue))
                    {
                        await EditingCell(e);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message, "Error occured while editing ticket");
                await Logger.Log(_fullname, $" | Error occured in DgvTicket_CellContentClick when EditClosedTicketsByAdmin items. | Error is: {ex.Message}");
            }
        }
        #region Calculating CSAT for Each user
        private async Task CalculateRatings(string users)
        {
            if (users.Contains(","))
            {
                string[] _userCollection = users.Split(',');
                foreach (string _user in _userCollection)
                {
                    await Ratings.CalculateCSAT(_user,_mainMenu._sessionId);
                }
            }
            else
            {
                await Ratings.CalculateCSAT(users, _mainMenu._sessionId);
            }
        }
        #endregion
        #region SqlTableDependency
        private async void ClosedTicketsForAdminPanel_FormClosing(object sender, FormClosingEventArgs e)
        {
            await StopTableDependencyAsync();
        }
        private void TableDependency_Ticket_OnChanged(object sender, RecordChangedEventArgs<TicketTable> e)
        {
            if (this.IsDisposed && !this.IsHandleCreated)
                return;
            if (e.ChangeType != ChangeType.None)
            {
                this.BeginInvoke((MethodInvoker)(async () => await LoadTickets()));
            }
        }
        private void TableDependency_Status_OnChanged(object sender, RecordChangedEventArgs<StatusTable> e)
        {
            if (this.IsDisposed && !this.IsHandleCreated) return;
            if (e.ChangeType != ChangeType.None)
            {
                this.BeginInvoke((MethodInvoker)(async () => await LoadTickets()));
            }
        }
        private void TableDependency_Rating_OnChanged(object sender, RecordChangedEventArgs<RatingTable> e)
        {
            if (this.IsDisposed && !this.IsHandleCreated) return;
            if (e.ChangeType != ChangeType.None)
            {
                this.BeginInvoke((MethodInvoker)(async () => await LoadTickets()));
            }
        }
        private async void TableDependency_OnError(object sender, ErrorEventArgs e)
        {
            await Logger.Log(_fullname, "Error occured while running table dependency in ClosedTicketForAdminPanel");
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
                    await RatingTableDependency(cts.Token);
                }
                catch (OperationCanceledException ex)
                {
                    Notifications.Error("Table dependencies were canceled.", "Error");
                    await Logger.Log(_fullname, $"Table dependencies were canceled in ClosedTicketForAdminPanel. Error : {ex.Message}");
                }
                catch (Exception ex)
                {
                    await Logger.Log(_fullname, $"An error occurred in ClosedTicket: {ex.Message}");
                }
            }, cts.Token);
        }
        private async Task ConnectDependenciesToDatabase()
        {
            _connection_string= ConfigurationManager.ConnectionStrings["ServiceDesk"].ConnectionString;
            await Task.Delay(1000);
            _tableDependency_Ticket ??= new SqlTableDependency<TicketTable>(_connection_string, "Ticket");
            await Task.Delay(1000);
            _tableDependency_Status ??= new SqlTableDependency<StatusTable>(_connection_string, "Status");
            await Task.Delay(1000);
            _tableDependency_Rating ??= new SqlTableDependency<RatingTable>(_connection_string, "Rating");
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
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message, "Error occurred while stopping table dependencies");
                await Logger.Log(_fullname, $"Error occurred in DashboardForAdminPanel while stopping table dependencies in ClosedTicketForAdminPanel. Error: {ex.Message}");
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