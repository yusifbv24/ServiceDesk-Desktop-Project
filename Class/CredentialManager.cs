using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ServiceDesk.Class
{
    internal class CredentialManager
    {
        private static readonly Connect connect = Connect.Instance;
        private static SqlConnection connection { get; set; } = null;
        private static readonly string _credentialFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "user_credentials.dat");
        protected static void SaveCredentials(string username, string userType, string hostname)
        {
            var encryptedData = EncryptData($"{username}:{userType}:{hostname}");
            File.WriteAllText(_credentialFilePath, encryptedData);
        }
        protected static (string username, string userType, string hostname) GetSavedCredentials()
        {
            if (!File.Exists(_credentialFilePath)) return (null, null, null);

            var encryptedData = File.ReadAllText(_credentialFilePath);
            var decryptedData = DecryptData(encryptedData);

            if (string.IsNullOrEmpty(encryptedData)) return (null, null, null);

            var parts = decryptedData.Split(':');
            return (parts[0], parts[1], parts[2]);
        }
        public static void RemoveCredentials(string fullname)
        {
            File.Delete(_credentialFilePath);
            RemoveSession(fullname);
        }
        private static async Task CreateConnectionWithDatabase()
        {
            if (connection == null)
            {
                connection = await connect.LoginWithoutAuthentication().ConfigureAwait(false);
            }
            await connection.OpenAsync();
        }
        public static async void RemoveSession(string fullname)
        {
            if (connection is null || connection.State == ConnectionState.Closed)
            {
                await CreateConnectionWithDatabase();
            }
            string query = @"DELETE FROM UserSessions WHERE UserId LIKE @UserId";

            using var cm = new SqlCommand(query, connection);
            cm.Parameters.AddWithValue("@UserId", fullname);

            await cm.ExecuteNonQueryAsync(); // Use the async version for non-blocking execution
        }
        private static string EncryptData(string data)
        {
            // Implement a secure encryption method, e.g., using AES
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(data));
        }
        private static string DecryptData(string encryptedData)
        {
            // Implement decryption to retrieve original data
            return Encoding.UTF8.GetString(Convert.FromBase64String(encryptedData));
        }
    }
}
