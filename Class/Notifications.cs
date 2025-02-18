using System.Windows.Forms;

namespace ServiceDesk.Class
{
    public class Notifications
    {
        public static void Error(string message, string title="Error")
        {
            MessageBox.Show(message, title,MessageBoxButtons.OK,MessageBoxIcon.Error);
        }
        public static void Information(string message, string title = "Information")
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public static void Warning(string message, string title = "Warning")
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
