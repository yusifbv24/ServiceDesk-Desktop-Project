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
        public Users(string fullname, Main mainMenu, out Users users)
        {
            InitializeComponent();
            _fullname = fullname;
            _mainMenu = mainMenu;
            users = this;
            _=LoadUsers();
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
    }
}