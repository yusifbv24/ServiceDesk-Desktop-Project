using ServiceDesk.Class;
using System;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;
using System.Data.Common;
using System.Collections.ObjectModel;
using System.Threading;

namespace ServiceDesk.Forms
{
    public partial class TaskModule : Form
    {
        private readonly string _fullname = default;
        public string problemID = default;
        private Main _mainMenu;
        private SqlConnection _connection { get; set; } = null;
        public TaskModule(string fullname, Main mainMenu)
        {
            InitializeComponent();
            _fullname = fullname;
            this.KeyPreview = true;
            _mainMenu = mainMenu;
        }
        private async Task ConnectToTheDatabase()
        {
            if (_connection == null)
            {
                _connection = await ConnectionDatabase.ConnectToTheServer(_mainMenu._sessionId);
                await _connection.OpenAsync();
            }
            if (_connection.State == ConnectionState.Closed)
            {
                await _connection.OpenAsync();
            }
        }
        private void BtnExit_Click(object sender, EventArgs e) => this.Dispose();
        private void BtnClear_Click(object sender, EventArgs e) => txtTask.Text = string.Empty;
        private async Task SaveTask()
        {
            if (string.IsNullOrWhiteSpace(txtTask.Text))
            {
                Notifications.Warning("Please fill!");
                return;
            }
            if (MessageBox.Show("Are you sure you want to save this task?", "Saving Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    if (_connection == null || _connection.State == ConnectionState.Closed)
                    {
                        await ConnectToTheDatabase();
                    }
                    using var cm = new SqlCommand("INSERT INTO Tasks(task)VALUES(@task)", _connection);
                    cm.Parameters.AddWithValue("@task", txtTask.Text);
                    await cm.ExecuteNonQueryAsync();
                    if (cm != null)
                    {
                        Notifications.Information("Task has been successfully saved");
                        await Logger.Log(_fullname, $" added a new task with Task_Name [{txtTask.Text}] to the Task Table");
                    }
                    _connection.Close();
                }
                catch (Exception ex)
                {
                    Notifications.Error(ex.Message, "Error saving a task");
                    await Logger.Log(_fullname, $" Error occured in Task Table when saving a task with Task_name [{txtTask.Text}] | Error: {ex.Message}");
                }
                finally
                {
                    this.Close();
                }
            }
        }
        private async Task UpdateTask()
        {
            if (string.IsNullOrEmpty(txtTask.Text))
            {
                Notifications.Warning("Please fill");
                return;
            }
            if (MessageBox.Show("Are you sure you want to update this task?", "Update Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    if (_connection == null || _connection.State == ConnectionState.Closed)
                    {
                        await ConnectToTheDatabase();
                    }
                    using var cm = new SqlCommand("UPDATE Tasks SET task = @task WHERE ID=@ID", _connection);
                    cm.Parameters.AddWithValue("@ID", Convert.ToInt32(problemID));
                    cm.Parameters.AddWithValue("@task", txtTask.Text);
                    await cm.ExecuteNonQueryAsync();
                    if (cm != null)
                    {
                        Notifications.Information("Task has been successfully updated!");
                        await Logger.Log(_fullname, $" updated a new task with Task_Name [{txtTask.Text} to the Task Table");
                    }
                    _connection.Close();
                }
                catch (Exception ex)
                {
                    Notifications.Error($"{ex.Message}", "Error updating a task");
                    await Logger.Log(_fullname, $" Error occured in Task Table when updating a task with Task Name [{txtTask.Text}] | Error: {ex.Message}");
                }
                finally
                {
                    this.Close();
                }
            }
        }
        private void BtnSave_Click(object sender, EventArgs e) => _ = SaveTask();
        private void BtnUpdate_Click(object sender, EventArgs e) => _ = UpdateTask();
        #region Design
        private void CategoryModule_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }
        private void ProblemModule_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (btnSave.Visible != false)
                {
                    BtnSave_Click(this, EventArgs.Empty);  // Trigger the Click event
                    e.SuppressKeyPress = true;  // Prevent the default behavior
                    e.Handled = true;// Stops the event from being passed to the control
                }
                else
                {
                    BtnUpdate_Click(this, EventArgs.Empty);  // Trigger the Click event
                    e.SuppressKeyPress = true;  // Prevent the default behavior
                    e.Handled = true;// Stops the event from being passed to the control
                }
            }
        }
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);
        #endregion
    }
}
