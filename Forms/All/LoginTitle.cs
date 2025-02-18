using System;
using System.Data.SqlClient;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using ServiceDesk.Class;
using System.Net.NetworkInformation;
using System.Data;

namespace ServiceDesk.Forms
{
    public partial class LoginTitle : Form
    {
        private readonly string _fullname;
        private readonly string _userType;
        private string _userIpAddress = default;
        private readonly Connect connect = Connect.Instance;
        private Guid sessionId = Guid.Empty;
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
                circle.Value +=3;
            }
        }
        private async Task CallMainMethod()
        {
            Main main = new(_userType, _fullname,sessionId);
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
        private async Task UpdateUserSession()
        {
            try
            {
                await GetYourIpAddress();
                using var connection = connect.LoginToTheServerAsync();
                string query = @"
                UPDATE UserSessions 
                SET LastActivity = GETDATE(),IsActive=1 
                WHERE SessionId = @SessionId;
                UPDATE Users
                SET ip_address=@ip_address ,session=@session
                WHERE fullname LIKE @fullname";
                if (connection is null) return;
                using var cm = new SqlCommand(query, connection);
                cm.Parameters.AddWithValue("@SessionId", sessionId);
                cm.Parameters.AddWithValue("@ip_address", _userIpAddress);
                cm.Parameters.AddWithValue("@session", Dns.GetHostName());
                cm.Parameters.AddWithValue("@fullname", _fullname);
                await cm.ExecuteNonQueryAsync();
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
            using var connection = connect.LoginToTheServerAsync();
            string query = @"
            INSERT INTO UserSessions (SessionId, UserId, LastActivity, IsActive)
            VALUES (@SessionId, @UserId, GETDATE(), 1)";
            if (connection is null) return;
            using var cm = new SqlCommand(query, connection);
            cm.Parameters.AddWithValue("@SessionId", sessionId);
            cm.Parameters.AddWithValue("@UserId", _fullname);
            await cm.ExecuteNonQueryAsync();
        }
        private async Task SearchForSession()
        {
            var connection= connect.LoginToTheServerAsync();
            string query = @"SELECT SessionId FROM UserSessions WHERE UserId LIKE @UserId";
            if (connection is null) return;
            using var cm = new SqlCommand(query, connection);
            cm.Parameters.AddWithValue("@UserId", _fullname);
            using var dr = await cm.ExecuteReaderAsync(CommandBehavior.CloseConnection);
            if (!dr.HasRows)
            {
                await CreateNewSession();
                return;
            }
            while (await dr.ReadAsync())
            {
                sessionId = dr.GetGuid(dr.GetOrdinal("SessionId"));
            }
        }
    }
}
