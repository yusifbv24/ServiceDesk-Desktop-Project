using ServiceDesk.Class;
using ServiceDesk.Forms;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServiceDesk
{
    internal static class Program
    {
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private static readonly string MutexName = "ServiceDesk";
        // Get the user's current date format
        static string shortDateFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
        private static readonly Connect connect = Connect.Instance;
        private static string latestversion = default;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string currentVersion = GetCurrentVersion();
            latestversion = GetLatestVersionAsync();
            // Compare versions
            if (currentVersion != latestversion)
            {
                MessageBox.Show($"A new version ({latestversion}) is available! Please update your software.",
                                "Update Available", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            using var mutex = new Mutex(true, MutexName, out bool isNewInstance);
            // Check if it matches "dd.MM.yyyy"
            if (shortDateFormat != "dd.MM.yyyy")
            {
                MessageBox.Show($"Your date format is incorrect! Please change it to dd.MM.yyyy. Current format: {shortDateFormat}",
                                "Date Format Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Exit the application if the format is wrong
            }
            if (isNewInstance)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                LoginSession.SearchingSession();
                Application.Run();
            }
            else
            {
                // Bring existing application to foreground
                IntPtr hWnd = FindWindow(null, "Main"); // Replace "MainForm" with your main form's title
                if (hWnd != IntPtr.Zero)
                {
                    SetForegroundWindow(hWnd);
                }
                else
                {
                    Notifications.Warning("Another instance of this application is already running");
                }
            }
        }
        static string GetCurrentVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
        static string GetLatestVersionAsync()
        {
            string version = string.Empty;
            using var connection = new SqlConnection(connect.ServicedeskConnection);
            try
            {
                connection.Open();
                string query = "SELECT version FROM Settings";
                using var cm = new SqlCommand(query, connection);
                using var dr = cm.ExecuteReader(CommandBehavior.CloseConnection);
                while (dr.Read())
                {
                    version = dr["version"].ToString();
                }
                return version;
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message, "Error occured while getting latest version");
                return GetCurrentVersion();
            }
            finally
            {
                connection.Close();
            }
        }
        private class LoginSession : CredentialManager
        {
            private static void CallLoginForm()
            {
                var Login = new Login();
                Login.Show();
            }
            private static void GettingValidation(string userType, string hostname, string username)
            {
                using var connection = new SqlConnection(connect.ServicedeskConnection);
                try
                {
                    connection.Open();
                    using var cm = new SqlCommand("SELECT ID FROM Users WHERE type=@type AND session=@session AND fullname LIKE @fullname ", connection);
                    cm.Parameters.AddWithValue("@type", userType);
                    cm.Parameters.AddWithValue("@session", hostname);
                    cm.Parameters.AddWithValue("@fullname", username);
                    using var dr = cm.ExecuteReader(CommandBehavior.CloseConnection);
                    if (dr.HasRows)
                    {
                        var loginTitle = new LoginTitle(userType, username);
                        loginTitle.Show();
                    }
                    else
                        CallLoginForm();
                }
                catch (Exception ex)
                {
                    Notifications.Error(ex.Message, "Error occured while getting validation");
                }
                finally
                {
                    connection.Close();
                }
            }
            public static async void SearchingSession()
            {
                try
                {
                    var (username, userType, hostname) = CredentialManager.GetSavedCredentials();
                    if (username != null && userType != null && hostname != null)
                    {
                        GettingValidation(userType, hostname, username);
                    }
                    else
                    {
                        CallLoginForm();
                    }
                }
                catch (FormatException)
                {
                    CallLoginForm();
                }
                catch (Exception ex)
                {
                    Notifications.Error($"{ex.Message}", "Error occured while searching session");
                    await Logger.Log("System", $" |  Error occured in Program Class while running SearchingSession. | Error is: {ex.Message}");
                }
            }
        }
    }
}