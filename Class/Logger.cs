using System;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;

namespace ServiceDesk.Class
{
    public class Logger
    {
        private static readonly string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logfile.txt");
        private static readonly object FileLock = new();
        private static async Task WriteAllLog(string fullname, string message)
        {
            var connect = Connect.Instance;
            using var connection = connect.LoginToTheServerAsync();
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
            if (connection is null) return;
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
