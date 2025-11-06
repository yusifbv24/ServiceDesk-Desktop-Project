using ServiceDesk.Class;
using ServiceDesk.Dto;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServiceDesk.Forms
{
    public partial class FindTickets : Form
    {
        private readonly string _fullname = default;
        private Main _mainMenu;
        private SqlConnection _connection { get; set; } = null;
        private ProductServiceClient _productServiceClient;

        // Store all ticket data in memory for fast filtering
        private DataTable _allTicketsData;

        // Debounce timer for the text search box
        private System.Windows.Forms.Timer _searchDebounceTimer;

        public FindTickets(string fullname, Main mainMenu, out FindTickets searchByDepartment)
        {
            InitializeComponent();
            searchByDepartment = this;
            _fullname = fullname;
            _mainMenu = mainMenu;
            _productServiceClient = new ProductServiceClient();

            // Initialize debounce timer for text search
            _searchDebounceTimer = new System.Windows.Forms.Timer();
            _searchDebounceTimer.Interval = 300;
            _searchDebounceTimer.Tick += SearchDebounceTimer_Tick;

            _ = LoadDefaultSettings();
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

        /// <summary>
        /// Loads initial data - populates the department and user dropdown lists,
        /// then loads all tickets from the database.
        /// </summary>
        public async Task LoadDefaultSettings()
        {
            await LoadDepartments();
            await LoadUsers();
            await LoadAllTickets();
        }

        /// <summary>
        /// Loads all departments from the Product Service API into the dropdown.
        /// This only needs to happen once when the form loads.
        /// </summary>
        private async Task LoadDepartments()
        {
            try
            {
                var departments = await _productServiceClient.GetDepartmentsAsync();

                cmbDepartmentSearch.Items.Clear();
                cmbDepartmentSearch.Items.Add("All Departments"); // Add an "all" option

                if (departments != null && departments.Count > 0)
                {
                    foreach (var dept in departments)
                    {
                        cmbDepartmentSearch.Items.Add(dept.Name);
                    }
                }

                cmbDepartmentSearch.SelectedIndex = 0; // Select "All Departments" by default
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message, "Error loading departments");
                await Logger.Log(_fullname, $" | Error occurred when loading departments in FindTickets Panel. | Error is: {ex.Message}");
            }
        }

        /// <summary>
        /// Loads all users into the user dropdown.
        /// This only needs to happen once when the form loads.
        /// </summary>
        private async Task LoadUsers()
        {
            try
            {
                if (_connection == null || _connection.State == ConnectionState.Closed)
                {
                    await ConnectToTheDatabase();
                }

                cmbUserSearch.Items.Clear();
                cmbUserSearch.Items.Add("All Users"); // Add an "all" option

                using var cm = new SqlCommand("SELECT fullname FROM Users WHERE type='User' ORDER BY fullname", _connection);
                using var dr = await cm.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                while (await dr.ReadAsync())
                {
                    cmbUserSearch.Items.Add(dr["fullname"].ToString());
                }

                cmbUserSearch.SelectedIndex = 0; // Select "All Users" by default
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message, "Error loading users");
                await Logger.Log(_fullname, $" | Error occurred when loading users in FindTickets Panel. | Error is: {ex.Message}");
            }
        }

        /// <summary>
        /// Loads ALL tickets from the database and stores them in memory.
        /// We load everything - all users, all departments, all tickets.
        /// Then we filter this data in memory based on the search criteria.
        /// This is much faster than querying the database every time a filter changes.
        /// </summary>
        private async Task LoadAllTickets()
        {
            dgvTicket.Rows.Clear();

            // Initialize our DataTable structure if this is the first load
            if (_allTicketsData == null)
            {
                _allTicketsData = new DataTable();
                _allTicketsData.Columns.Add("ID", typeof(string));
                _allTicketsData.Columns.Add("code", typeof(string));
                _allTicketsData.Columns.Add("worker", typeof(string));
                _allTicketsData.Columns.Add("device", typeof(string));
                _allTicketsData.Columns.Add("task", typeof(string));
                _allTicketsData.Columns.Add("solution", typeof(string));
                _allTicketsData.Columns.Add("creation_date", typeof(string));
                _allTicketsData.Columns.Add("taken_time", typeof(string));
                _allTicketsData.Columns.Add("finished_time", typeof(string));
                _allTicketsData.Columns.Add("dep_name", typeof(string));
                _allTicketsData.Columns.Add("fullname", typeof(string));
            }
            else
            {
                _allTicketsData.Clear();
            }

            // Load ALL tickets without any filters
            // We'll filter them in memory based on user selections
            string query = @"
                SELECT 
                    Ticket.ID,
                    Ticket.code,
                    Ticket.worker,
                    Ticket.device,
                    Ticket.task,
                    Ticket.solution,
                    Ticket.creation_date,
                    Ticket.taken_time,
                    Ticket.finished_time,
                    Ticket.dep_name,
                    Ticket.fullname
                FROM Ticket
                INNER JOIN Status WITH (NOLOCK) ON Status.ID = Ticket.ID
                WHERE Status.status = 'closed' ";

            // Only apply date filters to the database query
            if (!string.IsNullOrEmpty(_mainMenu.fromDate) && !string.IsNullOrEmpty(_mainMenu.toDate))
            {
                query += " AND (Status.time BETWEEN @fromDate AND @toDate)";
            }

            query += " ORDER BY Ticket.finished_time DESC";

            try
            {
                if (_connection == null || _connection.State == ConnectionState.Closed)
                {
                    await ConnectToTheDatabase();
                }

                using var cm = new SqlCommand(query, _connection);
                cm.Parameters.AddWithValue("@fromDate", _mainMenu.fromDate);
                cm.Parameters.AddWithValue("@toDate", _mainMenu.toDate);

                using var dr = await cm.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                // Store every ticket in our in-memory DataTable
                while (await dr.ReadAsync())
                {
                    _allTicketsData.Rows.Add(
                        dr["ID"].ToString(),
                        dr["code"].ToString(),
                        dr["worker"].ToString(),
                        dr["device"].ToString(),
                        dr["task"].ToString(),
                        dr["solution"].ToString(),
                        dr["creation_date"].ToString(),
                        dr["taken_time"].ToString(),
                        dr["finished_time"].ToString(),
                        dr["dep_name"].ToString(),
                        dr["fullname"].ToString()
                    );
                }

                // Apply any active filters to display the data
                ApplyAllFilters();
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message, "Error loading tickets");
                await Logger.Log(_fullname, $" | Error occurred when loading tickets in FindTickets Panel. | Error is: {ex.Message}");
            }
        }

        /// <summary>
        /// This is the key method that applies ALL filters at once.
        /// It considers:
        /// 1. The text search box (inventory code search)
        /// 2. The selected user from the dropdown
        /// 3. The selected department from the dropdown
        /// All of this happens in memory, so it's very fast.
        /// </summary>
        private void ApplyAllFilters()
        {
            if (_allTicketsData == null || _allTicketsData.Rows.Count == 0)
            {
                _mainMenu.lblTotalResult.Text = "0";
                return;
            }

            dgvTicket.Rows.Clear();

            // Get the current filter criteria
            string searchText = txtSearchCode.Text?.Trim() ?? "";
            string selectedUser = cmbUserSearch.SelectedItem?.ToString() ?? "All Users";
            string selectedDepartment = cmbDepartmentSearch.SelectedItem?.ToString() ?? "All Departments";

            // Build a filter expression that combines all three criteria
            List<string> filterConditions = new List<string>();

            // Add text search filter if there's text in the search box
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                // Search across multiple columns - inventory code is the primary focus,
                // but we also search in other fields for better user experience
                filterConditions.Add($@"(
                    code LIKE '%{EscapeFilterValue(searchText)}%' OR 
                    worker LIKE '%{EscapeFilterValue(searchText)}%' OR 
                    device LIKE '%{EscapeFilterValue(searchText)}%' OR 
                    task LIKE '%{EscapeFilterValue(searchText)}%' OR 
                    solution LIKE '%{EscapeFilterValue(searchText)}%'
                )");
            }

            // Add user filter if a specific user is selected
            if (selectedUser != "All Users" && !string.IsNullOrEmpty(selectedUser))
            {
                // The fullname column might contain multiple users separated by commas
                // so we use LIKE to match if the selected user appears anywhere in the string
                filterConditions.Add($"fullname LIKE '%{EscapeFilterValue(selectedUser)}%'");
            }

            // Add department filter if a specific department is selected
            if (selectedDepartment != "All Departments" && !string.IsNullOrEmpty(selectedDepartment))
            {
                filterConditions.Add($"dep_name LIKE '%{EscapeFilterValue(selectedDepartment)}%'");
            }

            // Combine all conditions with AND (all conditions must be true)
            string finalFilter = string.Join(" AND ", filterConditions);

            // Apply the filter to get matching rows
            DataRow[] filteredRows;
            if (string.IsNullOrWhiteSpace(finalFilter))
            {
                // No filters active - show everything
                filteredRows = _allTicketsData.Select();
            }
            else
            {
                // Apply the combined filter
                filteredRows = _allTicketsData.Select(finalFilter);
            }

            // Populate the grid with the filtered results
            foreach (var row in filteredRows)
            {
                dgvTicket.Rows.Add(
                    row["ID"],
                    row["code"],
                    row["worker"],
                    row["device"],
                    row["task"],
                    row["solution"],
                    row["creation_date"],
                    row["taken_time"],
                    row["finished_time"],
                    row["dep_name"],
                    row["fullname"]
                );
            }

            // Update the result count display
            _mainMenu.lblTotalResult.Text = dgvTicket.Rows.Count.ToString();
        }

        /// <summary>
        /// Escapes special characters in filter values
        /// </summary>
        private string EscapeFilterValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;
            return value.Replace("'", "''").Replace("[", "[[]").Replace("]", "[]]");
        }

        /// <summary>
        /// Called by Main.cs when the search text changes.
        /// Triggers the debounce timer for the text search.
        /// </summary>
        public void OnSearchTextChanged()
        {
            _searchDebounceTimer.Stop();
            _searchDebounceTimer.Start();
        }

        /// <summary>
        /// Fired when the user stops typing for 300ms.
        /// Applies all filters including the text search.
        /// </summary>
        private void SearchDebounceTimer_Tick(object sender, EventArgs e)
        {
            _searchDebounceTimer.Stop();
            ApplyAllFilters();
        }

        // Event handlers for the inventory code search textbox
        private void TxtSearchCode_TextChanged(object sender, EventArgs e)
        {
            // Use the debounce timer for this text box too
            OnSearchTextChanged();
        }

        // Event handlers for the user dropdown
        private void CmbUserSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            // When user selection changes, immediately refilter the data
            // No debounce needed here since it's a dropdown selection, not typing
            ApplyAllFilters();
        }

        private void CmbUserSearch_Enter(object sender, EventArgs e)
        {
            if (cmbUserSearch.Text == "Select a user")
            {
                cmbUserSearch.Text = "";
            }
        }

        private void CmbUserSearch_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cmbUserSearch.Text))
            {
                cmbUserSearch.Text = "Select a user";
            }
        }

        // Event handlers for the department dropdown
        private void CmbDepartmentSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            // When department selection changes, immediately refilter the data
            ApplyAllFilters();
        }

        private void CmbDepartmentSearch_Enter(object sender, EventArgs e)
        {
            if (cmbDepartmentSearch.Text == "Select a department")
            {
                cmbDepartmentSearch.Text = "";
            }
        }

        private void CmbDepartmentSearch_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cmbDepartmentSearch.Text))
            {
                cmbDepartmentSearch.Text = "Select a department";
            }
        }
    }
}