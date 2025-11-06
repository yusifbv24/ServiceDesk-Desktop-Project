using ServiceDesk.Class;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows.Forms;
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base.EventArgs;
using TableDependency.SqlClient.Base.Enums;
using System.Data;
using System.Threading;
using System.Configuration;

namespace ServiceDesk.Forms
{
    public partial class ClosedTickets : Form
    {
        private Main _mainMenu;
        private readonly string _fullname = default;
        private SqlConnection _connection { get; set; } = null;
        private static string _connection_string { get; set; } = null;
        public ClosedTickets(string _fullname, Main mainMenu,out ClosedTickets closedTickets)
        {
            InitializeComponent();
            this._fullname = _fullname;
            closedTickets = this;
            _mainMenu = mainMenu;
            _ = LoadTickets();
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
            string query = @"SELECT Ticket.ID,code,dep_name,worker,device,task,solution,finished_time,taken_time,fullname
                                FROM Ticket
                                INNER JOIN Status WITH (NOLOCK) ON Ticket.ID = Status.ID 
                                WHERE (Status.status='closed' OR Status.status='resolved') ";

            if (!string.IsNullOrEmpty(_mainMenu.fromDate) && !string.IsNullOrEmpty(_mainMenu.toDate))
            {
                query += " AND (Status.time BETWEEN @fromDate AND @toDate) ";
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
                               OR finished_time LIKE @searchText 
                               OR fullname LIKE @searchText
                               OR taken_time LIKE @searchText ) ";
            }
            query += " ORDER BY Ticket.finished_time DESC";
            try
            {
                if (_connection == null || _connection.State == ConnectionState.Closed)
                {
                    await ConnectToTheDatabase();
                }
                using SqlCommand cm = new(query, _connection);
                cm.Parameters.AddWithValue("@fromDate", _mainMenu.fromDate);
                cm.Parameters.AddWithValue("@toDate", _mainMenu.toDate);
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
                        $"{dr["finished_time"]} / {RemoveStringFromTime(dr["taken_time"].ToString())}",
                        dr["fullname"].ToString());
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
    }
}
