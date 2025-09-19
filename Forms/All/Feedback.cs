using ServiceDesk.Class;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServiceDesk.Forms
{
    public partial class Feedback : Form
    {
        private readonly string _fullname = default;
        private Guid _sessionId;
        public string ID = default;
        private SqlConnection _connection { get; set; } = null;
        public Feedback(string _fullname,Guid sessionId)
        {
            InitializeComponent();
            this._fullname = _fullname;
            _sessionId = sessionId;
        }
        private async Task ConnectToTheDatabase()
        {
            if (_connection == null)
            {
                _connection = await ConnectionDatabase.ConnectToTheServer(_sessionId);
                await _connection.OpenAsync();
            }
            if (_connection.State == ConnectionState.Closed)
            {
                await _connection.OpenAsync();
            }
        }
        private async Task UpdateDatabaseWithoutRating()
        {
            try
            {
                if (_connection == null || _connection.State == ConnectionState.Closed)
                {
                    await ConnectToTheDatabase();
                }
                using var cm = new SqlCommand("UPDATE Rating SET message=@message WHERE ID=@ID", _connection);
                cm.Parameters.AddWithValue("@ID", ID);
                cm.Parameters.AddWithValue("@message", txtMessage.Text);
                await cm.ExecuteNonQueryAsync();
                _connection.Close();
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message, "Error occured while updating feedback");
                await Logger.Log(_fullname, $" | Error occured in Feedback Panel while running UpdateDatabaseWithoutRating. | Error is: {ex.Message}");
            }
        }
        private async Task UpdateDatabaseWithRatingValue()
        {
            try
            {
                if (_connection == null || _connection.State == ConnectionState.Closed)
                {
                    await ConnectToTheDatabase();
                }
                using var cm = new SqlCommand("UPDATE Rating SET rating=@rating,message=@message WHERE ID=@ID", _connection);
                cm.Parameters.AddWithValue("@ID", ID);
                cm.Parameters.AddWithValue("@rating", Rating.Value);
                cm.Parameters.AddWithValue("@message", txtMessage.Text);
                await cm.ExecuteNonQueryAsync();
                _connection.Close();
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message, "Error occured while updating feedback");
                await Logger.Log(_fullname, $" | Error occured in Feedback Panel while running UpdateDatabaseWithRatingValue. | Error is: {ex.Message}");
            }
        }
        private async void BtnApply_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Are you sure you want to evaluate this ticket", "Evaluating", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (Rating.Value==0)
                    {
                        await UpdateDatabaseWithoutRating();
                    }
                    else
                    {
                        await UpdateDatabaseWithRatingValue();
                    }
                }
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message, "Error occured while apply feedback");
                await Logger.Log(_fullname, $" | Error occured in Feedback when giving rating a ticket with Ticket ID [{ID}]. | Error is: {ex.Message}");
            }
            finally
            {
                this.Close();
            }
        }
        private void BtnExit_Click(object sender, EventArgs e) => this.Dispose();
        private void Feedback_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                BtnApply_Click(this, EventArgs.Empty);  // Trigger the Click event
                e.SuppressKeyPress = true;  // Prevent the default behavior
                e.Handled = true;// Stops the event from being passed to the control
            }
        }
    }
}
