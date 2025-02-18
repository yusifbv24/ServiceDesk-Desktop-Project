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

namespace ServiceDesk.Forms
{
    public partial class ClosedTickets : Form
    {
        private readonly Connect connect = Connect.Instance;
        private Main _mainMenu;
        private readonly string _fullname = default;
        private SqlTableDependency<TicketTable> _tableDependency_Ticket;
        private SqlTableDependency<StatusTable> _tableDependency_Status;
        private SqlConnection connection { get; set; } = null;
        public ClosedTickets(string _fullname, Main mainMenu,out ClosedTickets closedTickets)
        {
            InitializeComponent();
            this._fullname = _fullname;
            closedTickets = this;
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
            await connection.OpenAsync();
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
                dgvTicket.Visible= true;
                dgvTicket.Rows.Clear();
                string query = @"SELECT Ticket.ID,code,dep_name,worker,device,task,solution,finished_time,taken_time
                                FROM Ticket
                                INNER JOIN Status WITH (NOLOCK) ON Ticket.ID = Status.ID 
                                WHERE ((Status.status='closed' OR Status.status='resolved')
                                AND Status.time BETWEEN @fromDate AND @toDate AND fullname LIKE @fullname) ";
                if (!string.IsNullOrEmpty(_mainMenu.txtSearch.Text))
                {
                    query = @" AND (Ticket.ID LIKE @searchText 
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
                if (connection is null || connection.State == ConnectionState.Closed)
                {
                    await CreateConnectionWithDatabase();
                }
                using SqlCommand cm = new(query, connection);
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
            await Logger.Log(_fullname, "Error occured while running table dependency in ClosedTicket");
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
                    await Logger.Log(_fullname, $"{name} dependency is already disposed in ClosedTicket.");
                }
                catch (InvalidOperationException ex)
                {
                    await Logger.Log(_fullname, $"Invalid operation while stopping {name} in ClosedTicket: {ex.Message}");
                }
                catch (AggregateException ex)
                {
                    await Logger.Log(_fullname, $"AggregateException occurred in {name} in ClosedTicket: {ex.InnerException?.Message}");
                }
                catch (Exception ex)
                {
                    await Logger.Log(_fullname, $"Error stopping {name} in ClosedTicket: {ex.Message}");
                }
            }
        }
        private async void ClosedTickets_FormClosing(object sender, FormClosingEventArgs e)
        {
            await StopTableDependency();
        }
        #endregion
    }
}
