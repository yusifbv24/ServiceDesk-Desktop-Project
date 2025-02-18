using System;
using System.Data.SqlClient;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using System.Configuration;
using System.Threading.Tasks;
using System.Data;

namespace ServiceDesk.Class
{
    internal class Connect
    {
        private static Connect _instance;
        public static Connect Instance => _instance ??= new Connect();
        private Connect() { }
        private string Inventory => ConfigurationManager.ConnectionStrings["Inventory"].ConnectionString;
        public string ServicedeskConnection => ConfigurationManager.ConnectionStrings["ServiceDesk"].ConnectionString;
        private async Task UpdateLastActivityAsync(Guid sessionId,SqlConnection connection)
        {
            try
            {
                string query = "UPDATE UserSessions SET LastActivity = GETDATE() WHERE SessionId = @SessionId";
                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@SessionId", sessionId);
                if (connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync();
                }
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message);
                await Logger.Log("System", $" | Error is occured while running UpdateLastActivity in Connect class. | Error is: {ex.Message}");
            }
        }
        private bool PingToTheServer(string serverAddress, int retries = 3)
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
        public bool CheckInternetConnection()
        {
            if (!PingToTheServer("10.0.1.60"))
            {
                PromptToRestoreConnection();
                return false;
            }
            return true;
        }
        private void PromptToRestoreConnection()
        {
            var result = MessageBox.Show(
                "Internet connection is lost. Do you want to restore it?",
                "Connection Lost",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                Notifications.Information("Please check your internet settings.","Restore Connection");
                CheckInternetConnection();
            }
            else
            {
                Notifications.Warning("Some features may not work without an internet connection.");
                Application.Exit();
            }
        }
        public async Task<SqlConnection> EstablishConnectionWithServiceDeskAsync(Guid sessionId)
        {
            try
            {
                if (!CheckInternetConnection())
                {
                    return null;
                }
                SqlConnection connection =new SqlConnection(ServicedeskConnection);
                await UpdateLastActivityAsync(sessionId,connection);
                return connection;
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message);
                await Logger.Log("System", $" | Error occurred while running EstablishConnectionWithServiceDesk. | Error: {ex.Message}");
                return null;
            }
        }
        public async Task<SqlConnection> EstablishConnectionWithInventoryAsync()
        {
            if (!CheckInternetConnection())
                return null;

            try
            {
                var connection = new SqlConnection(Inventory);
                if (connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync();
                }
                return connection;
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message);
                await Logger.Log("System", $" | Error is occured while running EstablishConnectionWithInventory. | Error is: {ex.Message}");
                return null;
            }
        }
        public SqlConnection LoginToTheServerAsync()
        {
            try
            {
                if (!CheckInternetConnection())
                    return null;
                var connection = new SqlConnection(ServicedeskConnection);
                connection.Open(); // Async call to prevent UI freezing
                return connection;
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message);
                return null;
            }
        }
    }
}