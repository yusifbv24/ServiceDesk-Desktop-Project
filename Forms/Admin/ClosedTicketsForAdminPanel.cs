using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ServiceDesk.Class;

namespace ServiceDesk.Forms
{
    public partial class ClosedTicketsForAdminPanel : Form
    {
        private Main _mainMenu;
        private readonly string _fullname = default;
        private SqlConnection _connection { get; set; } = null;
        // Store the complete dataset in memory
        private DataTable _allTicketsData;

        // Debounce timer to avoid searching on every keystroke
        private System.Windows.Forms.Timer _searchDebounceTimer;

        public ClosedTicketsForAdminPanel(string _fullname, Main mainMenu, out ClosedTicketsForAdminPanel closedTickets)
        {
            InitializeComponent();
            closedTickets = this;
            this._fullname = _fullname;
            _mainMenu = mainMenu;


            _searchDebounceTimer = new System.Windows.Forms.Timer();
            _searchDebounceTimer.Interval = 300; // Wait 300ms after user stops typing
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

        /// <summary>
        /// Loads tickets from the database once and stores them in memory.
        /// This is only called when date filters change or on initial load.
        /// </summary>
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
                _allTicketsData.Columns.Add("creation_date", typeof(string));
                _allTicketsData.Columns.Add("finished_time", typeof(string));
                _allTicketsData.Columns.Add("taken_time", typeof(string));
                _allTicketsData.Columns.Add("fullname", typeof(string));
                _allTicketsData.Columns.Add("rating", typeof(string));
                _allTicketsData.Columns.Add("message", typeof(string));

                // Query WITHOUT the search text filter - we'll filter in memory
                string query = @"SELECT Ticket.*, Rating.rating, Rating.message 
                 FROM Rating 
                 INNER JOIN Ticket WITH (NOLOCK) ON Rating.ID = Ticket.ID 
                 INNER JOIN Status WITH (NOLOCK) ON Status.ID = Ticket.ID 
                 WHERE (Status.status='closed' OR Status.status='resolved') ";

                // Only apply date filters to the database query
                if (!string.IsNullOrEmpty(_mainMenu.fromDate) && !string.IsNullOrEmpty(_mainMenu.toDate))
                {
                    query += " AND (Status.time BETWEEN @fromDate AND @toDate)";
                }

                query += " ORDER BY Status.ID DESC";

                if (_connection == null || _connection.State == ConnectionState.Closed)
                {
                    await ConnectToTheDatabase();
                }

                using SqlCommand cm = new(query, _connection);
                cm.Parameters.AddWithValue("@fromDate", _mainMenu.fromDate);
                cm.Parameters.AddWithValue("@toDate", _mainMenu.toDate);

                using var dr = await cm.ExecuteReaderAsync();
                while (await dr.ReadAsync())
                {
                    // Add to our in-memory DataTable
                    _allTicketsData.Rows.Add(
                        dr["ID"].ToString(),
                        dr["code"].ToString(),
                        dr["dep_name"].ToString(),
                        dr["worker"].ToString(),
                        dr["device"].ToString(),
                        dr["task"].ToString(),
                        dr["solution"].ToString(),
                        dr["creation_date"].ToString(),
                        dr["finished_time"].ToString(),
                        RemoveStringFromTime(dr["taken_time"].ToString()),
                        dr["fullname"].ToString(),
                        dr["rating"].ToString(),
                        dr["message"].ToString()
                    );
                }

                // Now display the data (initially unfiltered)
                ApplyLocalFilter("");
            }
            catch (InvalidOperationException ex)
            {
                await Logger.Log(_fullname, $" | InvalidOperationException in ClosedTicketForAdminPanel while loading tickets| Error is: {ex.Message}");
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message, "Error occured while loading data");
                await Logger.Log(_fullname, $" | Error occured in ClosedTicketForAdmin Panel when loading tickets. | Error is: {ex.Message}");
            }
        }

        /// <summary>
        /// Filters the already-loaded data based on the search text.
        /// This is fast because it works entirely in memory.
        /// </summary>
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
                creation_date LIKE '%{EscapeFilterValue(searchText)}%' OR 
                finished_time LIKE '%{EscapeFilterValue(searchText)}%' OR 
                taken_time LIKE '%{EscapeFilterValue(searchText)}%' OR 
                fullname LIKE '%{EscapeFilterValue(searchText)}%' OR 
                rating LIKE '%{EscapeFilterValue(searchText)}%' OR 
                message LIKE '%{EscapeFilterValue(searchText)}%'";

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
                    row["creation_date"],
                    row["finished_time"],
                    row["taken_time"],
                    row["fullname"],
                    row["rating"],
                    row["message"]
                );
            }

            _mainMenu.lblTotalResult.Text = dgvTicket.Rows.Count.ToString();
        }


        /// <summary>
        /// Escapes special characters in filter values to prevent errors
        /// </summary>
        private string EscapeFilterValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            // Escape single quotes and brackets which have special meaning in DataTable filters
            return value.Replace("'", "''")
                       .Replace("[", "[[]")
                       .Replace("]", "[]]")
                       .Replace("*", "[*]")
                       .Replace("%", "[%]");
        }

        /// <summary>
        /// Called when user changes search text. This triggers the debounce timer
        /// instead of immediately filtering.
        /// </summary>
        public void OnSearchTextChanged()
        {
            // Reset the timer - this prevents searching while user is still typing
            _searchDebounceTimer.Stop();
            _searchDebounceTimer.Start();
        }

        /// <summary>
        /// Timer tick event - user has stopped typing for 300ms, perform the search
        /// </summary>
        private void SearchDebounceTimer_Tick(object sender, EventArgs e)
        {
            _searchDebounceTimer.Stop();
            ApplyLocalFilter(_mainMenu.txtSearch.Text);
        }


        private float ConvertRatingValue(string value)
        {
            if (float.TryParse(value, out float rating))
            {
                return rating;
            }
            else
            {
                return 0; // Default value if parsing fails
            }
        }

        private async Task EditingCell(DataGridViewCellEventArgs e)
        {
            var task = dgvTicket.Rows[e.RowIndex].Cells[5].Value.ToString();

            TicketModule ticktModule = new(_fullname, _mainMenu._userType, _mainMenu);
            ticktModule.ticketID = Convert.ToInt32(dgvTicket.Rows[e.RowIndex].Cells[0].Value);
            ticktModule.txtCode.Text = dgvTicket.Rows[e.RowIndex].Cells[1].Value.ToString();
            ticktModule.txtDep.Text = dgvTicket.Rows[e.RowIndex].Cells[2].Value.ToString();
            ticktModule.txtWorker.Text = dgvTicket.Rows[e.RowIndex].Cells[3].Value.ToString();
            ticktModule.txtDevice.Text = dgvTicket.Rows[e.RowIndex].Cells[4].Value.ToString();
            ticktModule.txtSolution.Text = dgvTicket.Rows[e.RowIndex].Cells[6].Value.ToString();
            if (!string.IsNullOrEmpty(task))
            {
                foreach (var item in task.Split(','))
                {
                    await ticktModule.AddingTasksToTable(item);
                }
            }
            foreach (var item in dgvTicket.Rows[e.RowIndex].Cells[10].Value.ToString().Split(','))
            {
                ticktModule.cmbSelectedUsers.Items.Add(item);
                if (string.IsNullOrEmpty(item))
                {
                    ticktModule.cmbSelectedUsers.Items.Clear();
                }
            }
            ticktModule.btnUpdate.Visible = true;
            ticktModule.btnClose.Visible = true;
            ticktModule.btnClose.Enabled = false;
            ticktModule.btnUpdate.Location = new Point(398, 454);
            await ticktModule.SettingsWhileUpdating();
            ticktModule.ShowDialog();
            await LoadTickets();
        }

        private async Task EvaluateCell(DataGridViewCellEventArgs e)
        {
            string users = dgvTicket.Rows[e.RowIndex].Cells[10].Value.ToString();
            try
            {
                Feedback evaluating = new(_fullname, _mainMenu._sessionId);
                evaluating.ID = dgvTicket.Rows[e.RowIndex].Cells[0].Value.ToString();
                evaluating.Rating.Value = ConvertRatingValue(dgvTicket.Rows[e.RowIndex].Cells[11].Value.ToString());
                evaluating.txtMessage.Text = dgvTicket.Rows[e.RowIndex].Cells[12].Value.ToString();
                evaluating.ShowDialog();
            }
            catch (Exception ex)
            {
                Notifications.Error($"{ex.Message}", "Error occured while evaluating ticket");
                await Logger.Log(_fullname, $" | Error occured in ClosedTicketForAdminPanel while running evaluating in EditClosedTicketsByAdmin. | Error is: {ex.Message}");
            }
            finally
            {
                await LoadTickets();
                await CalculateRatings(users);
            }
        }

        private async Task ResolveCell(DataGridViewCellEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Are you sure you want to resolve this ticket?", "Restore Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string query = @"UPDATE Ticket 
                                            SET finished_time=@finished_time,
                                            taken_time=@taken_time 
                                            WHERE ID=@ID

                                            UPDATE Rating 
                                            SET rating=@rating
                                            WHERE ID=@ID

                                            UPDATE Status 
                                            SET status=@status 
                                            WHERE ID=@ID";
                    var ID = dgvTicket.Rows[e.RowIndex].Cells[0].Value.ToString();
                    if (_connection == null || _connection.State == ConnectionState.Closed)
                    {
                        await ConnectToTheDatabase();
                    }
                    using SqlCommand cm = new(query, _connection);
                    cm.Parameters.AddWithValue("@ID", ID);
                    cm.Parameters.AddWithValue("@finished_time", "");
                    cm.Parameters.AddWithValue("@taken_time", "");
                    cm.Parameters.AddWithValue("@rating", "");
                    cm.Parameters.AddWithValue("@status", "resolving");
                    await cm.ExecuteNonQueryAsync();
                    if (cm != null)
                    {
                        Notifications.Information("You have restored a ticket succesfully", "Succesful");
                        await Logger.Log(_fullname, $" restored a ticket with Ticket_ID [{ID}] from Ticket Table");
                    }
                }
            }
            catch (Exception ex)
            {
                Notifications.Error($"{ex.Message}", "Error occured while resolving ticket");
                await Logger.Log(_fullname, $" | Error occured in ClosedTicketForAdminPanel while running resolving in EditClosedTicketsByAdmin. | Error is: {ex.Message}");
            }
            finally
            {
                _connection.Close();
                await LoadTickets();
            }
        }

        private async Task DeleteCell(DataGridViewCellEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Are you sure you want to delete this ticket?", "Delete Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string query = @"DELETE FROM TICKET WHERE ID=@ID
                                             DELETE FROM Status WHERE ID=@ID
                                             DELETE FROM Rating WHERE ID=@ID";
                    if (_connection == null || _connection.State == ConnectionState.Closed)
                    {
                        await ConnectToTheDatabase();
                    }
                    var ID = dgvTicket.Rows[e.RowIndex].Cells[0].Value.ToString();
                    using SqlCommand cm = new(query, _connection);
                    cm.Parameters.AddWithValue("@ID", ID);
                    cm.ExecuteNonQuery();
                    if (cm != null)
                    {
                        Notifications.Information("Ticket has been deleted succesfully", "Succesful");
                        await Logger.Log(_fullname, $" deleted a ticket with Ticket_ID [{ID}] from Ticket Table");
                    }
                }
            }
            catch (Exception ex)
            {
                Notifications.Error($"{ex.Message}", "Error occured while deleting ticket");
                await Logger.Log(_fullname, $" | Error occured in ClosedTicketForAdminPanel while running deleting in EditClosedTicketsByAdmin. | Error is: {ex.Message}");
            }
            finally
            {
                _connection.Close();
                await LoadTickets();
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
                if (colName == "Evaluate")
                {
                    await EvaluateCell(e);
                    return;
                }
                if (colName == "Resolve")
                {
                    await ResolveCell(e);
                    return;
                }
                if (colName == "Delete")
                {
                    await DeleteCell(e);
                    return;
                }
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    var cellValue = dgvTicket.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString();
                    if (!string.IsNullOrEmpty(cellValue))
                    {
                        await EditingCell(e);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message, "Error occured while editing ticket");
                await Logger.Log(_fullname, $" | Error occured in DgvTicket_CellContentClick when EditClosedTicketsByAdmin items. | Error is: {ex.Message}");
            }
        }


        #region Calculating CSAT for Each user
        private async Task CalculateRatings(string users)
        {
            if (users.Contains(","))
            {
                string[] _userCollection = users.Split(',');
                foreach (string _user in _userCollection)
                {
                    await Ratings.CalculateCSAT(_user,_mainMenu._sessionId);
                }
            }
            else
            {
                await Ratings.CalculateCSAT(users, _mainMenu._sessionId);
            }
        }
        #endregion
    }
}
