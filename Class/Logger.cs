using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServiceDesk.Class
{
    public class Logger
    {
        private static readonly string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logfile.txt");
        private static readonly object FileLock = new();
        private static async Task WriteAllLog(string fullname, string message)
        {
            try
            {
                using (var _connection = await ConnectionDatabase.ConnectToTheServer())
                {
                    await _connection.OpenAsync();
                    using SqlCommand cm = new("INSERT INTO Log(Time, Logs, fullname) VALUES(@Time, @Logs, @fullname)", _connection);
                    cm.Parameters.AddWithValue("@Time", DateTime.Now);
                    cm.Parameters.AddWithValue("@Logs", message);
                    cm.Parameters.AddWithValue("@fullname", fullname);
                    await cm.ExecuteNonQueryAsync(); // Use async for non-blocking execution
                }
            }
            catch
            {
                // Log the error and notify the user
                WriteUserLog("System", "Failed to connect to the database for logging.");
                Notifications.Error("An error occurred while logging data.", "Error log");
            }
        }
        private static void WriteUserLog(string fullname, string message)
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
        }
        public static async Task Log(string fullname, string message)
        {
            await WriteAllLog(fullname, message);
            WriteUserLog(fullname, message);
        }
    }
}
