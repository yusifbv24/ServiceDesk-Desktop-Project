using Bunifu.Framework.UI;
using ServiceDesk.Class;
using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ServiceDesk.Class.TableDependencies;
using TableDependency.SqlClient.Base.Enums;
using TableDependency.SqlClient.Base.EventArgs;
using TableDependency.SqlClient;
using System.Data;

namespace ServiceDesk.Forms
{
    public partial class OpenedTickets : Form
    {
        private readonly Connect connect = Connect.Instance;
        private Main _mainMenu;
        private readonly string _fullname = default;
        private SqlTableDependency<TicketTable> _tableDependency_Ticket;
        private SqlTableDependency<StatusTable> _tableDependency_Status;
        private SqlConnection connection { get; set; } = null;
        public OpenedTickets(string _fullname, Main mainMenu,out OpenedTickets form)
        {
            InitializeComponent();
            _mainMenu = mainMenu;
            form = this;
            this._fullname = _fullname;
            _ = LoadTickets();
            StartTableDependency();
        }
        private async Task CreateConnectionWithDatabase()
        {
            if (connection == null)
            {
                connection = await connect.EstablishConnectionWithServiceDeskAsync(_mainMenu._sessionId).ConfigureAwait(false);
            }
            await connection.OpenAsync();
        }
        private string CalculateTime(DateTime OpenedTime)
        {
            var closedTime = DateTime.Now;
            TimeSpan _calculatedTime = closedTime - OpenedTime;
            // If the time difference is less than 60 seconds
            if (_calculatedTime.TotalSeconds < 60)
            {
                return $"{Math.Floor(_calculatedTime.TotalSeconds)} seconds ago";
            }
            // If the time difference is less than 60 minutes
            else if (_calculatedTime.TotalMinutes < 60)
            {
                return $"{Math.Floor(_calculatedTime.TotalMinutes)} minutes ago";
            }
            // If the time difference is less than 24 hours
            else if (_calculatedTime.TotalHours < 24)
            {
                return $"{Math.Floor(_calculatedTime.TotalHours)} hours ago";
            }
            // If the time difference is less than 30 days
            else if (_calculatedTime.TotalDays < 30)
            {
                return $"{Math.Floor(_calculatedTime.TotalDays)} days ago";
            }
            else
            {
                // For longer periods, show the actual date
                return $"on {OpenedTime:dd/MM/yyyy}";
            }
        }
        public async Task LoadTickets()
        {
            try
            {
                dgvTask.Rows.Clear();
                string query = @"SELECT Ticket.ID,code,dep_name,worker,device,task,solution,creation_date FROM Ticket 
                            INNER JOIN Status WITH (NOLOCK) ON Status.ID=Ticket.ID
							WHERE ((Status.status='pending' OR Status.status='resolving')
                            AND fullname LIKE @fullname) ";
                if (!string.IsNullOrEmpty(_mainMenu.txtSearch.Text))
                {
                    query = @" AND (Ticket.ID LIKE @searchText 
                     OR code LIKE @searchText 
                     OR dep_name LIKE @searchText 
                     OR worker LIKE @searchText 
                     OR device LIKE @searchText 
                     OR task LIKE @searchText 
                     OR solution LIKE @searchText 
                     OR creation_date LIKE @searchText ) ";
                }
                query += " ORDER BY Ticket.creation_date DESC, Ticket.ID DESC";

                if (connection is null || connection.State == ConnectionState.Closed)
                {
                    await CreateConnectionWithDatabase();
                }
                using var cm = new SqlCommand(query, connection);
                cm.Parameters.AddWithValue("@fullname", $"%{_fullname}%");
                // Add search parameter only if searchText is not empty
                if (!string.IsNullOrEmpty(_mainMenu.txtSearch.Text))
                {
                    cm.Parameters.AddWithValue("@searchText", $"%{_mainMenu.txtSearch.Text}%");
                }
                using var dr = await cm.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                while (await dr.ReadAsync())
                {
                    dgvTask.Rows.Add(
                        dr["ID"].ToString(),
                        dr["code"].ToString(),
                        dr["dep_name"].ToString(),
                        dr["worker"].ToString(),
                        dr["device"].ToString(),
                        dr["task"].ToString(),
                        dr["solution"].ToString(),
                        dr["creation_date"].ToString(),
                        CalculateTime(DateTime.Parse(dr["creation_date"].ToString())));
                }
            }
            catch (InvalidOperationException ex)
            {
                await Logger.Log(_fullname, $" | InvalidOperationException in OpenedTicketForAdminPanel while loading tickets| Error is: {ex.Message}");
            }
            catch (Exception ex)
            {
                Notifications.Error($"{ex.Message}", "Error occured while loading tickets");
                await Logger.Log(_fullname, $" | Error occured in OpenedTicket Panel when loading tickets. | Error is: {ex.Message}");
            }
            finally
            {
                _mainMenu.lblTotalResult.Text= dgvTask.Rows.Count.ToString();
            }
        }
        private async Task EditingCell(DataGridViewCellEventArgs e)
        {
            var problems = dgvTask.Rows[e.RowIndex].Cells[5].Value.ToString();
            TicketModule ticktModule = new(_fullname, _mainMenu._userType, _mainMenu);
            //adding information to items
            ticktModule.ticketID = Convert.ToInt32(dgvTask.Rows[e.RowIndex].Cells[0].Value);
            ticktModule.txtCode.Text = dgvTask.Rows[e.RowIndex].Cells[1].Value.ToString();
            ticktModule.txtDep.Text = dgvTask.Rows[e.RowIndex].Cells[2].Value.ToString();
            ticktModule.txtWorker.Text = dgvTask.Rows[e.RowIndex].Cells[3].Value.ToString();
            ticktModule.txtDevice.Text = dgvTask.Rows[e.RowIndex].Cells[4].Value.ToString();
            ticktModule.txtSolution.Text = dgvTask.Rows[e.RowIndex].Cells[6].Value.ToString();
            ticktModule.timeElapsed = dgvTask.Rows[e.RowIndex].Cells[7].Value.ToString();
            if (!string.IsNullOrEmpty(problems))
            {
                foreach (var item in problems.Split(','))
                {
                    await ticktModule.AddingTasksToTable(item);
                }
            }
            var users = await FindUserForID(dgvTask.Rows[e.RowIndex].Cells[0].Value.ToString());
            foreach (var item in users.Split(','))
            {
                ticktModule.cmbSelectedUsers.Items.Add(item);
                if (string.IsNullOrEmpty(item))
                {
                    ticktModule.cmbSelectedUsers.Items.Clear();
                }
            }
            ticktModule.btnClose.Visible = true;
            ticktModule.btnUpdate.Visible = true;
            ticktModule.btnUpdate.Location = new Point(398, 454);
            ticktModule.LoadDefaultSettings();
            await ticktModule.SettingsWhileUpdating();
            ticktModule.ShowDialog();
            await LoadTickets();
        }
        private async void DgvTicket_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                string colName = dgvTask.Columns[e.ColumnIndex].Name;
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    var cellValue = dgvTask.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString();
                    if (!string.IsNullOrEmpty(cellValue))
                    {
                        await EditingCell(e);
                    }
                }
            }
            catch (Exception ex)
            {
                Notifications.Error($"{ex.Message}", "Error occured while editing tickets");
                await Logger.Log(_fullname, $" | Error occured in OpenedTicket Panel when editing tickets. | Error is: {ex.Message}");
            }
        }
        private async Task<string> FindUserForID(string ID)
        {
            try
            {
                string _user = string.Empty;
                if (connection is null || connection.State == ConnectionState.Closed)
                {
                    await CreateConnectionWithDatabase();
                }
                using var cm = new SqlCommand("SELECT fullname FROM Ticket WHERE ID=@ID ", connection);
                cm.Parameters.AddWithValue("@ID", ID);
                using var dr =await cm.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                while (dr.Read())
                {
                    _user = dr["fullname"].ToString();
                }

                return _user;
            }
            catch (Exception ex)
            {
                Notifications.Error($"{ex.Message}", "Error occured while finding user ID");
                await Logger.Log(_fullname, $" | Error is occured when loading FindUsersForID in Ticket Module Panel. | Error is: {ex.Message}");
                throw;
            }
        }
        #region SqlTableDependency
        private void StartTableDependency()
        {
            Task.Run(async () =>
            {
                await Task.Delay(1000);
                await TicketTableDependency();
                await Task.Delay(1000);
                await StatusTableDependency();
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
        private async void TableDependency_OnError(object sender, ErrorEventArgs e)
        {
            await Logger.Log(_fullname, "Error occured while running table dependency in OpenTicket");
        }
        private async Task StopTableDependency()
        {
            await SafeStop(_tableDependency_Ticket, "Ticket");
            await SafeStop(_tableDependency_Status, "Status");
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
                    await Logger.Log(_fullname, $"{name} dependency is already disposed in OpenTicket.");
                }
                catch (InvalidOperationException ex)
                {
                    await Logger.Log(_fullname, $"Invalid operation while stopping {name} in OpenTicket: {ex.Message}");
                }
                catch (AggregateException ex)
                {
                    await Logger.Log(_fullname, $"AggregateException occurred in {name} in OpenTicket: {ex.InnerException?.Message}");
                }
                catch (Exception ex)
                {
                    await Logger.Log(_fullname, $"Error stopping {name} in OpenTicket: {ex.Message}");
                }
            }
        }
        private async void OpenedTickets_FormClosing(object sender, FormClosingEventArgs e)
        {
            await StopTableDependency();
        }
        #endregion
    }
}