using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;

namespace ServiceDesk.Class
{
    public class Logger
    {
        private static readonly string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logfile.txt");
        private static readonly object FileLock = new();
        private static readonly Connect connect = Connect.Instance;
        private static SqlConnection connection { get; set; } = null;
        private static async Task CreateConnectionWithDatabase()
        {
            if (connection == null)
            {
                connection = await connect.LoginWithoutAuthentication().ConfigureAwait(false);
            }
            await Task.Delay(1);
        }
        private static async Task WriteAllLog(string fullname, string message)
        {
            if (connection == null||connection.State==ConnectionState.Closed)
            {
                await CreateConnectionWithDatabase();
            }
            try
            {
                using SqlCommand cm = new("INSERT INTO Log(Time, Logs, fullname) VALUES(@Time, @Logs, @fullname)", connection);
                cm.Parameters.AddWithValue("@Time", DateTime.Now);
                cm.Parameters.AddWithValue("@Logs", message);
                cm.Parameters.AddWithValue("@fullname", fullname);
                await cm.ExecuteNonQueryAsync(); // Use async for non-blocking execution
            }
            catch
            {
                // Log the error and notify the user
                await WriteUserLog("System", "Failed to connect to the database for logging.");
                Notifications.Error("An error occurred while logging data.", "Error log");
            }
        }
        private static async Task WriteUserLog(string fullname, string message)
        {
            try
            {
                // Write the log entry to the file
                var logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | User: {fullname} | {message}";
                lock (FileLock)
                {
                    File.AppendAllText(logFilePath, logEntry+Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                Notifications.Error($"Error happened while logging : {ex.Message}","Log Problem");
            }
            finally
            {
                await Task.Delay(1);
            }
        }
        public static async Task Log(string fullname, string message)
        {
            await WriteAllLog(fullname, message);
            await WriteUserLog(fullname, message);
        }
    }
}
