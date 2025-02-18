using System;
using System.Data;
using System.Data.SqlClient;
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
        private readonly Connect connect = Connect.Instance;
        private Main _mainMenu;
        private readonly string _fullname = default;
        private SqlTableDependency<TaskTable> _tableDependency;
        private SqlConnection connection { get; set; } = null;
        public Tasks(string fullname, Main mainMenu, out Tasks problems)
        {
            InitializeComponent();
            _fullname = fullname;
            _mainMenu = mainMenu;
            problems = this;
            _ = LoadTasks();
            StartTableDependency(); // Start listening for table changes
        }
        private async Task CreateConnectionWithDatabase()
        {
            if (connection == null)
            {
                connection = await connect.EstablishConnectionWithServiceDeskAsync(_mainMenu._sessionId).ConfigureAwait(false);
            }
            await connection.OpenAsync();
        }
        public async Task LoadTasks()
        {
            try
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
                if (connection is null || connection.State == ConnectionState.Closed)
                {
                    await CreateConnectionWithDatabase();
                }
                using var cm = new SqlCommand(query, connection);
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
            try
            {
                var _ID = 0;
                if (connection is null || connection.State == ConnectionState.Closed)
                {
                    await CreateConnectionWithDatabase();
                }
                using var cm = new SqlCommand("SELECT ID FROM Tasks WHERE task LIKE @task", connection);
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
            if (connection is null || connection.State == ConnectionState.Closed)
            {
                await CreateConnectionWithDatabase();
            }
            using var cm = new SqlCommand("DELETE FROM Tasks WHERE ID LIKE @ID ", connection);
            cm.Parameters.AddWithValue("@ID", ID);
            await cm.ExecuteNonQueryAsync();
            if (cm != null)
            {
                Notifications.Information("Task has been successfully deleted!");
                await Logger.Log(_fullname, $" deleted a task with Task_Name [{ID}] from Tasks Table");
                await LoadTasks();
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
            await StopTableDependency();
        }
        private void StartTableDependency()
        {
            Task.Run(async () =>
            {
                await Task.Delay(1000);
                await ProblemTableDependency();
            });
        }
        private async Task ProblemTableDependency()
        {
            if (_tableDependency == null)
            {
                using (_tableDependency = new SqlTableDependency<TaskTable>(connect.ServicedeskConnection, "Ticket"))
                {
                    _tableDependency.OnChanged += TableDependency_Task_OnChanged;
                    _tableDependency.OnError += TableDependency_OnError;
                    _tableDependency.Start();
                }
            }
            else
            {
                _tableDependency.OnChanged += TableDependency_Task_OnChanged;
                _tableDependency.OnError += TableDependency_OnError;
                _tableDependency.Start();
            }
            await Task.Delay(1);
        }
        private async Task StopTableDependency()
        {
            await SafeStop(_tableDependency, "Task");
        }
        private async Task SafeStop<T>(SqlTableDependency<T> dependency, string name) where T : class, new()
        {
            if (dependency != null)
            {
                try
                {
                    dependency.Stop();
                    dependency.Dispose();
                }
                catch (ObjectDisposedException)
                {
                    await Logger.Log(_fullname, $"{name} dependency is already disposed in Task.");
                }
                catch (InvalidOperationException ex)
                {
                    await Logger.Log(_fullname, $"Invalid operation while stopping {name} in Task: {ex.Message}");
                }
                catch (AggregateException ex)
                {
                    await Logger.Log(_fullname, $"AggregateException occurred in {name} in Task: {ex.InnerException?.Message}");
                }
                catch (Exception ex)
                {
                    await Logger.Log(_fullname, $"Error stopping {name} in Task: {ex.Message}");
                }
            }
        }
        private async void TableDependency_Task_OnChanged(object sender, RecordChangedEventArgs<TaskTable> e)
        {
            if (!this.IsDisposed && this.IsHandleCreated)
            {
                if (e.ChangeType != TableDependency.SqlClient.Base.Enums.ChangeType.None)
                {
                    // Refresh DataGridView
                    this.BeginInvoke((MethodInvoker)(async () => await LoadTasks()));
                }
                await Task.Delay(1);
            }
        }
        private async void TableDependency_OnError(object sender, ErrorEventArgs e)
        {
            await Logger.Log(_fullname, "Error occured while running table dependency in Tasks");
        }
        #endregion
    }
}