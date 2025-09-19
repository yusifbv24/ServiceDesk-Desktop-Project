using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows.Forms;
using ServiceDesk.Class;
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base.EventArgs;
using TableDependency.SqlClient.Base.Enums;
using static ServiceDesk.Class.TableDependencies;
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
        private SqlTableDependency<UserTable> _tableDependency_User;
        private SqlTableDependency<UserSessionTable> _tableDependency_Session;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private SqlConnection _connection { get; set; } = null;
        private static string _connection_string { get; set; } = null;
        public Users(string fullname, Main mainMenu, out Users users)
        {
            InitializeComponent();
            _fullname = fullname;
            _mainMenu = mainMenu;
            users = this;
            _=LoadUsers();
            _=ConnectDependenciesToDatabase();
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
        private string ChangeBooleanToString(bool status)
        {
            return status == true ? "online" : "offline";
        }
        public async Task LoadUsers()
        {
            dgvUser.Rows.Clear();
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
                            UserSessions ON UserSessions.UserId = Users.fullname;";
            // Add search conditions only if searchText is not empty
            if (!string.IsNullOrEmpty(_mainMenu.txtSearch.Text))
            {
                query += @" WHERE 
                     fullname LIKE @searchText 
                     OR type LIKE @searchText
                     OR session LIKE @searchText
                     OR ip_address LIKE @searchText 
                     OR LastActivity LIKE @searchText";
            }
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
                    cm.Parameters.AddWithValue("@searchText", $"%{_mainMenu.txtSearch.Text}%");
                }
                using var dr = await cm.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                while (await dr.ReadAsync())
                {
                    dgvUser.Rows.Add(
                        dr["ID"].ToString(),
                        dr["fullname"].ToString(),
                        dr["type"].ToString(),
                        ChangeBooleanToString(Convert.ToBoolean(dr["IsActive"])),
                        dr["LastActivity"].ToString(),
                        dr["session"].ToString(),
                        dr["ip_address"].ToString());
                }
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
            finally
            {
                _mainMenu.lblTotalResult.Text = dgvUser.Rows.Count.ToString();
            }
        }
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
        #region SqlTableDependency
        private async void Users_FormClosing(object sender, FormClosingEventArgs e)
        {
            await StopTableDependencyAsync();
        }
        private async Task ConnectDependenciesToDatabase()
        {
            _connection_string= ConfigurationManager.ConnectionStrings["ServiceDesk"].ConnectionString;
            await Task.Delay(1000);
            _tableDependency_User = new SqlTableDependency<UserTable>(_connection_string, "Users");
            await Task.Delay(1000);
            _tableDependency_Session = new SqlTableDependency<UserSessionTable>(_connection_string, "UserSessions");
        }
        private void StartTableDependency()
        {
            var cts = new CancellationTokenSource(); // Create a new CancellationTokenSource
            Task.Run(async () =>
            {
                try
                {
                    await UserSessionsTableDependency(cts.Token);
                    await UserTableDependency(cts.Token);
                }
                catch (OperationCanceledException ex)
                {
                    Notifications.Error("Table dependencies were canceled.", "Error");
                    await Logger.Log(_fullname, $"Table dependencies were canceled in User Panel. Error : {ex.Message}");
                }
                catch (Exception ex)
                {
                    await Logger.Log(_fullname, $"An error occurred in User Panel: {ex.Message}");
                }
            }, cts.Token);
        }
        private async Task UserTableDependency(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            // Ensure the previous instance is disposed before creating a new one
            _tableDependency_User?.Stop();
            _tableDependency_User?.Dispose();

            if (_tableDependency_User != null)
            {
                _tableDependency_User.OnChanged += TableDependency_User_OnChanged;
                _tableDependency_User.OnError += TableDependency_OnError;
                _tableDependency_User.Start();
            }

            await Task.Delay(1000, cancellationToken); // Simulate work
        }
        private async Task UserSessionsTableDependency(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            // Ensure the previous instance is disposed before creating a new one
            _tableDependency_Session?.Stop();
            _tableDependency_Session?.Dispose();

            if (_tableDependency_Session != null)
            {
                _tableDependency_Session.OnChanged += TableDependency_Session_OnChanged;
                _tableDependency_Session.OnError += TableDependency_OnError;
                _tableDependency_Session.Start();
            }

            await Task.Delay(1000, cancellationToken); // Simulate work
        }
        private void TableDependency_User_OnChanged(object sender, RecordChangedEventArgs<UserTable> e)
        {
            if (!this.IsDisposed && this.IsHandleCreated)
            {
                if (e.ChangeType != ChangeType.None)
                {
                    this.BeginInvoke((MethodInvoker)(async () => await LoadUsers()));
                }
            }
        }
        private void TableDependency_Session_OnChanged(object sender, RecordChangedEventArgs<UserSessionTable> e)
        {
            if (!this.IsDisposed && this.IsHandleCreated)
            {
                if (e.ChangeType != ChangeType.None)
                {
                    this.BeginInvoke((MethodInvoker)(async () => await LoadUsers()));
                }
            }
        }
        private async void TableDependency_OnError(object sender, ErrorEventArgs e)
        {
            await Logger.Log(_fullname, "Error occured while running table dependency in Users");
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
                _tableDependency_Session?.Stop();
                _tableDependency_Session?.Dispose();
                _tableDependency_Session = null;

                _tableDependency_User?.Stop();
                _tableDependency_User?.Dispose();
                _tableDependency_User = null;
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message, "Error occurred while stopping table dependencies");
                await Logger.Log(_fullname, $"Error occurred in User Panel while stopping table dependencies. Error: {ex.Message}");
            }
            finally
            {
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = new CancellationTokenSource();
            }
        }
        #endregion
    }
}