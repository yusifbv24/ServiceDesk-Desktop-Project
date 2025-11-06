using Bunifu.Framework.UI;
using ServiceDesk.Class;
using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
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
        private SqlConnection _connection { get; set; } = null;
        private DataTable _allTicketsData;
        private System.Windows.Forms.Timer _searchDebounceTimer;

        public OpenedTickets(string _fullname, Main mainMenu, out OpenedTickets form)
        {
            InitializeComponent();
            _mainMenu = mainMenu;
            form = this;
            this._fullname = _fullname;

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
            dgvTask.Rows.Clear();

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
            }
            else
            {
                _allTicketsData.Clear();
            }

            string query = @"SELECT Ticket.ID,code,dep_name,worker,device,task,solution,creation_date FROM Ticket 
                        INNER JOIN Status WITH (NOLOCK) ON Status.ID=Ticket.ID
                        WHERE ((Status.status='pending' OR Status.status='resolving')
                        AND fullname LIKE @fullname)
                        ORDER BY Ticket.creation_date DESC, Ticket.ID DESC";

            try
            {
                if (_connection == null || _connection.State == ConnectionState.Closed)
                {
                    await ConnectToTheDatabase();
                }

                using var cm = new SqlCommand(query, _connection);
                cm.Parameters.AddWithValue("@fullname", $"%{_fullname}%");
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
                        CalculateTime(DateTime.Parse(dr["creation_date"].ToString()))
                    );
                }

                ApplyLocalFilter("");
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
        }

        private void ApplyLocalFilter(string searchText)
        {
            if (_allTicketsData == null || _allTicketsData.Rows.Count == 0)
            {
                _mainMenu.lblTotalResult.Text = "0";
                return;
            }

            dgvTask.Rows.Clear();
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
                creation_date LIKE '%{EscapeFilterValue(searchText)}%'";

                filteredRows = _allTicketsData.Select(filterExpression);
            }

            foreach (var row in filteredRows)
            {
                dgvTask.Rows.Add(
                    row["ID"],
                    row["code"],
                    row["dep_name"],
                    row["worker"],
                    row["device"],
                    row["task"],
                    row["solution"],
                    row["creation_date"],
                    row["time_elapsed"]
                );
            }

            _mainMenu.lblTotalResult.Text = dgvTask.Rows.Count.ToString();
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
    }
}