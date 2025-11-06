using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows.Forms;
using ServiceDesk.Class;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Runtime.InteropServices;
using System.Configuration;

namespace ServiceDesk.Forms
{
    public partial class Users : Form
    {
        private readonly string _fullname = default;
        private Main _mainMenu;
        private SqlConnection _connection { get; set; } = null;
        private static string _connection_string { get; set; } = null;

        // Our in-memory storage for all user data
        // This will hold every user record so we can filter it quickly
        private DataTable _allUsersData;

        // The debounce timer that prevents searching on every keystroke
        private System.Windows.Forms.Timer _searchDebounceTimer;

        public Users(string fullname, Main mainMenu, out Users users)
        {
            InitializeComponent();
            _fullname = fullname;
            _mainMenu = mainMenu;
            users = this;

            // Set up the debounce timer with a 300ms delay
            _searchDebounceTimer = new System.Windows.Forms.Timer();
            _searchDebounceTimer.Interval = 300;
            _searchDebounceTimer.Tick += SearchDebounceTimer_Tick;

            _ = LoadUsers();
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

        private string ChangeBooleanToString(bool status)
        {
            return status == true ? "online" : "offline";
        }

        /// <summary>
        /// Loads all users from the database and stores them in memory.
        /// Notice we're joining the Users table with UserSessions to get the online status,
        /// but we're NOT filtering by search text in the SQL query.
        /// </summary>
        public async Task LoadUsers()
        {
            dgvUser.Rows.Clear();

            // Initialize the DataTable structure to match our user data
            if (_allUsersData == null)
            {
                _allUsersData = new DataTable();
                _allUsersData.Columns.Add("ID", typeof(string));
                _allUsersData.Columns.Add("fullname", typeof(string));
                _allUsersData.Columns.Add("type", typeof(string));
                _allUsersData.Columns.Add("IsActive", typeof(string));     // "online" or "offline"
                _allUsersData.Columns.Add("LastActivity", typeof(string));
                _allUsersData.Columns.Add("session", typeof(string));
                _allUsersData.Columns.Add("ip_address", typeof(string));
            }
            else
            {
                _allUsersData.Clear();
            }

            // Load ALL users without any search filter
            string query = @"
                SELECT 
                    Users.ID,
                    Users.fullname,
                    Users.type,
                    Users.session,
                    Users.ip_address,
                    UserSessions.LastActivity,
                    COALESCE(UserSessions.IsActive, 0) AS IsActive
                FROM 
                    Users
                LEFT JOIN 
                    UserSessions ON UserSessions.UserId = Users.fullname";

            try
            {
                if (_connection == null || _connection.State == ConnectionState.Closed)
                {
                    await ConnectToTheDatabase();
                }

                using var cm = new SqlCommand(query, _connection);
                using var dr = await cm.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                // Store each user in our in-memory DataTable
                while (await dr.ReadAsync())
                {
                    _allUsersData.Rows.Add(
                        dr["ID"].ToString(),
                        dr["fullname"].ToString(),
                        dr["type"].ToString(),
                        ChangeBooleanToString(Convert.ToBoolean(dr["IsActive"])),
                        dr["LastActivity"].ToString(),
                        dr["session"].ToString(),
                        dr["ip_address"].ToString()
                    );
                }

                // Display all data initially (or apply existing search filter)
                ApplyLocalFilter(_mainMenu.txtSearch.Text);
            }
            catch (InvalidOperationException ex)
            {
                await Logger.Log(_fullname, $" | InvalidOperationException in UserPanel while loading users| Error is: {ex.Message}");
            }
            catch (Exception ex)
            {
                Notifications.Error($"{ex.Message}", "Error occured while loading users");
                await Logger.Log(_fullname, $" | Error occured when loading users in User Panel. | Error is: {ex.Message}");
            }
        }

        /// <summary>
        /// Filters the user data that's already in memory based on the search text.
        /// This searches across ALL columns - fullname, type, session, IP address, and last activity.
        /// Because we're searching in-memory data, this is extremely fast even with thousands of users.
        /// </summary>
        private void ApplyLocalFilter(string searchText)
        {
            if (_allUsersData == null || _allUsersData.Rows.Count == 0)
            {
                _mainMenu.lblTotalResult.Text = "0";
                return;
            }

            dgvUser.Rows.Clear();
            DataRow[] filteredRows;

            if (string.IsNullOrWhiteSpace(searchText))
            {
                // No search - show all users
                filteredRows = _allUsersData.Select();
            }
            else
            {
                // Build a filter that searches across all user-visible columns
                // Notice we search in fullname, type, status, session, IP, and last activity
                string filterExpression = $@"
                    fullname LIKE '%{EscapeFilterValue(searchText)}%' OR 
                    type LIKE '%{EscapeFilterValue(searchText)}%' OR 
                    IsActive LIKE '%{EscapeFilterValue(searchText)}%' OR 
                    session LIKE '%{EscapeFilterValue(searchText)}%' OR 
                    ip_address LIKE '%{EscapeFilterValue(searchText)}%' OR 
                    LastActivity LIKE '%{EscapeFilterValue(searchText)}%'";

                filteredRows = _allUsersData.Select(filterExpression);
            }

            // Populate the grid with filtered results
            foreach (var row in filteredRows)
            {
                dgvUser.Rows.Add(
                    row["ID"],
                    row["fullname"],
                    row["type"],
                    row["IsActive"],
                    row["LastActivity"],
                    row["session"],
                    row["ip_address"]
                );
            }

            _mainMenu.lblTotalResult.Text = dgvUser.Rows.Count.ToString();
        }

        /// <summary>
        /// Escapes special characters so they don't break the DataTable filter expression
        /// </summary>
        private string EscapeFilterValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;
            return value.Replace("'", "''").Replace("[", "[[]").Replace("]", "[]]");
        }

        /// <summary>
        /// Called by Main.cs when the search text changes.
        /// Starts the debounce timer to delay the actual search.
        /// </summary>
        public void OnSearchTextChanged()
        {
            _searchDebounceTimer.Stop();
            _searchDebounceTimer.Start();
        }

        /// <summary>
        /// Fired when the user has stopped typing for 300ms.
        /// Now we can safely perform the search.
        /// </summary>
        private void SearchDebounceTimer_Tick(object sender, EventArgs e)
        {
            _searchDebounceTimer.Stop();
            ApplyLocalFilter(_mainMenu.txtSearch.Text);
        }

        // All your existing methods remain unchanged
        // DeleteUser, EditUser, and DgvUser_CellContentClick stay exactly as they were

        private async Task DeleteUser(DataGridViewCellEventArgs e)
        {
            string _id = dgvUser.Rows[e.RowIndex].Cells[0].Value.ToString();
            try
            {
                if (_connection == null || _connection.State == ConnectionState.Closed)
                {
                    await ConnectToTheDatabase();
                }
                using var cm = new SqlCommand("DELETE FROM Users WHERE ID=@ID", _connection);
                cm.Parameters.AddWithValue("@ID", _id);
                await cm.ExecuteNonQueryAsync();
                if (cm != null)
                {
                    Notifications.Information("User has been succesfully deleted!", "Succesful");
                    await Logger.Log(_fullname, $" deleted a user with ID [{_id}]");
                }
                _connection.Close();
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message, "Error occured while deleting user");
                await Logger.Log(_fullname, $" | Error occured in User Panel while running DeleteUser. | Error is: {ex.Message}");
            }
            finally
            {
                await LoadUsers();
            }
        }

        private async Task EditUser(DataGridViewCellEventArgs e)
        {
            UserModule userModule = new(_fullname, _mainMenu);
            userModule.user_ID = Convert.ToInt32(dgvUser.Rows[e.RowIndex].Cells[0].Value);
            userModule.txtFullname.Text = dgvUser.Rows[e.RowIndex].Cells[1].Value.ToString();
            userModule.cmbUsertype.Text = dgvUser.Rows[e.RowIndex].Cells[2].Value.ToString();
            userModule.status = dgvUser.Rows[e.RowIndex].Cells[3].Value.ToString();
            userModule.session = dgvUser.Rows[e.RowIndex].Cells[4].Value.ToString();
            userModule.hostname = dgvUser.Rows[e.RowIndex].Cells[5].Value.ToString();
            userModule.ip_address = dgvUser.Rows[e.RowIndex].Cells[6].Value.ToString();
            userModule.btnUpdate.Visible = true;
            userModule.btnClear.Visible = true;
            userModule.ShowDialog();
            await LoadUsers();
        }

        private async void DgvUser_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                string colName = dgvUser.Columns[e.ColumnIndex].Name;
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    var cellValue = dgvUser.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString();
                    if (!string.IsNullOrEmpty(cellValue))
                    {
                        await EditUser(e);
                    }
                }
                if (colName == "Delete")
                {
                    if (MessageBox.Show("Are you sure you want to delete this user?", "Delete Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        await DeleteUser(e);
                    }
                }
            }
            catch (Exception ex)
            {
                Notifications.Error($"{ex.Message}", "Error occured while editing user");
                await Logger.Log(_fullname, $" | Error occured when editing users in User Panel. | Error is: {ex.Message}");
            }
        }
    }
}