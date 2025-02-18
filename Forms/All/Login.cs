using ServiceDesk.Class;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServiceDesk.Forms
{
    public partial class Login : Form
    {
        private class AutoLogin : CredentialManager
        {
            public static void SaveUserInformation(string username, string userType, string hostname)
            {
                SaveCredentials(username, userType, hostname);
            }
        }
        private readonly Connect connect = Connect.Instance;
        private string _userType;
        private string _fullName;
        private string _haveSession;
        private readonly string key = "c24ca5898a4fjwidjwi2bdsn235a1916";
        private SqlConnection connection { get; set; } = null;

        public Login()
        {
            InitializeComponent();
            _ = LoadFullname();
        }
        private async Task CreateConnectionWithDatabase()
        {
            connection = await connect.LoginWithoutAuthentication().ConfigureAwait(false);
            await connection.OpenAsync();
        }
        private async Task LoadFullname()
        {
            try
            {
                txtUsername.Items.Clear();
                if (connection is null || connection.State == ConnectionState.Closed)
                {
                    await CreateConnectionWithDatabase();
                }
                using var cm = new SqlCommand("SELECT fullname FROM Users", connection);
                using var dr = await cm.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                while (await dr.ReadAsync())
                {
                    txtUsername.Items.Add(dr["fullname"].ToString());
                }
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message, "Error occured while loading fullname");
                await Logger.Log("System", $" | Error is occured when loading fullname in Login Panel. | Error is: {ex.Message}");
            }
        }
        private async Task CheckSession()
        {
            try
            {
                if (connection is null || connection.State == ConnectionState.Closed)
                {
                    await CreateConnectionWithDatabase();
                }
                using var cm = new SqlCommand("SELECT session FROM Users WHERE fullname LIKE @fullname", connection);
                cm.Parameters.AddWithValue("@fullname", txtUsername.Text);
                using SqlDataReader dr = await cm.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                while (await dr.ReadAsync())
                {
                    _haveSession = dr["session"].ToString();
                    if (!dr.HasRows)
                    {
                        return;
                    }
                    break;
                }
                if (_haveSession == Dns.GetHostName())
                {
                    _haveSession = string.Empty;
                }
            }
            catch (Exception ex)
            {
                Notifications.Error($"{ex.Message}", "Error occured while checking session");
                await Logger.Log("System", $" | Error is occured when checking session. | Error is: {ex.Message}");
            }
        }
        private void CallLoginTitle()
        {
            LoginTitle loginForm = new (_userType, _fullName);
            this.Hide();
            loginForm.Show();
        }
        private async void BtnLogin_Click(object sender, EventArgs e)
        {
            await CheckSession();
            var user_password = Cryptography.EncryptString(key, txtPassword.Text);
            if (connection is null || connection.State == ConnectionState.Closed)
            {
                await CreateConnectionWithDatabase();
            }
            using (var cm = new SqlCommand("SELECT type FROM Users WHERE fullname LIKE @fullname AND password = @password", connection))
            {
                cm.Parameters.AddWithValue("@fullname", txtUsername.Text);
                cm.Parameters.AddWithValue("@password", user_password);

                using var dr = await cm.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                if (await dr.ReadAsync()) // Move to the first row
                {
                    _fullName = txtUsername.Text;
                    _userType = dr["type"].ToString();
                }
                else
                {
                    MessageBox.Show("Invalid username or password!", "ACCESS DENIED", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await Logger.Log("System", $" Fullname: [{txtUsername.Text}] attempted to login and failed.");
                    return;
                }
            }
            // Handle active session
            if (!string.IsNullOrEmpty(_haveSession))
            {
                MessageBox.Show($"You are logged in on another computer with this hostname: {_haveSession}", "User Active", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // Call additional login actions
            CallLoginTitle();
            // Save login details for auto-login
            AutoLogin.SaveUserInformation(txtUsername.Text, _userType, Dns.GetHostName());
        }
        private void BtnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        private void BtnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void Login_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
        #region Design
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);
        private void Login_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }
        private void Login_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                BtnLogin_Click(sender, EventArgs.Empty);  // Trigger the Click event
                e.SuppressKeyPress = true;  // Prevent the default behavior
                e.Handled = true;// Stops the event from being passed to the control
            }
        }
        private void TxtUsername_Enter(object sender, EventArgs e)
        {
            if (txtUsername.Text == "Fullname")
            {
                txtUsername.Text = "";
            }
            Fullname_seperator.FillColor = Color.FromArgb(94, 148, 255);
            Fullname_seperator.FillThickness = 2;
        }
        private void TxtUsername_Leave(object sender, EventArgs e)
        {
            if (txtUsername.Text == "")
            {
                txtUsername.Text = "Fullname";
            }
            Fullname_seperator.FillColor = Color.FromArgb(193, 200, 207);
            Fullname_seperator.FillThickness = 1;
        }
        private void TxtPassword_Leave(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = true;
            if (txtPassword.Text == "")
            {
                txtPassword.UseSystemPasswordChar = false;
            }
        }
        private void TxtPassword_Enter(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = true;
            if (txtPassword.Text == "Password")
            {
                txtPassword.Text = "";
            }
        }
        #endregion
    }
}
