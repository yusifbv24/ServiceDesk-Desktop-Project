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
        private SqlConnection connection { get; set; } = null;
        private async Task CreateDatabaseConnection()
        {
            if (connection == null)
            {
                connection = new SqlConnection(ServicedeskConnection);
            }
            await connection.OpenAsync();
        }
        private async Task UpdateLastActivityAsync(Guid sessionId)
        {
            try
            {
                if (connection == null)
                {
                    await CreateDatabaseConnection();
                }
                string query = "UPDATE UserSessions SET LastActivity = GETDATE() WHERE SessionId = @SessionId";
                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@SessionId", sessionId);
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
                await UpdateLastActivityAsync(sessionId);
                return connection;
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message, "Error connecting to the server");
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
                var connection_inventory = new SqlConnection(Inventory);
                return connection_inventory;
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message,"Error connecting to the inventory server");
                await Logger.Log("System", $" | Error is occured while running EstablishConnectionWithInventory. | Error is: {ex.Message}");
                return null;
            }
        }
        public async Task<SqlConnection> LoginWithoutAuthentication()
        {
            try
            {
                if (!CheckInternetConnection())
                    return null;
                var connection = new SqlConnection(ServicedeskConnection);
                return connection;
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message, "Error connecting to the server without authentication");
                return null;
            }
            finally
            {
                await Task.Delay(1);
            }
        }
    }
}