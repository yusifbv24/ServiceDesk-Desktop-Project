using System;
using System.Data.SqlClient;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using ServiceDesk.Class;
using System.Net.NetworkInformation;
using System.Data;
using System.Data.Common;
using System.Collections.ObjectModel;

namespace ServiceDesk.Forms
{
    public partial class LoginTitle : Form
    {
        private readonly string _fullname;
        private readonly string _userType;
        private string _userIpAddress = default;
        private Guid sessionId = Guid.Empty;
        private SqlConnection _connection { get; set; } = null;
        public LoginTitle(string userType, string fullname)
        {
            InitializeComponent();
            this.timer1.Enabled = true;
            _userType = userType;
            _fullname = fullname;
            _ = SearchForSession();
        }
        private async void Timer1_Tick(object sender, EventArgs e)
        {
            if (circle.Value == circle.Maximum)
            {
                timer1.Enabled = false;
                this.Hide();
                await CallMainMethod();
            }
            else
            {
                circle.Value += 3;
            }
        }
        private async Task CallMainMethod()
        {
            Main main = new(_userType, _fullname, sessionId);
            main.btnFullname.Text = _fullname;
            main.Show();
            await UpdateUserSession();
        }
        #region UpdateOnlineSection
        private async Task GetYourIpAddress()
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect("10.0.1.60", 1433);
            var ipEndPoint = socket.LocalEndPoint as IPEndPoint;
            _userIpAddress = ipEndPoint.Address.ToString();
            await Task.Delay(1);
        }
        private async Task ConnectToTheDatabase()
        {
            if (_connection == null)
            {
                _connection = await ConnectionDatabase.ConnectToTheServer();
                await _connection.OpenAsync();
            }
            if (_connection.State == ConnectionState.Closed)
            {
                await _connection.OpenAsync();
            }
        }
        private async Task UpdateUserSession()
        {
            await GetYourIpAddress();
            try
            {
                if (_connection == null || _connection.State == ConnectionState.Closed)
                {
                    await ConnectToTheDatabase();
                }
                string query = @"
                UPDATE UserSessions 
                SET LastActivity = GETDATE(),IsActive=1 
                WHERE SessionId = @SessionId;
                UPDATE Users
                SET ip_address=@ip_address ,session=@session
                WHERE fullname LIKE @fullname";
                using var cm = new SqlCommand(query, _connection);
                cm.Parameters.AddWithValue("@SessionId", sessionId);
                cm.Parameters.AddWithValue("@ip_address", _userIpAddress);
                cm.Parameters.AddWithValue("@session", Dns.GetHostName());
                cm.Parameters.AddWithValue("@fullname", _fullname);
                await cm.ExecuteNonQueryAsync();
                _connection.Close();
                await Logger.Log(_fullname, " logged in to the system with " + _userIpAddress);
            }
            catch (Exception ex)
            {
                Notifications.Error($"{ex.Message}", "Error occured while updating user session");
                await Logger.Log("System", $" | Error is occured when updating user session in LoginTitle Panel. | Error is:  {ex.Message}");
            }
        }
        #endregion
        private async Task CreateNewSession()
        {
            sessionId = Guid.NewGuid();
            try
            {
                if (_connection == null || _connection.State == ConnectionState.Closed)
                {
                    await ConnectToTheDatabase();
                }
                string query = @"INSERT INTO UserSessions (SessionId, UserId, LastActivity, IsActive)
                                VALUES (@SessionId, @UserId, GETDATE(), 1)";
                using var cm = new SqlCommand(query, _connection);
                cm.Parameters.AddWithValue("@SessionId", sessionId);
                cm.Parameters.AddWithValue("@UserId", _fullname);
                await cm.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message, "Error occured while creating new session");
                await Logger.Log(_fullname, $" | Error occured in LoginTitle Panel while running CreateNewSession. | Error is: {ex.Message}");
            }
        }
        private async Task SearchForSession()
        {
            string query = @"SELECT SessionId FROM UserSessions WHERE UserId LIKE @UserId";
            try
            {
                if (_connection == null || _connection.State == ConnectionState.Closed)
                {
                    await ConnectToTheDatabase();
                }
                using var cm = new SqlCommand(query, _connection);
                cm.Parameters.AddWithValue("@UserId", _fullname);
                using var dr = await cm.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                if (await dr.ReadAsync())
                {
                    sessionId = dr.GetGuid(dr.GetOrdinal("SessionId"));
                }
                else 
                {
                    dr.Close();
                    await CreateNewSession();
                }
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message, "Error occured while searching session in LoginTitle");
                await Logger.Log(_fullname, $" | Error occured in LoginTitle Panel while running SearchForSession. | Error is: {ex.Message}");
            }
        }
    }
}
