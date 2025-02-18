using ServiceDesk.Class;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows.Forms;
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base.EventArgs;
using static ServiceDesk.Class.TableDependencies;
using TableDependency.SqlClient.Base.Enums;

namespace ServiceDesk.Forms
{
    public partial class ClosedTickets : Form
    {
        private readonly Connect connect = Connect.Instance;
        private Main _mainMenu;
        private readonly string _fullname = default;
        private SqlTableDependency<TicketTable> _tableDependency_Ticket;
        private SqlTableDependency<StatusTable> _tableDependency_Status;
        public ClosedTickets(string _fullname, Main mainMenu,out ClosedTickets closedTickets)
        {
            InitializeComponent();
            this._fullname = _fullname;
            closedTickets = this;
            _mainMenu = mainMenu;
            _ = LoadTickets();
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
                using var connection = await connect.EstablishConnectionWithServiceDeskAsync(_mainMenu._sessionId);
                if (connection is null) return;
                using SqlCommand cm = new(query, connection);
                cm.Parameters.AddWithValue("@fromDate", _mainMenu.fromDate);
                cm.Parameters.AddWithValue("@toDate", _mainMenu.toDate);
                cm.Parameters.AddWithValue("@fullname", $"%{_fullname}%");
                //Add search parameter only if searchText is not empty
                if (!string.IsNullOrEmpty(_mainMenu.txtSearch.Text))
                {
                    cm.Parameters.AddWithValue("@searchText", $"%{_mainMenu.txtSearch.Text}%");
                }
                using SqlDataReader dr = await cm.ExecuteReaderAsync();
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
        private async void ClosedTickets_FormClosing(object sender, FormClosingEventArgs e)
        {
            await StopTableDependency();
        }
        private void StartTableDependency()
        {
            System.Threading.Tasks.Task.Run(async () =>
            {
                await TicketTableDependency();
                await System.Threading.Tasks.Task.Delay(3000);
                await StatusTableDependency();
                await System.Threading.Tasks.Task.Delay(3000);
            });
        }
        private async Task TicketTableDependency()
        {
            _tableDependency_Ticket = new SqlTableDependency<TicketTable>(connect.ServicedeskConnection, "Ticket");
            _tableDependency_Ticket.OnChanged += TableDependency_Ticket_OnChanged;
            _tableDependency_Ticket.OnError += TableDependency_OnError;
            _tableDependency_Ticket.Start();
            await System.Threading.Tasks.Task.Delay(1000);
        }
        private async Task StatusTableDependency()
        {
            _tableDependency_Status = new SqlTableDependency<StatusTable>(connect.ServicedeskConnection, "Status");
            _tableDependency_Status.OnChanged += TableDependency_Status_OnChanged;
            _tableDependency_Status.OnError += TableDependency_OnError;
            _tableDependency_Status.Start();
            await System.Threading.Tasks.Task.Delay(1000);
        }
        private async Task StopTableDependency()
        {
            _tableDependency_Ticket?.Stop();
            _tableDependency_Ticket?.Dispose();
            _tableDependency_Status?.Stop();
            _tableDependency_Status?.Dispose();
            await System.Threading.Tasks.Task.Delay(1);
        }
        private async Task RestartTableDependency()
        {
            try
            {
                await StopTableDependency();

                // Wait a moment before restarting
                await System.Threading.Tasks.Task.Delay(5000);

                // Restart dependencies
                StartTableDependency();
            }
            catch (Exception ex)
            {
                await Logger.Log(_fullname,
                    $" | Error restarting TableDependency in ClosedTicket. | Error is: {ex.Message}");
            }
        }
        private async void TableDependency_OnError(object sender, ErrorEventArgs e)
        {
            await RestartTableDependency();
        }
        private async void TableDependency_Ticket_OnChanged(object sender, RecordChangedEventArgs<TicketTable> e)
        {
            if (!this.IsDisposed && this.IsHandleCreated)
            {
                if (e.ChangeType != ChangeType.None)
                {
                    BeginInvoke((MethodInvoker)(async () => await LoadTickets()));
                }
                await System.Threading.Tasks.Task.Delay(1);
            }
        }
        private async void TableDependency_Status_OnChanged(object sender, RecordChangedEventArgs<StatusTable> e)
        {
            if (!this.IsDisposed && this.IsHandleCreated)
            {
                if (e.ChangeType != ChangeType.None)
                {
                    BeginInvoke((MethodInvoker)(async () => await LoadTickets()));
                }
                await System.Threading.Tasks.Task.Delay(1);
            }
        }
        #endregion
    }
}
