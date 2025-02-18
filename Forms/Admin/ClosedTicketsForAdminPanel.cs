using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Runtime.CompilerServices;
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
        private readonly Connect connect = Connect.Instance;
        private Main _mainMenu;
        private readonly string _fullname = default;
        private SqlTableDependency<TicketTable> _tableDependency_Ticket;
        private SqlTableDependency<StatusTable> _tableDependency_Status;
        private SqlTableDependency<RatingTable> _tableDependency_Rating;
        private SqlConnection connection { get; set; } = null;
        public ClosedTicketsForAdminPanel(string _fullname, Main mainMenu, out ClosedTicketsForAdminPanel closedTickets)
        {
            InitializeComponent();
            closedTickets = this;
            this._fullname = _fullname;
            _mainMenu = mainMenu;
            _ = LoadTickets();
            StartTableDependency(); // Start listening for table changes
        }
        private async Task CreateConnectionWithDatabase()
        {
            if (connection == null)
            {
                connection = await connect.EstablishConnectionWithServiceDeskAsync(_mainMenu._sessionId).ConfigureAwait(false);
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
            try
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
                if (connection is null||connection.State==ConnectionState.Closed)
                {
                    await CreateConnectionWithDatabase();
                }
                using SqlCommand cm = new(query, connection);
                cm.Parameters.AddWithValue("@fromDate", _mainMenu.fromDate);
                cm.Parameters.AddWithValue("@toDate", _mainMenu.toDate);
                // Add search parameter only if searchText is not empty
                if (!string.IsNullOrEmpty(_mainMenu.txtSearch.Text))
                {
                    cm.Parameters.AddWithValue("@searchText", $"%{_mainMenu.txtSearch.Text}%");
                }
                using (var dr = await cm.ExecuteReaderAsync(CommandBehavior.CloseConnection))
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
                    if (connection is null || connection.State == ConnectionState.Closed)
                    {
                        await CreateConnectionWithDatabase();
                    }
                    var ID = dgvTicket.Rows[e.RowIndex].Cells[0].Value.ToString();
                    using SqlCommand cm = new(query, connection);
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
                    if (connection is null || connection.State == ConnectionState.Closed)
                    {
                        await CreateConnectionWithDatabase();
                    }
                    var ID = dgvTicket.Rows[e.RowIndex].Cells[0].Value.ToString();
                    using SqlCommand cm = new(query, connection);
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
        private async void TableDependency_Ticket_OnChanged(object sender, RecordChangedEventArgs<TicketTable> e)
        {
            if (this.IsDisposed && !this.IsHandleCreated)
                return;
            if (e.ChangeType != ChangeType.None)
            {
                this.BeginInvoke((MethodInvoker)(async () => await LoadTickets()));
            }
            await Task.Delay(1);
        }
        private async void TableDependency_Status_OnChanged(object sender, RecordChangedEventArgs<StatusTable> e)
        {
            if (this.IsDisposed && !this.IsHandleCreated) return;
            if (e.ChangeType != ChangeType.None)
            {
                this.BeginInvoke((MethodInvoker)(async () => await LoadTickets()));
            }
            await Task.Delay(1);
        }
        private async void TableDependency_Rating_OnChanged(object sender, RecordChangedEventArgs<RatingTable> e)
        {
            if (this.IsDisposed && !this.IsHandleCreated) return;
            if (e.ChangeType != ChangeType.None)
            {
                this.BeginInvoke((MethodInvoker)(async () => await LoadTickets()));
            }
            await Task.Delay(1);
        }
        private async void TableDependency_OnError(object sender, ErrorEventArgs e)
        {
            await Logger.Log(_fullname, "Error occured while running table dependency in ClosedTicketForAdminPanel");
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
                    await Logger.Log(_fullname, $"{name} dependency is already disposed in ClosedTicketForAdminPanel.");
                }
                catch (InvalidOperationException ex)
                {
                    await Logger.Log(_fullname, $"Invalid operation while stopping {name} in ClosedTicketForAdminPanel: {ex.Message}");
                }
                catch (AggregateException ex)
                {
                    await Logger.Log(_fullname, $"AggregateException occurred in {name} in ClosedTicketForAdminPanel: {ex.InnerException?.Message}");
                }
                catch (Exception ex)
                {
                    await Logger.Log(_fullname, $"Error stopping {name} in ClosedTicketForAdminPanel: {ex.Message}");
                }
            }
        }
        #endregion
    }
}
