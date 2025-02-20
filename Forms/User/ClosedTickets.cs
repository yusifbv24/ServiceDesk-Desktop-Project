using ServiceDesk.Class;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows.Forms;
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base.EventArgs;
using static ServiceDesk.Class.TableDependencies;
using TableDependency.SqlClient.Base.Enums;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Configuration;

namespace ServiceDesk.Forms
{
    public partial class ClosedTickets : Form
    {
        private Main _mainMenu;
        private readonly string _fullname = default;
        private SqlTableDependency<TicketTable> _tableDependency_Ticket;
        private SqlTableDependency<StatusTable> _tableDependency_Status;

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private SqlConnection _connection { get; set; } = null;
        private static string _connection_string { get; set; } = null;
        public ClosedTickets(string _fullname, Main mainMenu,out ClosedTickets closedTickets)
        {
            InitializeComponent();
            this._fullname = _fullname;
            closedTickets = this;
            _mainMenu = mainMenu;
            _ = LoadTickets();
            _ = ConnectDependenciesToDatabase();
            StartTableDependency(); // Start listening for table changes
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
        public async Task LoadTickets()
        {
            dgvTicket.Visible = true;
            dgvTicket.Rows.Clear();
            string query = @"SELECT Ticket.ID,code,dep_name,worker,device,task,solution,finished_time,taken_time
                                FROM Ticket
                                INNER JOIN Status WITH (NOLOCK) ON Ticket.ID = Status.ID 
                                WHERE ((Status.status='closed' OR Status.status='resolved')
                                AND Status.time BETWEEN @fromDate AND @toDate AND fullname LIKE @fullname) ";
            if (!string.IsNullOrEmpty(_mainMenu.txtSearch.Text))
            {
                query += @" AND (Ticket.ID LIKE @searchText 
                               OR code LIKE @searchText 
                               OR dep_name LIKE @searchText 
                               OR worker LIKE @searchText 
                               OR device LIKE @searchText  
                               OR task LIKE @searchText 
                               OR solution LIKE @searchText 
                               OR finished_time LIKE @searchText 
                               OR taken_time LIKE @searchText) ";
            }
            query += " ORDER BY Status.ID DESC; ";
            try
            {
                if (_connection == null || _connection.State == ConnectionState.Closed)
                {
                    await ConnectToTheDatabase();
                }
                using SqlCommand cm = new(query, _connection);
                cm.Parameters.AddWithValue("@fromDate", _mainMenu.fromDate);
                cm.Parameters.AddWithValue("@toDate", _mainMenu.toDate);
                cm.Parameters.AddWithValue("@fullname", $"%{_fullname}%");
                //Add search parameter only if searchText is not empty
                if (!string.IsNullOrEmpty(_mainMenu.txtSearch.Text))
                {
                    cm.Parameters.AddWithValue("@searchText", $"%{_mainMenu.txtSearch.Text}%");
                }
                using var dr = await cm.ExecuteReaderAsync(CommandBehavior.CloseConnection);
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
                        $"{dr["finished_time"]} / {RemoveStringFromTime(dr["taken_time"].ToString())}");
                }
            }
            catch (InvalidOperationException ex)
            {
                await Logger.Log(_fullname, $" | InvalidOperationException in ClosedTicketForAdminPanel while loading tickets| Error is: {ex.Message}");
            }
            catch (Exception ex)
            {
                Notifications.Error($"{ex.Message}", "Error occured while loading tickets");
                await Logger.Log(_fullname, $" | Error occured in ClosedTicket Panel when loading tickets. | Error is: {ex.Message}");
            }
            finally
            {
                _mainMenu.lblTotalResult.Text = dgvTicket.Rows.Count.ToString();
            }
        }
        #region SqlTableDependency
        private async Task ConnectDependenciesToDatabase()
        {
            _connection_string= ConfigurationManager.ConnectionStrings["ServiceDesk"].ConnectionString;
            await Task.Delay(1000);
            _tableDependency_Ticket ??= new SqlTableDependency<TicketTable>(_connection_string, "Ticket");
            await Task.Delay(1000);
            _tableDependency_Status ??= new SqlTableDependency<StatusTable>(_connection_string, "Status");
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
                }
                catch (OperationCanceledException ex)
                {
                    Notifications.Error("Table dependencies were canceled.", "Error");
                    await Logger.Log(_fullname, $"Table dependencies were canceled in ClosedTicket. Error : {ex.Message}");
                }
                catch (Exception ex)
                {
                    await Logger.Log(_fullname, $"An error occurred in ClosedTicket: {ex.Message}");
                }
            }, cts.Token);
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
                await Logger.Log(_fullname, $"Error occurred in ClosedTicket while stopping table dependencies in ClosedTicket. Error: {ex.Message}");
            }
            finally
            {
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = new CancellationTokenSource();
            }
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
            await Logger.Log(_fullname, "Error occured while running table dependency in ClosedTicket");
        }
        private async void ClosedTickets_FormClosing(object sender, FormClosingEventArgs e)
        {
            await StopTableDependencyAsync();
        }
        #endregion
    }
}
