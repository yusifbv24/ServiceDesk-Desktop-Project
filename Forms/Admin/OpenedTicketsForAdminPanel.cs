using ServiceDesk.Class;
using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServiceDesk.Forms
{
    public partial class OpenedTicketsForAdminPanel : Form
    {
        private Main _mainMenu;
        private readonly string _fullname = default;
        private SqlConnection _connection { get; set; } = null;
        private static string _connection_string { get; set; } = null;
        private DataTable _allTicketsData;
        private System.Windows.Forms.Timer _searchDebounceTimer;
        public OpenedTicketsForAdminPanel(string _fullname, Main mainMenu, out OpenedTicketsForAdminPanel form)
        {
            InitializeComponent();
            _mainMenu = mainMenu;
            form = this;
            this._fullname = _fullname;

            // Initialize the debounce timer
            _searchDebounceTimer = new System.Windows.Forms.Timer();
            _searchDebounceTimer.Interval = 300;
            _searchDebounceTimer.Tick += SearchDebounceTimer_Tick;

            _ = LoadTickets();
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
            dgvTicket.Rows.Clear();

            // Create DataTable if it doesn't exist
            if (_allTicketsData == null)
            {
                _allTicketsData = new DataTable();
                _allTicketsData.Columns.Add("ID", typeof(string));
                _allTicketsData.Columns.Add("code", typeof(string));
                _allTicketsData.Columns.Add("dep_name", typeof(string));
                _allTicketsData.Columns.Add("worker", typeof(string));
                _allTicketsData.Columns.Add("device", typeof(string));
                _allTicketsData.Columns.Add("task", typeof(string));
                _allTicketsData.Columns.Add("solution", typeof(string));
                _allTicketsData.Columns.Add("creation_date", typeof(string));
                _allTicketsData.Columns.Add("time_elapsed", typeof(string));
                _allTicketsData.Columns.Add("fullname", typeof(string));
            }
            else
            {
                _allTicketsData.Clear();
            }

            // Query WITHOUT search filter - we filter in memory
            string query = @"SELECT Ticket.ID,code,dep_name,worker,device,task,solution,creation_date,fullname
                        FROM Ticket 
                        INNER JOIN Status WITH (NOLOCK) ON Status.ID=Ticket.ID
                        WHERE (Status.status='pending' OR Status.status='resolving')
                        ORDER BY Ticket.creation_date DESC, Ticket.ID DESC";

            try
            {
                if (_connection == null || _connection.State == ConnectionState.Closed)
                {
                    await ConnectToTheDatabase();
                }

                using var cm = new SqlCommand(query, _connection);
                using var dr = await cm.ExecuteReaderAsync();

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
                        dr["creation_date"].ToString(),
                        CalculateTime(DateTime.Parse(dr["creation_date"].ToString())),
                        dr["fullname"].ToString()
                    );
                }

                // Display all data initially
                ApplyLocalFilter("");
            }
            catch (InvalidOperationException ex)
            {
                await Logger.Log(_fullname, $" | InvalidOperationException in OpenedTicketForAdminPanel while loading tickets| Error is: {ex.Message}");
            }
            catch (Exception ex)
            {
                Notifications.Error($"{ex.Message}", "Error occured while loading data");
                await Logger.Log(_fullname, $" | Error occured in OpenedTicketForAdminPanel Panel when loading tickets. | Error is: {ex.Message}");
            }
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
                filteredRows = _allTicketsData.Select();
            }
            else
            {
                string filterExpression = $@"
                ID LIKE '%{EscapeFilterValue(searchText)}%' OR 
                code LIKE '%{EscapeFilterValue(searchText)}%' OR 
                dep_name LIKE '%{EscapeFilterValue(searchText)}%' OR 
                worker LIKE '%{EscapeFilterValue(searchText)}%' OR 
                device LIKE '%{EscapeFilterValue(searchText)}%' OR 
                task LIKE '%{EscapeFilterValue(searchText)}%' OR 
                solution LIKE '%{EscapeFilterValue(searchText)}%' OR 
                creation_date LIKE '%{EscapeFilterValue(searchText)}%' OR 
                fullname LIKE '%{EscapeFilterValue(searchText)}%'";

                filteredRows = _allTicketsData.Select(filterExpression);
            }

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
                    row["creation_date"],
                    row["time_elapsed"],
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

        private async Task EditingCell(DataGridViewCellEventArgs e)
        {
            var tasks = dgvTicket.Rows[e.RowIndex].Cells[5].Value.ToString();

            TicketModule ticktModule = new(_fullname, _mainMenu._userType, _mainMenu);
            ticktModule.ticketID = Convert.ToInt32(dgvTicket.Rows[e.RowIndex].Cells[0].Value);
            ticktModule.txtCode.Text = dgvTicket.Rows[e.RowIndex].Cells[1].Value.ToString();
            ticktModule.txtDep.Text = dgvTicket.Rows[e.RowIndex].Cells[2].Value.ToString();
            ticktModule.txtWorker.Text = dgvTicket.Rows[e.RowIndex].Cells[3].Value.ToString();
            ticktModule.txtDevice.Text = dgvTicket.Rows[e.RowIndex].Cells[4].Value.ToString();
            ticktModule.txtSolution.Text = dgvTicket.Rows[e.RowIndex].Cells[6].Value.ToString();
            ticktModule.timeElapsed = dgvTicket.Rows[e.RowIndex].Cells[7].Value.ToString();
            if (!string.IsNullOrEmpty(tasks))
            {
                foreach (var item in tasks.Split(','))
                {
                    await ticktModule.AddingTasksToTable(item);
                }
            }
            foreach (var item in dgvTicket.Rows[e.RowIndex].Cells[9].Value.ToString().Split(','))
            {
                ticktModule.cmbSelectedUsers.Items.Add(item);
                if (string.IsNullOrEmpty(item))
                {
                    ticktModule.cmbSelectedUsers.Items.Clear();
                }
            }
            ticktModule.btnUpdate.Visible = true;
            ticktModule.btnClose.Visible = true;
            ticktModule.btnUpdate.Location = new Point(398, 454);
            ticktModule.LoadDefaultSettings();
            await ticktModule.SettingsWhileUpdating();
            ticktModule.ShowDialog();
            await LoadTickets();
        }
        private async Task DeleteCell(DataGridViewCellEventArgs e)
        {
            try
            {
                string query = @"DELETE FROM TICKET WHERE ID=@ID
                                             DELETE FROM Status WHERE ID=@ID
                                             DELETE FROM Rating WHERE ID=@ID";
                if (_connection == null || _connection.State == ConnectionState.Closed)
                {
                    await ConnectToTheDatabase();
                }
                using var cm = new SqlCommand(query, _connection);
                cm.Parameters.AddWithValue("@ID", dgvTicket.Rows[e.RowIndex].Cells[0].Value.ToString());
                await cm.ExecuteNonQueryAsync();
                if (cm != null)
                {
                    Notifications.Information("Ticket has been successfully deleted!");
                    await Logger.Log(_fullname, $" deleted a ticket with Ticket_ID [{dgvTicket.Rows[e.RowIndex].Cells[0].Value}] from Ticket Table");
                    await LoadTickets();
                }
            }
            catch (Exception ex)
            {
                Notifications.Error($"{ex.Message}", "Error occured while loading data");
                await Logger.Log(_fullname, $" | Error occured in OpenedTicketForAdminPanel Panel when loading tickets. | Error is: {ex.Message}");
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
                if (colName == "Delete")
                {
                    if (MessageBox.Show("Are you sure you want to delete this open ticket?", "Delete Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        await DeleteCell(e);
                    }
                    return;
                }
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    var cellValue = dgvTicket.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString();
                    if (!string.IsNullOrEmpty(cellValue))
                    {
                        await EditingCell(e);
                    }
                    return;
                }
            }
            catch (Exception ex)
            {
                Notifications.Error($"{ex.Message}", "Error occured while editing data");
                await Logger.Log(_fullname, $" | Error occured in OpenedTicketForAdminPanel when editing tickets. | Error is: {ex.Message}");
            }
        }
    }
}