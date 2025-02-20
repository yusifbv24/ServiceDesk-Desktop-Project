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
using System.Data.Common;
using System.Threading;
using System.Configuration;

namespace ServiceDesk.Forms
{
    public partial class OpenedTickets : Form
    {
        private Main _mainMenu;
        private readonly string _fullname = default;
        private SqlTableDependency<TicketTable> _tableDependency_Ticket;
        private SqlTableDependency<StatusTable> _tableDependency_Status;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private static string _connection_string { get; set; } = null;
        private SqlConnection _connection { get; set; } = null;
        public OpenedTickets(string _fullname, Main mainMenu,out OpenedTickets form)
        {
            InitializeComponent();
            _mainMenu = mainMenu;
            form = this;
            this._fullname = _fullname;
            _ = LoadTickets();
            _=ConnectDependenciesToDatabase();
            StartTableDependency();
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
            dgvTask.Rows.Clear();
            string query = @"SELECT Ticket.ID,code,dep_name,worker,device,task,solution,creation_date FROM Ticket 
                            INNER JOIN Status WITH (NOLOCK) ON Status.ID=Ticket.ID
							WHERE ((Status.status='pending' OR Status.status='resolving')
                            AND fullname LIKE @fullname) ";
            if (!string.IsNullOrEmpty(_mainMenu.txtSearch.Text))
            {
                query += @" AND (Ticket.ID LIKE @searchText 
                     OR code LIKE @searchText 
                     OR dep_name LIKE @searchText 
                     OR worker LIKE @searchText 
                     OR device LIKE @searchText 
                     OR task LIKE @searchText 
                     OR solution LIKE @searchText 
                     OR creation_date LIKE @searchText ) ";
            }
            query += " ORDER BY Ticket.creation_date DESC, Ticket.ID DESC";
            try
            {
                if (_connection == null || _connection.State == ConnectionState.Closed)
                {
                    await ConnectToTheDatabase();
                }
                using var cm = new SqlCommand(query, _connection);
                cm.Parameters.AddWithValue("@fullname", $"%{_fullname}%");
                // Add search parameter only if searchText is not empty
                if (!string.IsNullOrEmpty(_mainMenu.txtSearch.Text))
                {
                    cm.Parameters.AddWithValue("@searchText", $"%{_mainMenu.txtSearch.Text}%");
                }
                using var dr = await cm.ExecuteReaderAsync();
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
                await Logger.Log(_fullname, $" | InvalidOperationException in OpenedTicket while loading tickets| Error is: {ex.Message}");
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
            string _user = string.Empty;
            try
            {
                if (_connection == null || _connection.State == ConnectionState.Closed)
                {
                    await ConnectToTheDatabase();
                }
                using var cm = new SqlCommand("SELECT fullname FROM Ticket WHERE ID=@ID ", _connection);
                cm.Parameters.AddWithValue("@ID", ID);
                using var dr = await cm.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                while (await dr.ReadAsync())
                {
                    _user = dr["fullname"].ToString();
                }
                return _user;
            }
            catch (Exception ex)
            {
                Notifications.Error($"{ex.Message}", "Error occured while finding user ID");
                await Logger.Log(_fullname, $" | Error is occured when loading FindUsersForID in Ticket Module Panel. | Error is: {ex.Message}");
                return string.Empty;
            }
        }
        #region SqlTableDependency
        private void StartTableDependency()
        {
            var cts = new CancellationTokenSource(); // Create a new CancellationTokenSource
            Task.Run(async () =>
            {
                try
                {
                    await TicketTableDependency(cts.Token);
                    await StatusTableDependency(cts.Token);
                }
                catch (OperationCanceledException ex)
                {
                    Notifications.Error("Table dependencies were canceled.", "Error");
                    await Logger.Log(_fullname, $"Table dependencies were canceled in OpenTickets. Error : {ex.Message}");
                }
                catch (Exception ex)
                {
                    await Logger.Log(_fullname, $"An error occurred in OpenTickets: {ex.Message}");
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
        private async void TableDependency_OnError(object sender, ErrorEventArgs e)
        {
            await Logger.Log(_fullname, "Error occured while running table dependency in OpenTicket");
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
                await Logger.Log(_fullname, $"Error occurred in OpenTickets while stopping table dependencies in OpenTickets. Error: {ex.Message}");
            }
            finally
            {
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = new CancellationTokenSource();
            }
        }
        private async void OpenedTickets_FormClosing(object sender, FormClosingEventArgs e)
        {
            await StopTableDependencyAsync();
        }
        #endregion
    }
}