using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ServiceDesk.Class;

namespace ServiceDesk.Forms
{
    public partial class Tasks : Form
    {
        private Main _mainMenu;
        private readonly string _fullname = default;
        private SqlConnection _connection { get; set; } = null;
        private static string _connection_string { get; set; } = null;

        // This DataTable will hold ALL tasks loaded from the database
        // Think of it as our "master copy" that we filter from
        private DataTable _allTasksData;

        // This timer implements "debouncing" - it waits for the user to stop typing
        // before actually performing the search. This prevents searching on every single keystroke.
        private System.Windows.Forms.Timer _searchDebounceTimer;

        public Tasks(string fullname, Main mainMenu, out Tasks problems)
        {
            InitializeComponent();
            _fullname = fullname;
            _mainMenu = mainMenu;
            problems = this;

            // Initialize our debounce timer
            // The 300ms interval means we wait 300 milliseconds after the user stops typing
            // before we actually perform the search. This feels natural to users and prevents
            // unnecessary processing while they're still typing.
            _searchDebounceTimer = new System.Windows.Forms.Timer();
            _searchDebounceTimer.Interval = 300; // 300 milliseconds = 0.3 seconds
            _searchDebounceTimer.Tick += SearchDebounceTimer_Tick;

            _ = LoadTasks();
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
        /// This method loads ALL tasks from the database and stores them in memory.
        /// Notice that we're NOT filtering by search text in the SQL query anymore.
        /// Instead, we load everything once and filter it in memory using ApplyLocalFilter.
        /// This is much faster and prevents the duplicate row issues you were experiencing.
        /// </summary>
        public async Task LoadTasks()
        {
            int i = 0;
            dgvTask.Rows.Clear();

            // Initialize our DataTable structure if this is the first time loading
            // We define the columns that match what we're pulling from the database
            if (_allTasksData == null)
            {
                _allTasksData = new DataTable();
                _allTasksData.Columns.Add("RowNumber", typeof(int)); // For the row counter
                _allTasksData.Columns.Add("task", typeof(string));   // The actual task name
            }
            else
            {
                // If we're reloading, clear the existing data
                _allTasksData.Clear();
            }

            // Notice: NO search filter in this query!
            // We load ALL tasks and filter them in memory later
            string query = "SELECT * FROM Tasks ORDER BY task ASC";

            try
            {
                if (_connection == null || _connection.State == ConnectionState.Closed)
                {
                    await ConnectToTheDatabase();
                }

                using var cm = new SqlCommand(query, _connection);
                using var dr = await cm.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                // Store every task in our in-memory DataTable
                while (await dr.ReadAsync())
                {
                    i++;
                    _allTasksData.Rows.Add(i, dr["task"].ToString());
                }

                // Now display all the data (unfiltered initially)
                // If there's already text in the search box, this will filter it
                ApplyLocalFilter(_mainMenu.txtSearch.Text);
            }
            catch (InvalidOperationException ex)
            {
                await Logger.Log(_fullname, $" | InvalidOperationException in TaskPanel while loading tasks| Error is: {ex.Message}");
            }
            catch (Exception ex)
            {
                Notifications.Error($"{ex.Message}", "Error occured while loading tasks");
                await Logger.Log(_fullname, $" | Error is occured when loading tasks in Tasks Panel. | Error is: {ex.Message}");
            }
        }

        /// <summary>
        /// This method filters the data that's already loaded in memory.
        /// It searches through _allTasksData and only displays rows that match the search text.
        /// This is incredibly fast because we're not touching the database at all.
        /// </summary>
        private void ApplyLocalFilter(string searchText)
        {
            // Safety check - make sure we have data to filter
            if (_allTasksData == null || _allTasksData.Rows.Count == 0)
            {
                _mainMenu.lblTotalResult.Text = "0";
                return;
            }

            // Clear the visible grid - we're about to repopulate it with filtered results
            dgvTask.Rows.Clear();

            DataRow[] filteredRows;

            if (string.IsNullOrWhiteSpace(searchText))
            {
                // No search text means show everything
                filteredRows = _allTasksData.Select();
            }
            else
            {
                // Build a filter expression that searches in the task column
                // The LIKE operator works just like in SQL - % means "any characters"
                string filterExpression = $"task LIKE '%{EscapeFilterValue(searchText)}%'";

                // DataTable.Select() is very fast - it searches through the in-memory data
                filteredRows = _allTasksData.Select(filterExpression);
            }

            // Add each filtered row to the visible grid
            foreach (var row in filteredRows)
            {
                dgvTask.Rows.Add(row["RowNumber"], row["task"]);
            }

            // Update the result count that the user sees
            _mainMenu.lblTotalResult.Text = dgvTask.Rows.Count.ToString();
        }

        /// <summary>
        /// This helper method "escapes" special characters in the search text.
        /// Some characters like single quotes and brackets have special meaning in DataTable filters.
        /// If we don't escape them, the filter will fail with an error.
        /// For example, if someone searches for "test's", the single quote needs to be doubled to "test''s"
        /// </summary>
        private string EscapeFilterValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            // Replace special characters with their escaped versions
            return value.Replace("'", "''")      // Single quotes must be doubled
                       .Replace("[", "[[]")      // Opening brackets
                       .Replace("]", "[]]")      // Closing brackets  
                       .Replace("*", "[*]")      // Asterisks
                       .Replace("%", "[%]");     // Percent signs
        }

        /// <summary>
        /// This is the method that Main.cs will call when the search text changes.
        /// Instead of immediately filtering (which would happen on every keystroke),
        /// we start/restart the debounce timer. This creates a small delay that feels
        /// natural to users and dramatically reduces unnecessary processing.
        /// </summary>
        public void OnSearchTextChanged()
        {
            // Stop the timer if it's already running
            // This resets the countdown back to 300ms
            _searchDebounceTimer.Stop();

            // Start the timer again
            // If the user keeps typing, this will keep getting reset
            // The search only happens when they pause for 300ms
            _searchDebounceTimer.Start();
        }

        /// <summary>
        /// This event fires when the debounce timer completes its countdown.
        /// It means the user has stopped typing for 300ms, so now we can safely
        /// perform the search without worrying about doing it too frequently.
        /// </summary>
        private void SearchDebounceTimer_Tick(object sender, EventArgs e)
        {
            // Stop the timer - we only want to search once
            _searchDebounceTimer.Stop();

            // Now perform the actual filtering on the in-memory data
            ApplyLocalFilter(_mainMenu.txtSearch.Text);
        }

        // Rest of your existing code remains exactly the same...
        // LoadID, EditingCell, DeleteCell, DgvTasks_CellContentClick all stay unchanged

        private async Task<int> LoadID(string text)
        {
            var _ID = 0;
            try
            {
                if (_connection == null || _connection.State == ConnectionState.Closed)
                {
                    await ConnectToTheDatabase();
                }
                using var cm = new SqlCommand("SELECT ID FROM Tasks WHERE task LIKE @task", _connection);
                cm.Parameters.AddWithValue("@task", text);
                using var dr = await cm.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                if (await dr.ReadAsync())
                {
                    _ID = Convert.ToInt32(dr["ID"]);
                }
                return _ID;
            }
            catch (Exception ex)
            {
                Notifications.Error($"{ex.Message}", "Error occured while loading task id");
                await Logger.Log(_fullname, $" | Error is occured when loading ID in Tasks Panel. | Error is: {ex.Message}");
                return 0;
            }
        }

        private async Task EditingCell(DataGridViewCellEventArgs e)
        {
            TaskModule problemModule = new(_fullname, _mainMenu);
            problemModule.problemID = LoadID(dgvTask.Rows[e.RowIndex].Cells[1].Value.ToString()).ToString();
            problemModule.txtTask.Text = dgvTask.Rows[e.RowIndex].Cells[1].Value.ToString();

            problemModule.StartPosition = FormStartPosition.CenterScreen;
            problemModule.btnUpdate.Visible = true;
            problemModule.btnClear.Visible = true;
            problemModule.ShowDialog();
            await LoadTasks();
        }

        private async Task DeleteCell(DataGridViewCellEventArgs e)
        {
            int ID = await LoadID(dgvTask.Rows[e.RowIndex].Cells[1].Value.ToString());
            try
            {
                if (_connection == null || _connection.State == ConnectionState.Closed)
                {
                    await ConnectToTheDatabase();
                }
                using var cm = new SqlCommand("DELETE FROM Tasks WHERE ID LIKE @ID ", _connection);
                cm.Parameters.AddWithValue("@ID", ID);
                await cm.ExecuteNonQueryAsync();
                if (cm != null)
                {
                    Notifications.Information("Task has been successfully deleted!");
                    await Logger.Log(_fullname, $" deleted a task with Task_Name [{ID}] from Tasks Table");
                    await LoadTasks();
                }
                _connection.Close();
            }
            catch (Exception ex)
            {
                Notifications.Error($"{ex.Message}", "Error occured while deleting task");
                await Logger.Log(_fullname, $" | Error is occured when running DeleteCell in Tasks Panel. | Error is: {ex.Message}");
            }
        }

        private async void DgvTasks_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                string colName = dgvTask.Columns[e.ColumnIndex].Name;
                if (colName == "Delete")
                {
                    if (MessageBox.Show("Are you sure you want to delete this task?", "Delete Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        await DeleteCell(e);
                    }
                    return;
                }
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    var cellValue = dgvTask.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString();
                    if (!string.IsNullOrEmpty(cellValue))
                    {
                        await EditingCell(e);
                    }
                    return;
                }
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message, "Error occured while editing tasks");
                await Logger.Log(_fullname, $" | Error is occured when editing tasks. | Error is: {ex.Message}");
            }
        }
    }
}