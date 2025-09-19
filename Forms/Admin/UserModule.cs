using ServiceDesk.Class;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServiceDesk.Forms
{
    public partial class UserModule : Form
    {
        public string status = default;
        public string session = default;
        public string hostname = default;
        public string ip_address = default;
        public int user_ID = default;
        private readonly string _fullname = default;
        private readonly string key = "c24ca5898a4fjwidjwi2bdsn235a1916";
        private Main _mainMenu;
        private SqlConnection _connection { get; set; } = null;
        private string User_password(string password)
        {
            return Cryptography.EncryptString(key, password);
        }
        public UserModule(string fullname, Main mainMenu)
        {
            InitializeComponent();
            this._fullname = fullname;
            this.KeyPreview = true;
            _mainMenu = mainMenu;
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
        private async Task AddingNewUser()
        {
            try
            {
                if (_connection == null || _connection.State == ConnectionState.Closed)
                {
                    await ConnectToTheDatabase();
                }
                using var cm = new SqlCommand(@"INSERT INTO 
                        Users(fullname,password,type,session,ip_address,csat)
                        VALUES(@fullname,@password,@type,@session,@ip_address,@csat) 
                        INSERT INTO UserSessions (SessionId, UserId, LastActivity, IsActive)
                        VALUES (@SessionId, @UserId, GETDATE(), 0) ", _connection);
                cm.Parameters.AddWithValue("@fullname", txtFullname.Text);
                cm.Parameters.AddWithValue("@password", User_password(txtPassword.Text));
                cm.Parameters.AddWithValue("@type", cmbUsertype.Text);
                cm.Parameters.AddWithValue("@session", "");
                cm.Parameters.AddWithValue("@ip_address", "");
                cm.Parameters.AddWithValue("@csat", 0);
                cm.Parameters.AddWithValue("@SessionId", Guid.NewGuid());
                cm.Parameters.AddWithValue("@UserId", txtFullname.Text);

                await cm.ExecuteNonQueryAsync();
                if (cm != null)
                {
                    Notifications.Information("User has been successfully created");
                    await Logger.Log(txtFullname.Text, $" added a new user with Fullname [{txtFullname.Text}], Password [{User_password(txtPassword.Text)}], User_Type [{cmbUsertype.Text}] to the User Table");
                }
                _connection.Close();
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message, "Error occured while adding new user");
                await Logger.Log(_fullname, $" | Error occured in UserModule Panel while running AddingNewUser. | Error is: {ex.Message}");
            }
        }
        private async Task UpdateUser()
        {
            try
            {
                if (_connection == null || _connection.State == ConnectionState.Closed)
                {
                    await ConnectToTheDatabase();
                }
                using var cm = new SqlCommand("UPDATE Users SET fullname=@fullname,password=@password,type=@type,session=@session,ip_address=@ip_address WHERE ID=@ID", _connection);
                cm.Parameters.AddWithValue("@ID", user_ID);
                cm.Parameters.AddWithValue("@fullname", txtFullname.Text);
                cm.Parameters.AddWithValue("@password", User_password(txtPassword.Text));
                cm.Parameters.AddWithValue("@type", cmbUsertype.Text);
                cm.Parameters.AddWithValue("@session", hostname);
                cm.Parameters.AddWithValue("@ip_address", ip_address);
                await cm.ExecuteNonQueryAsync();
                if (cm != null)
                {
                    Notifications.Information("User has been successfully updated");
                    await Logger.Log(txtFullname.Text, $" updated a user with Fullname [{txtFullname.Text}], Password [{User_password(txtPassword.Text)}], User_Type [{cmbUsertype.Text}] to the User Table");
                }
                _connection.Close();
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message, "Error occured while updating user");
                await Logger.Log(_fullname, $" | Error occured in UserModule Panel while running UpdateUser. | Error is: {ex.Message}");
            }
        }
        private async Task UpdateUserWithoutChangingPassword()
        {
            try
            {
                if (_connection == null || _connection.State == ConnectionState.Closed)
                {
                    await ConnectToTheDatabase();
                }
                using var cm = new SqlCommand("UPDATE Users SET fullname=@fullname,type=@type WHERE ID=@ID", _connection);
                cm.Parameters.AddWithValue("@fullname", txtFullname.Text);
                cm.Parameters.AddWithValue("@type", cmbUsertype.Text);
                cm.Parameters.AddWithValue("@ID", user_ID);
                await cm.ExecuteNonQueryAsync();
                if (cm != null)
                {
                    Notifications.Information("User has been successfully updated");
                    await Logger.Log(txtFullname.Text, $" updated a user with Fullname [{txtFullname.Text}], Password [{User_password(txtPassword.Text)}], User_Type [{cmbUsertype.Text}] to the User Table");
                }
                _connection.Close();
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message, "Error occured while updating user");
                await Logger.Log(_fullname, $" | Error occured in UserModule Panel while running UpdateUserWithoutChangingPassword. | Error is: {ex.Message}");
            }
        }
        private async void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtFullname.Text) || string.IsNullOrEmpty(txtPassword.Text) || string.IsNullOrEmpty(txtConfirmPass.Text) || string.IsNullOrEmpty(cmbUsertype.Text))
                {
                    Notifications.Warning("Please fill");
                    return;
                }
                if (txtPassword.Text != txtConfirmPass.Text)
                {
                    Notifications.Warning("Password did not match");
                    return;
                }
                if (MessageBox.Show("Are you sure you want to save this user?", "Saving Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    await AddingNewUser();
                }
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message, "Error occured while saving new user");
                await Logger.Log(_fullname, $" | Error occured when saving a new user with Fullname [{txtFullname.Text}] in UserModule Panel. | Error is: {ex.Message}");
            }
            finally
            {
                this.Close();
            }
        }
        private async void BtnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtFullname.Text) || string.IsNullOrEmpty(txtPassword.Text) || string.IsNullOrEmpty(txtConfirmPass.Text))
                {
                    Notifications.Warning("Please fill");
                    return;
                }
                if ((txtPassword.Text != "Password" && txtConfirmPass.Text != "Confirm Password") && txtPassword.Text != txtConfirmPass.Text)
                {
                    Notifications.Warning("Password did not match!");
                    return;
                }
                if (txtPassword.Text == "Password" || txtConfirmPass.Text == "Confirm Password")
                    await UpdateUserWithoutChangingPassword();
                else
                    await UpdateUser();
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message, "Error occured while updating users");
                await Logger.Log(_fullname, $" | Error occured when updating a user with Fullname [{txtFullname.Text}]. | Error is: {ex.Message}");
            }
            finally
            {
                this.Close();
            }
        }
        private void BtnExit_Click(object sender, EventArgs e) => this.Dispose();
        private void BtnClear_Click(object sender, EventArgs e)
        {
            txtFullname.Text = string.Empty;
            txtPassword.Text = string.Empty;
            cmbUsertype.Text = string.Empty;
            txtConfirmPass.Text = string.Empty;
        }
        #region Design
        private void UserModule_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (btnSave.Visible != false)
                {
                    BtnSave_Click(this, EventArgs.Empty);  // Trigger the Click event
                    e.SuppressKeyPress = true;  // Prevent the default behavior
                    e.Handled = true;// Stops the event from being passed to the control
                }
                else
                {
                    BtnUpdate_Click(this, EventArgs.Empty);  // Trigger the Click event
                    e.SuppressKeyPress = true;  // Prevent the default behavior
                    e.Handled = true;// Stops the event from being passed to the control
                }
            }
        }
        private void TxtPassword_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtPassword.Text))
            {
                txtPassword.Text = "Password";
                txtPassword.UseSystemPasswordChar = false;
            }
            else
                txtPassword.UseSystemPasswordChar = true;
        }
        private void TxtPassword_Enter(object sender, EventArgs e)
        {
            if (txtPassword.Text == "Password")
            {
                txtPassword.Text = "";
                txtPassword.UseSystemPasswordChar = true;
            }
            else
                txtPassword.UseSystemPasswordChar = true;
        }
        private void TxtConfirmPass_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtConfirmPass.Text))
            {
                txtConfirmPass.Text = "Confirm Password";
                txtConfirmPass.UseSystemPasswordChar = false;
            }
            else
                txtConfirmPass.UseSystemPasswordChar = true;
        }
        private void TxtConfirmPass_Enter(object sender, EventArgs e)
        {
            if (txtConfirmPass.Text == "Confirm Password")
            {
                txtConfirmPass.Text = "";
                txtConfirmPass.UseSystemPasswordChar = true;
            }
            else
                txtConfirmPass.UseSystemPasswordChar = true;
        }
        private void CmbUsertype_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cmbUsertype.Text))
            {
                cmbUsertype.Text = "User Type";
            }
        }
        private void CmbUsertype_Enter(object sender, EventArgs e)
        {
            if (cmbUsertype.Text == "User Type")
            {
                cmbUsertype.Text = "";
            }
        }
        private void UserModule_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);
        #endregion
    }
}