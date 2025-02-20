using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServiceDesk.Class
{
    public static class ConnectionDatabase
    {
        private static string inventory_string => ConfigurationManager.ConnectionStrings["Inventory"].ConnectionString;
        private static string servicedesk_string => ConfigurationManager.ConnectionStrings["ServiceDesk"].ConnectionString;
        public static async Task<SqlConnection> ConnectToTheServer(Guid sessionId=default)
        {
            if (!CheckInternetConnection())
            {
                return null;
            }
            if (sessionId != Guid.Empty)
            {
                await UpdateLastActivityAsync(sessionId);
            }
            return new SqlConnection(servicedesk_string);
        }
        public static SqlConnection ConnectToTheInventoryServer()
        {
            if (!CheckInternetConnection())
            {
                return null;
            }
            return new SqlConnection(inventory_string);
        }
        private static async Task UpdateLastActivityAsync(Guid sessionId)
        {
            var _connection = new SqlConnection(servicedesk_string);
            try
            {
                await _connection.OpenAsync();
                string query = "UPDATE UserSessions SET LastActivity = GETDATE() WHERE SessionId = @SessionId";
                using var command = new SqlCommand(query, _connection);
                command.Parameters.AddWithValue("@SessionId", sessionId);
                await command.ExecuteNonQueryAsync();
                _connection.Close();
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message,"Error occured while updating user activity");
                await Logger.Log("System", $" | Error is occured while running UpdateLastActivity in ConnectionDatabase class. | Error is: {ex.Message}");
            }
        }
        private static bool PingToTheServer(string serverAddress, int retries = 3)
        {
            for (int attempt = 0; attempt < retries; attempt++)
            {
                using var ping = new Ping();
                var reply = ping.Send(serverAddress, 3000);
                if (reply.Status == IPStatus.Success)
                    return true;
            }
            return false;
        }
        public static bool CheckInternetConnection()
        {
            if (!PingToTheServer("10.0.1.60"))
            {
                PromptToRestoreConnection();
                return false;
            }
            return true;
        }
        private static void PromptToRestoreConnection()
        {
            var result = MessageBox.Show(
                "Internet connection is lost. Do you want to restore it?",
                "Connection Lost",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                Notifications.Information("Please check your internet settings.", "Restore Connection");
                CheckInternetConnection();
            }
            else
            {
                Notifications.Warning("Some features may not work without an internet connection.");
                Application.Exit();
            }
        }
    }
}