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
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base.Enums;
using TableDependency.SqlClient.Base.EventArgs;
using static ServiceDesk.Class.TableDependencies;

namespace ServiceDesk.Forms
{
    public partial class Tasks : Form
    {
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private Main _mainMenu;
        private readonly string _fullname = default;
        private SqlTableDependency<TaskTable> _tableDependency;
        private SqlConnection _connection { get; set; } = null;
        private static string _connection_string { get; set; } = null;
        public Tasks(string fullname, Main mainMenu, out Tasks problems)
        {
            InitializeComponent();
            _fullname = fullname;
            _mainMenu = mainMenu;
            problems = this;
            _ = LoadTasks();
            _ = ConnectDependenciesToDatabase();
            StartTableDependency(); // Start listening for table changes
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
        public async Task LoadTasks()
        {
            int i = 0;
            dgvTask.Rows.Clear();
            string query = "SELECT * FROM Tasks";
            // Add search conditions only if searchText is not empty
            if (!string.IsNullOrEmpty(_mainMenu.txtSearch.Text))
            {
                query += @" WHERE task LIKE @searchText";
            }
            query += " ORDER BY task ASC";
            try
            {
                if (_connection == null || _connection.State == ConnectionState.Closed)
                {
                    await ConnectToTheDatabase();
                }
                using var cm = new SqlCommand(query, _connection);
                // Add search parameter only if searchText is not empty
                if (!string.IsNullOrEmpty(_mainMenu.txtSearch.Text))
                {
                    cm.Parameters.AddWithValue("@searchText", "%" + _mainMenu.txtSearch.Text + "%");
                }
                using var dr = await cm.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                while (await dr.ReadAsync())
                {
                    i++;
                    dgvTask.Rows.Add(i, dr["task"].ToString());
                }
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
            finally
            {
                _mainMenu.lblTotalResult.Text = dgvTask.Rows.Count.ToString();
            }
        }
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
        #region SqlTableDependency
        private async void Tasks_FormClosing(object sender, FormClosingEventArgs e)
        {
            await StopTableDependencyAsync();
        }
        private void StartTableDependency()
        {
            var cts = new CancellationTokenSource(); // Create a new CancellationTokenSource
            Task.Run(async () =>
            {
                try
                {
                    await ProblemTableDependency(cts.Token);
                }
                catch (OperationCanceledException ex)
                {
                    Notifications.Error("Table dependencies were canceled.", "Error");
                    await Logger.Log(_fullname, $"Table dependencies were canceled in Task Panel. Error : {ex.Message}");
                }
                catch (Exception ex)
                {
                    await Logger.Log(_fullname, $"An error occurred in Task Panel: {ex.Message}");
                }
            }, cts.Token);
        }
        private async Task ProblemTableDependency(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            // Ensure the previous instance is disposed before creating a new one
            _tableDependency?.Stop();
            _tableDependency?.Dispose();

            if (_tableDependency != null)
            {
                _tableDependency.OnChanged += TableDependency_Task_OnChanged;
                _tableDependency.OnError += TableDependency_OnError;
                _tableDependency.Start();
            }

            await Task.Delay(1000, cancellationToken); // Simulate work
        }
        private async Task ConnectDependenciesToDatabase()
        {
            _connection_string= ConfigurationManager.ConnectionStrings["ServiceDesk"].ConnectionString;
            await Task.Delay(1000);
            _tableDependency ??= new SqlTableDependency<TaskTable>(_connection_string, "Ticket");
        }
        private async Task StopTableDependencyAsync()
        {
            try
            {
                if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
                {
                    _cancellationTokenSource.Cancel();
                }

                // Stop and dispose table dependencies
                _tableDependency?.Stop();
                _tableDependency?.Dispose();
                _tableDependency = null;
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message, "Error occurred while stopping table dependencies");
                await Logger.Log(_fullname, $"Error occurred in Task panel while stopping table dependencies. Error: {ex.Message}");
            }
            finally
            {
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = new CancellationTokenSource();
            }
        }
        private void TableDependency_Task_OnChanged(object sender, RecordChangedEventArgs<TaskTable> e)
        {
            if (!this.IsDisposed && this.IsHandleCreated)
            {
                if (e.ChangeType != TableDependency.SqlClient.Base.Enums.ChangeType.None)
                {
                    // Refresh DataGridView
                    this.BeginInvoke((MethodInvoker)(async () => await LoadTasks()));
                }
            }
        }
        private async void TableDependency_OnError(object sender, ErrorEventArgs e)
        {
            await Logger.Log(_fullname, "Error occured while running table dependency in Tasks");
        }
        #endregion
    }
}