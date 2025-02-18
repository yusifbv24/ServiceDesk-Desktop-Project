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

namespace ServiceDesk.Forms
{
    public partial class Users : Form
    {
        private readonly Connect connect = Connect.Instance;
        private readonly string _fullname = default;
        private Main _mainMenu;
        private SqlTableDependency<UserTable> _tableDependency_User;
        private SqlTableDependency<UserSessionTable> _tableDependency_Session;
        private SqlConnection connection { get; set; } = null;
        public Users(string fullname, Main mainMenu, out Users users)
        {
            InitializeComponent();
            _fullname = fullname;
            _mainMenu = mainMenu;
            users = this;
            _=LoadUsers();
            StartTableDependency(); // Start listening for table changes
        }
        private async Task CreateConnectionWithDatabase()
        {
            if (connection == null)
            {
                connection = await connect.EstablishConnectionWithServiceDeskAsync(_mainMenu._sessionId).ConfigureAwait(false);
            }
        }
        private string ChangeBooleanToString(bool status)
        {
            return status == true ? "online" : "offline";
        }
        public async Task LoadUsers()
        {
            try
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
                if (connection is null || connection.State == ConnectionState.Closed)
                {
                    await CreateConnectionWithDatabase();
                }
                using var cm = new SqlCommand(query, connection);
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
            if (connection is null || connection.State == ConnectionState.Closed)
            {
                await CreateConnectionWithDatabase();
            }
            using var cm = new SqlCommand("DELETE FROM Users WHERE ID=@ID", connection);
            cm.Parameters.AddWithValue("@ID", _id);
            await cm.ExecuteNonQueryAsync();
            if (cm != null)
            {
                Notifications.Information("User has been succesfully deleted!", "Succesful");
                await Logger.Log(_fullname, $" deleted a user with ID [{_id}]");
            }
            await LoadUsers();
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
            await StopTableDependency();
        }
        private void StartTableDependency()
        {
            Task.Run(async () =>
            {
                await Task.Delay(1000);
                await UserTableDependency();
                await Task.Delay(1000);
                await UserSessionsTableDependency();
            });
        }
        private async Task UserTableDependency()
        {
            if (_tableDependency_User == null)
            {
                using (_tableDependency_User = new SqlTableDependency<UserTable>(connect.ServicedeskConnection, "Users"))
                {
                    _tableDependency_User.OnChanged += TableDependency_User_OnChanged;
                    _tableDependency_User.OnError += TableDependency_OnError;
                    _tableDependency_User.Start();
                }
            }
            else
            {
                _tableDependency_User.OnChanged += TableDependency_User_OnChanged;
                _tableDependency_User.OnError += TableDependency_OnError;
                _tableDependency_User.Start();
            }
            await Task.Delay(1);
        }
        private async Task UserSessionsTableDependency()
        {
            if (_tableDependency_Session == null)
            {
                using (_tableDependency_Session = new SqlTableDependency<UserSessionTable>(connect.ServicedeskConnection, "UserSessions"))
                {
                    _tableDependency_Session.OnChanged += TableDependency_Session_OnChanged;
                    _tableDependency_Session.OnError += TableDependency_OnError;
                    _tableDependency_Session.Start();
                }
            }
            else
            {
                _tableDependency_Session.OnChanged += TableDependency_Session_OnChanged;
                _tableDependency_Session.OnError += TableDependency_OnError;
                _tableDependency_Session.Start();
            }
            await Task.Delay(1);
        }
        private async void TableDependency_User_OnChanged(object sender, RecordChangedEventArgs<UserTable> e)
        {
            if (!this.IsDisposed && this.IsHandleCreated)
            {
                if (e.ChangeType != ChangeType.None)
                {
                    this.BeginInvoke((MethodInvoker)(async () => await LoadUsers()));
                }
                await Task.Delay(1);
            }
        }
        private async void TableDependency_Session_OnChanged(object sender, RecordChangedEventArgs<UserSessionTable> e)
        {
            if (!this.IsDisposed && this.IsHandleCreated)
            {
                if (e.ChangeType != ChangeType.None)
                {
                    this.BeginInvoke((MethodInvoker)(async () => await LoadUsers()));
                }
                await Task.Delay(1);
            }
        }
        private async void TableDependency_OnError(object sender, ErrorEventArgs e)
        {
            await Logger.Log(_fullname, "Error occured while running table dependency in Users");
        }
        private async Task StopTableDependency()
        {
            await SafeStop(_tableDependency_User, "Users");
            await SafeStop(_tableDependency_Session, "UserSessions");
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
                    await Logger.Log(_fullname, $"{name} dependency is already disposed in Users.");
                }
                catch (InvalidOperationException ex)
                {
                    await Logger.Log(_fullname, $"Invalid operation while stopping {name} in Users: {ex.Message}");
                }
                catch (AggregateException ex)
                {
                    await Logger.Log(_fullname, $"AggregateException occurred in {name} in Users: {ex.InnerException?.Message}");
                }
                catch (Exception ex)
                {
                    await Logger.Log(_fullname, $"Error stopping {name} in Users: {ex.Message}");
                }
            }
        }
        #endregion
    }
}
