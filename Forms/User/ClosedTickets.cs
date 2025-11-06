using ServiceDesk.Class;
using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServiceDesk.Forms
{
    public partial class ClosedTickets : Form
    {
        private Main _mainMenu;
        private readonly string _fullname = default;
        private SqlConnection _connection { get; set; } = null;
        private DataTable _allTicketsData;
        private System.Windows.Forms.Timer _searchDebounceTimer;

        public ClosedTickets(string _fullname, Main mainMenu, out ClosedTickets closedTickets)
        {
            InitializeComponent();
            this._fullname = _fullname;
            closedTickets = this;
            _mainMenu = mainMenu;

            _searchDebounceTimer = new System.Windows.Forms.Timer();
            _searchDebounceTimer.Interval = 300;
            _searchDebounceTimer.Tick += SearchDebounceTimer_Tick;

            _ = LoadTickets();
        }

        // Add the OnSearchTextChanged method
        public void OnSearchTextChanged()
        {
            _searchDebounceTimer.Stop();
            _searchDebounceTimer.Start();
        }

        private void SearchDebounceTimer_Tick(object sender, EventArgs e)
        {
            _searchDebounceTimer.Stop();
            ApplyLocalFilter(_mainMenu.txtSearch.Text);
        }

        private void ApplyLocalFilter(string searchText)
        {
            if (_allTicketsData == null || _allTicketsData.Rows.Count == 0)
            {
                _mainMenu.lblTotalResult.Text = "0";
                return;
            }

            dgvTicket.Rows.Clear();

            DataRow[] filteredRows;

            if (string.IsNullOrWhiteSpace(searchText))
            {
                // No filter - show all rows
                filteredRows = _allTicketsData.Select();
            }
            else
            {
                // Build filter expression for all searchable columns
                string filterExpression = $@"
                ID LIKE '%{EscapeFilterValue(searchText)}%' OR 
                code LIKE '%{EscapeFilterValue(searchText)}%' OR 
                dep_name LIKE '%{EscapeFilterValue(searchText)}%' OR 
                worker LIKE '%{EscapeFilterValue(searchText)}%' OR 
                device LIKE '%{EscapeFilterValue(searchText)}%' OR 
                task LIKE '%{EscapeFilterValue(searchText)}%' OR 
                solution LIKE '%{EscapeFilterValue(searchText)}%' OR 
                finished_time LIKE '%{EscapeFilterValue(searchText)}%' OR 
                taken_time LIKE '%{EscapeFilterValue(searchText)}%' OR 
                fullname LIKE '%{EscapeFilterValue(searchText)}%' ";

                filteredRows = _allTicketsData.Select(filterExpression);
            }

            // Add filtered rows to the grid
            foreach (var row in filteredRows)
            {
                dgvTicket.Rows.Add(
                    row["ID"],
                    row["code"],
                    row["dep_name"],
                    row["worker"],
                    row["device"],
                    row["task"],
                    row["solution"],
                    row["finished_time"],
                    row["taken_time"],
                    row["fullname"]
                );
            }

            _mainMenu.lblTotalResult.Text = dgvTicket.Rows.Count.ToString();
        }

        private string EscapeFilterValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;
            return value.Replace("'", "''").Replace("[", "[[]").Replace("]", "[]]");
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
            try
            {
                dgvTicket.Rows.Clear();

                // Create a DataTable to hold our results
                _allTicketsData = new DataTable();
                _allTicketsData.Columns.Add("ID", typeof(string));
                _allTicketsData.Columns.Add("code", typeof(string));
                _allTicketsData.Columns.Add("dep_name", typeof(string));
                _allTicketsData.Columns.Add("worker", typeof(string));
                _allTicketsData.Columns.Add("device", typeof(string));
                _allTicketsData.Columns.Add("task", typeof(string));
                _allTicketsData.Columns.Add("solution", typeof(string));
                _allTicketsData.Columns.Add("finished_time", typeof(string));
                _allTicketsData.Columns.Add("taken_time", typeof(string));
                _allTicketsData.Columns.Add("fullname", typeof(string));

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
                if(_connection == null || _connection.State == ConnectionState.Closed)
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
                    _allTicketsData.Rows.Add(
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
                // Now display the data (initially unfiltered)
                ApplyLocalFilter("");
            }
            catch (InvalidOperationException ex)
            {
                await Logger.Log(_fullname, $" | InvalidOperationException in ClosedTicketFor while loading tickets| Error is: {ex.Message}");
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message, "Error occured while loading data");
                await Logger.Log(_fullname, $" | Error occured in ClosedTicket Panel when loading tickets. | Error is: {ex.Message}");
            }
        }
    }
}
