using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Web.UI.WebControls.WebParts;
using System.Windows.Forms;
using ServiceDesk.Class;
using ServiceDesk.Properties;

namespace ServiceDesk.Forms
{
    public partial class TicketModule : Form
    {
        private readonly Connect connect = Connect.Instance;
        private readonly string _userType = default;
        private readonly string _fullname = default;
        public int ticketID = default;
        public string timeElapsed = default;
        private bool _IsValidCode = true;
        private bool _IsValidDepartment = true;
        private bool _isNotTodayTask = false;
        private string _tasks;
        private string _users;
        private Main _mainMenu;
        public TicketModule(string _fullName,string userType, Main mainMenu)
        {
            InitializeComponent();
            this._fullname = _fullName;
            this._userType = userType;
            _mainMenu = mainMenu;
            LoadInformation();
            LoadDefaultSettings();
            guna2ShadowForm.SetShadowForm(this);
        }
        #region LoadSomeInformation
        public void LoadDefaultSettings()
        {
            bool isAdmin = _userType == "Admin";
            UserPic.Visible = isAdmin;
            cmbUsers.Visible = isAdmin;
            cmbSelectedUsers.Visible = isAdmin;
            btnDeleteUsers.Visible = isAdmin;
            if (_userType == "User"&& cmbSelectedUsers.Items.Count == 0)
            {
                cmbSelectedUsers.Items.Add(_fullname);
            }
            if (txtDep.Text!="Department" && !string.IsNullOrWhiteSpace(txtDep.Text)&&string.IsNullOrWhiteSpace(txtCode.Text)&& txtCode.PlaceholderText=="Inventory code")
            {
                IfNoInventoryCode.Checked = true;
            }
            date.Value= DateTime.Now;
        }
        private async void LoadInformation()
        {
            await LoadTasks();
            await LoadDepartments();
            await LoadUsers();
        }
        private async Task LoadTasks()
        {
            try
            {
                cmbTask.Items.Clear();
                using var connection = await connect.EstablishConnectionWithServiceDeskAsync(_mainMenu._sessionId);
                if (connection is null) return;
                using var cm = new SqlCommand("SELECT task FROM Tasks", connection);
                using var dr = await cm.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                while (await dr.ReadAsync())
                {
                    cmbTask.Items.Add(dr["task"].ToString());
                }
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message, "Error loading tasks");
                await Logger.Log(_fullname, $" | Error is occured when loading tasks in Ticket Module Panel. | Error is: {ex.Message}");
            }
        }
        private async Task LoadDepartments()
        {
            try
            {
                txtDep.Items.Clear();
                using var connection = await connect.EstablishConnectionWithInventoryAsync();
                if (connection is null) return;
                using var cm = new SqlCommand("SELECT dname FROM Department", connection);
                using var dr = await cm.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                while (await dr.ReadAsync())
                {
                    txtDep.Items.Add(dr["dname"].ToString());
                }
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message,"Error loading departments");
                await Logger.Log(_fullname, $" | Error is occured when loading departments in Ticket Module Panel. | Error is: {ex.Message}");
            }
        }
        private async Task LoadUsers()
        {
            try
            {
                cmbUsers.Items.Clear();
                using var connection = await connect.EstablishConnectionWithServiceDeskAsync(_mainMenu._sessionId);
                if (connection is null) return;
                using var cm = new SqlCommand("SELECT fullname FROM Users WHERE type='User'", connection);
                using var dr = await cm.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                while (await dr.ReadAsync())
                {
                    cmbUsers.Items.Add(dr["fullname"].ToString());
                }
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message,"Error loading users");
                await Logger.Log(_fullname, $" | Error is occured when loading users in Ticket Module Panel. | Error is: {ex.Message}");
            }
        }
        #endregion
        #region Ticket ID
        private async Task FindTicketID()
        {
            try
            {
                string query = "SELECT MAX(ID) AS MaxID FROM Ticket";
                using var connection = await connect.EstablishConnectionWithServiceDeskAsync(_mainMenu._sessionId);
                if (connection is null) return;
                using var cm = new SqlCommand(query, connection);
                var result = await cm.ExecuteScalarAsync();
                ticketID = result != DBNull.Value && result != null ? Convert.ToInt32(result) + 1 : 1;
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message,"Error finding ticket ID");
                await Logger.Log(_fullname, $" | Error occured in SearchNewTicketID in TicketModule Panel. | Error is: {ex.Message}");
            }
        }
        #endregion
        #region Settings
        public async Task SettingsWhileUpdating()
        {
            UserPic.Location = new Point(398, 293);
            cmbUsers.Location=new Point(433, 293);
            cmbSelectedUsers.Location=new Point(398, 342);
            btnDeleteUsers.Location = new Point(660, 342);
            await Task.Delay(1);
        }
        public async Task HideTxtSolution()
        {
            txtSolution.Visible = false;
            cmbUsers.Location = new Point(433, 174);
            UserPic.Location = new Point(398, 174);
            cmbSelectedUsers.Location = new Point(398, 224);
            btnDeleteUsers.Location = new Point(660, 224);
            await Task.Delay(1);
        }
        public async Task ShowNotTodayAction()
        {
            txtSolution.Visible = true;
            txtSolution.Location = new Point(398, 174);
            UserPic.Location = new Point(398, 333);
            cmbUsers.Location = new Point(433, 333);
            cmbSelectedUsers.Location = new Point(398, 382);
            btnDeleteUsers.Location = new Point(660, 382);
            await Task.Delay(1);
        }
        private string CalculateTime(DateTime OpenedTime)
        {
            var closedTime = DateTime.Now;
            TimeSpan _calculatedTime = closedTime - OpenedTime;
            // If the time difference is less than 60 seconds
            if (_calculatedTime.TotalSeconds < 60)
            {
                return $"{Math.Floor(_calculatedTime.TotalSeconds)} s";
            }
            // If the time difference is less than 60 minutes
            else if (_calculatedTime.TotalMinutes < 60)
            {
                return $"{Math.Floor(_calculatedTime.TotalMinutes)} m";
            }
            // If the time difference is less than 24 hours
            else if (_calculatedTime.TotalHours < 24)
            {
                return $"{Math.Floor(_calculatedTime.TotalHours)} h";
            }
            // If the time difference is less than 30 days
            else if (_calculatedTime.TotalDays < 30)
            {
                return $"{Math.Floor(_calculatedTime.TotalDays)} d";
            }
            else
            {
                // For longer periods, show the actual date
                return $"on {OpenedTime:dd/MM/yyyy}";
            }
        }
        private async Task DefaultButtonSettings()
        {
            if (txtDep.Text == "Department") 
                txtDep.Text = "";
            if (txtWorker.Text == "Worker")
                txtWorker.Text = "";

            _tasks = CollectTasks();
            _users = await JoinStringInComboBox(cmbSelectedUsers);
        }
        private async void NotToday_CheckStateChanged(object sender, EventArgs e)
        {
            if (!NotToday.Checked)
            {
                _isNotTodayTask = false;
                date.Visible = false;
                btnSave.Visible = true;
                btnClose.Visible = false;
                await HideTxtSolution();
            }
            else
            {
                _isNotTodayTask = true;
                date.Visible = true;
                btnSave.Visible = false;
                btnClose.Visible = true;
                await ShowNotTodayAction();
            }
        }
        #endregion
        #region Checking Inventory Code
        private async Task CheckInventoryCodeSituation()
        {
            if (_IsValidCode)
            {
                CheckInventoryCode.Visible=true;
                CheckInventoryCode.Image = Resources.check;
            }
            else
            {
                CheckInventoryCode.Image = Resources.exit_hover;
            }
            await Task.Delay(1);
        }
        private async Task InventoryCodeCondition()
        {
            try
            {
                using var connection = await connect.EstablishConnectionWithInventoryAsync();
                if (connection is null) return;
                using var cm = new SqlCommand("SELECT prodCode,pdepartment,pworker,pcategory FROM Product WHERE prodCode=@prodCode1 OR prodCode=@prodCode2 OR prodCode=@prodCode3", connection);
                cm.Parameters.AddWithValue("@prodCode1", $"{txtCode.Text}");
                cm.Parameters.AddWithValue("@prodCode2", $"*{txtCode.Text}");
                cm.Parameters.AddWithValue("@prodCode3", $"#{txtCode.Text}");
                using var dr = await cm.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                if (!dr.HasRows)
                {
                    _IsValidCode = false;
                    CheckInventoryCode.Visible = true;
                    await CheckInventoryCodeSituation();
                    return;
                }
                while (await dr.ReadAsync())
                {
                    txtCode.Text = dr["prodCode"].ToString();
                    txtDep.Text = dr["pdepartment"].ToString();
                    txtWorker.Text = dr["pworker"].ToString();
                    txtDevice.Text = dr["pcategory"].ToString();
                }
                txtDevice.Enabled = false;
                txtDep.Enabled = false;
                txtWorker.Enabled = false;
                _IsValidCode = true;
                await CheckInventoryCodeSituation();
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message, "Error occured while checking InventoryCodeCondition");
                await Logger.Log(_fullname, $" | Error occured in InventoryCodeCondition in TicketModule Panel. | Error is: {ex.Message}");
            }
        }
        private async void IfNoInventoryCode_CheckStateChanged(object sender, EventArgs e)
        {
            if (!IfNoInventoryCode.Checked)
            {
                txtCode.PlaceholderText = "Inventory code";
                txtCode.Enabled = true;
                txtCode.Text = string.Empty;
                txtDep.Text = "Department";
                txtWorker.Text = string.Empty;
                txtDevice.Text = string.Empty;
                txtCode.Enabled = true;
                txtDevice.Enabled = false;
                txtWorker.Enabled = false;
                txtDep.Enabled= false;
                _IsValidCode = true;
            }
            else
            {
                CheckInventoryCode.Visible = false;
                txtCode.PlaceholderText = "Inventory code";
                txtCode.Text = string.Empty;
                txtCode.Enabled = false;
                txtDevice.Enabled = true;
                txtDep.Enabled = true;
                txtWorker.Enabled = true;
                _IsValidCode = true;
            }
            await Task.Delay(1);
        }
        private async void TxtCode_Leave(object sender, EventArgs e)
        {
            await InventoryCodeCondition();
        }
        #endregion
        #region ComboBox-Users
        private void CmbSelectedUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbSelectedUsers.Text = "Selected users";
        }
        private void CmbUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            string _selectedUser = cmbUsers.SelectedItem.ToString();
            if (!cmbSelectedUsers.Items.Contains(_selectedUser))
            {
                cmbSelectedUsers.Items.Add(_selectedUser);
                cmbSelectedUsers.Visible = true;
                btnDeleteUsers.Visible = true;
            }
        }
        private void BtnDeleteUsers_Click(object sender, EventArgs e)
        {
            if (cmbSelectedUsers.Items.Count == 0)
            {
                Notifications.Warning("Please select a user to delete", "No Select");
                cmbSelectedUsers.Visible = false;
                btnDeleteUsers.Visible = false;
            }
            if (cmbSelectedUsers.Items.Count == 1)
            {
                RemoveUsersInComboBox();
                cmbSelectedUsers.Visible = false;
                btnDeleteUsers.Visible = false;
            }
            if (cmbSelectedUsers.Items.Count > 1)
            {
                RemoveUsersInComboBox();
            }
        }
        private void RemoveUsersInComboBox()
        {
            // Remove the selected item from the ListBox
            cmbSelectedUsers.Items.Remove(cmbSelectedUsers.SelectedItem);
            cmbUsers.Text = "Users";
            cmbSelectedUsers.Text = "Selected users";
        }
        private async Task<string> JoinStringInComboBox(ComboBox checkBox)
        {
            try
            {
                var checkedItems = checkBox.Items;
                List<string> selectedItems = [];
                foreach (string item in checkedItems)
                {
                    if (!selectedItems.Contains(item))
                    {
                        selectedItems.Add(item.ToString());
                    }
                }
                return string.Join(",", selectedItems);
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message);
                await Logger.Log(_fullname, $" | Error is occured when using JoinStringInComboBox in Ticket Module Panel. | Error is: {ex.Message}");
                throw;
            }
        }
        #endregion
        #region Adding Tasks
        public async Task ShowTaskTable()
        {
            dgvTasks.Visible = true;
            await Task.Delay(1);
        }
        public async Task AddingTasksToTable(string newItem)
        {
            // Check if the DataGridView is empty and add the new item if so
            if (dgvTasks.Rows.Count == 0)
            {
                dgvTasks.Rows.Add(newItem);
            }
            else
            {
                // Check if the item already exists in the first column of the DataGridView
                bool TaskExists = false;
                foreach (DataGridViewRow row in dgvTasks.Rows)
                {
                    if (row.Cells[0].Value.ToString() == newItem)
                    {
                        TaskExists = true;
                        break;
                    }
                }
                // Only add if the item doesn't already exist
                if (!TaskExists)
                {
                    dgvTasks.Rows.Add(newItem);
                }
            }
            await Task.Delay(1);
        }
        private string CollectTasks()
        {
            List<string> selectedItems = [];
            foreach (DataGridViewRow row in dgvTasks.Rows)
            {
                var newRow = row.Cells[0].Value.ToString();
                if (!selectedItems.Contains(newRow))
                {
                    selectedItems.Add(newRow);
                }
            }
            return string.Join(",", selectedItems);
        }
        private async void cmbTasks_SelectedIndexChanged(object sender, EventArgs e)
        {
            await AddingTasksToTable(cmbTask.SelectedItem.ToString());
            await ShowTaskTable();
        }
        private void dgvTasks_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string colname = dgvTasks.Columns[e.ColumnIndex].Name;
            if (colname == "Delete")
            {
                dgvTasks.Rows.RemoveAt(e.RowIndex);
            }
        }
        #endregion
        #region SaveButton
        private async Task SaveTicketToDatabase()
        {
            try
            {
                string query = @"INSERT INTO Ticket
                                    (ID,code,dep_name,worker,device,task,solution,creation_date,fullname)
                              VALUES(@ID,@code, @dep_name, @worker, @device, @task, @solution,@creation_date,@fullname)
                                    INSERT INTO Status
                                    (ID,status,time)
                              VALUES(@ID,@status,@time)
                                    INSERT INTO Rating
                                    (ID)
                              VALUES(@ID)";
                using var connection = await connect.EstablishConnectionWithServiceDeskAsync(_mainMenu._sessionId);
                if (connection is null) return;
                using var cm = new SqlCommand(query, connection);
                cm.Parameters.AddWithValue("@ID", ticketID);
                cm.Parameters.AddWithValue("@code", txtCode.Text);
                cm.Parameters.AddWithValue("@dep_name", txtDep.Text);
                cm.Parameters.AddWithValue("@worker", txtWorker.Text);
                cm.Parameters.AddWithValue("@device", txtDevice.Text);
                cm.Parameters.AddWithValue("@task", _tasks);
                cm.Parameters.AddWithValue("@solution", "Processing");
                cm.Parameters.AddWithValue("@creation_date", DateTime.Now.ToString());
                cm.Parameters.AddWithValue("@status", "pending");
                cm.Parameters.AddWithValue("@time", DateTime.Now);
                cm.Parameters.AddWithValue("@fullname", _users);
                await cm.ExecuteNonQueryAsync();
                if (cm != null)
                {
                    Notifications.Information("Ticket has been succesfully saved", "Succesful");
                    await Logger.Log(_fullname, $" added a new ticket with Ticket_ID [{ticketID}], Inventory_Code  [{txtCode.Text}], Product_Department [{txtDep.Text}], Product_Worker [{txtWorker.Text}], Device [{txtDevice.Text}], Task [{_tasks}], Solution [{txtSolution.Text}], Fullname: [{_users}] to the Ticket Table");
                }
                else
                {
                    Notifications.Warning("Ticket has not been saved!");
                    return;
                }
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message, "Error saving ticket");
                await Logger.Log(_fullname, $" | Error occured in SaveTicketToDatabase in TicketModule Panel. | Error is: {ex.Message}");
            }
            finally
            {
                this.Close();
            }
        }
        private async Task SaveTicket()
        {
            try
            {
                await DefaultButtonSettings();
                await FindTicketID();
                if (MessageBox.Show("Are you sure you want to save a new ticket?", "Saving Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    await SaveTicketToDatabase();
                }
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message,"Error saving ticket");
                await Logger.Log(_fullname, $" | Error occured in Ticket Table when saving a ticket with Ticket ID [{ticketID}]. | Error is: {ex.Message}");
            }
        }
        private async void BtnSave_Click(object sender, EventArgs e)
        {
            await SaveTicket();
        }
        #endregion
        #region UpdateButton
        private async Task UpdateTicketToDatabase()
        {
            try
            {
                string query = @"
                    UPDATE Ticket
                    SET 
                          code = @code,
                          dep_name=@dep_name,
                          worker=@worker,
                          device=@device,
                          task=@task,
                          solution=@solution,
                          fullname=@fullname
                    WHERE 
                          ID=@ID";
                using var connection = await connect.EstablishConnectionWithServiceDeskAsync(_mainMenu._sessionId);
                if (connection is null) return;
                using var cm = new SqlCommand(query, connection);
                cm.Parameters.AddWithValue("@ID", ticketID);
                cm.Parameters.AddWithValue("@code", txtCode.Text);
                cm.Parameters.AddWithValue("@dep_name", txtDep.Text);
                cm.Parameters.AddWithValue("@worker", txtWorker.Text);
                cm.Parameters.AddWithValue("@device", txtDevice.Text);
                cm.Parameters.AddWithValue("@task", _tasks);
                cm.Parameters.AddWithValue("@solution", txtSolution.Text);
                cm.Parameters.AddWithValue("@fullname", _users);
                await cm.ExecuteNonQueryAsync();
                if (cm != null)
                {
                    Notifications.Information("Ticket has been succesfully updated", "Succesful");
                    await Logger.Log(_fullname, $" updated a ticket with Ticket ID: [{ticketID}], Inventory Code: [{txtCode.Text}], Department: [{txtDep.Text}], Worker: [{txtWorker.Text}], Device: [{txtDevice.Text}], Task: [{_tasks}], Solution: +[{txtSolution.Text}], Fullname: [{_users}] in Ticket Table");
                }
                else
                {
                    Notifications.Warning("Ticket has not been updated! ");
                    return;
                }
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message, "Error updating ticket");
                await Logger.Log(_fullname, $" | Error occured in UpdateTicketToDatabase in TicketModule Panel. | Error is: {ex.Message}");
            }
            finally
            {
                this.Close();
            }
        }
        private async Task UpdateTicket()
        {
            try
            {
                await DefaultButtonSettings();
                if (MessageBox.Show("Are you sure you want to update this ticket?", "Update Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    await UpdateTicketToDatabase();
                }
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message,"Error updating ticket");
                await Logger.Log(_fullname, $" | Error occured while updating ticket with ID[{ticketID}] in TicketModule Section. | Error is: {ex.Message}");
            }
        }
        private async void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (_IsValidCode && _IsValidDepartment)
            {
                await UpdateTicket();
            }
            else
            {
                Notifications.Warning("Please enter a valid product code or department name!");
                return;
            }
        }
        #endregion
        #region CloseButton
        private async Task CloseTicketInDatabase()
        {
            try
            {
                string query = @"
                    UPDATE Ticket
                    SET
                          code = @code,
                          dep_name=@dep_name,
                          worker=@worker,
                          device=@device,
                          task=@task,
                          solution=@solution,
                          finished_time=@finished_time,
                          taken_time=@taken_time,
                          fullname=@fullname
                    WHERE
                          ID=@ID";
                using var connection = await connect.EstablishConnectionWithServiceDeskAsync(_mainMenu._sessionId);
                if (connection is null) return;
                {
                    using var cm = new SqlCommand(query, connection);
                    cm.Parameters.AddWithValue("@ID", ticketID);
                    cm.Parameters.AddWithValue("@code", txtCode.Text);
                    cm.Parameters.AddWithValue("@dep_name", txtDep.Text);
                    cm.Parameters.AddWithValue("@worker", txtWorker.Text);
                    cm.Parameters.AddWithValue("@device", txtDevice.Text);
                    cm.Parameters.AddWithValue("@task", _tasks);
                    cm.Parameters.AddWithValue("@fullname", _users);
                    cm.Parameters.AddWithValue("@solution", txtSolution.Text);
                    cm.Parameters.AddWithValue("@finished_time", DateTime.Now.ToString());
                    cm.Parameters.AddWithValue("@taken_time", CalculateTime(DateTime.Parse(timeElapsed)));
                    await cm.ExecuteNonQueryAsync();
                    if (cm != null)
                    {
                        Notifications.Information("Ticket has been succesfully closed", "Succesful");
                        await Logger.Log(_fullname, $" closed a ticket with Ticket ID: [{ticketID}], Inventory Code: [{txtCode.Text}], Department: [{txtDep.Text}], Worker: [{txtWorker.Text}], Device: [{txtDevice.Text}], Task: [{_tasks}], Solution: [{txtSolution.Text}], Fullname: [{_fullname}] in Ticket Table");
                        await CloseTicketStatus("closed");
                        await CloseTicketStatus("resolved");
                    }
                    else
                    {
                        Notifications.Warning("Ticket has not been closed");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message, "Error closing ticket");
                await Logger.Log(_fullname, $" | Error occured in CloseTicketInDatabase in TicketModule Panel. | Error is: {ex.Message}");
            }
            finally
            {
                this.Close();
            }
        }
        private async Task CreateAndCloseTicketInDatabase()
        {
            await FindTicketID();
            try
            {
                string query = @"INSERT INTO Ticket
                                    (ID,code,dep_name,worker,device,task,solution,creation_date,fullname,finished_time,taken_time)
                              VALUES(@ID,@code, @dep_name, @worker, @device, @task, @solution,@creation_date,@fullname,@finished_time,@taken_time)
                                    INSERT INTO Status
                                    (ID,status,time)
                              VALUES(@ID,@status,@time)
                                    INSERT INTO Rating
                                    (ID)
                              VALUES(@ID)";
                using var connection = await connect.EstablishConnectionWithServiceDeskAsync(_mainMenu._sessionId);
                if (connection is null) return;
                using var cm = new SqlCommand(query, connection);
                cm.Parameters.AddWithValue("@ID", ticketID);
                cm.Parameters.AddWithValue("@code", txtCode.Text);
                cm.Parameters.AddWithValue("@dep_name", txtDep.Text);
                cm.Parameters.AddWithValue("@worker", txtWorker.Text);
                cm.Parameters.AddWithValue("@device", txtDevice.Text);
                cm.Parameters.AddWithValue("@task", _tasks);
                cm.Parameters.AddWithValue("@solution", txtSolution.Text);
                cm.Parameters.AddWithValue("@creation_date", date.Value.ToString());
                cm.Parameters.AddWithValue("@finished_time", date.Value.ToString());
                cm.Parameters.AddWithValue("@taken_time", "1 m");
                cm.Parameters.AddWithValue("@status", "closed");
                cm.Parameters.AddWithValue("@time", date.Value);
                cm.Parameters.AddWithValue("@fullname", _users);
                await cm.ExecuteNonQueryAsync();
                if (cm != null)
                {
                    Notifications.Information("Ticket has been succesfully created", "Succesful");
                    await Logger.Log(_fullname, $" created a new ticket with Ticket_ID [{ticketID}], Inventory_Code  [{txtCode.Text}], Product_Department [{txtDep.Text}], Product_Worker [{txtWorker.Text}], Device [{txtDevice.Text}], Task [{_tasks}], Solution [{txtSolution.Text}], Fullname: [{_users}] to the Ticket Table");
                }
                else
                {
                    Notifications.Warning("Ticket has not been created!");
                    return;
                }
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message, "Error closing ticket");
                await Logger.Log(_fullname, $" | Error occured in CloseTicketInDatabase in TicketModule Panel. | Error is: {ex.Message}");
            }
            finally
            {
                this.Close();
            }
        }
        private async Task CloseTicketStatus(string status)
        {
            //Closing the status of ticket
            using var connection = await connect.EstablishConnectionWithServiceDeskAsync(_mainMenu._sessionId);
            if (connection is null) return;
            using var cm = new SqlCommand("UPDATE Status SET status=@status WHERE status=@currentStatus AND ID=@ID", connection);
            cm.Parameters.AddWithValue("@status", status);
            cm.Parameters.AddWithValue("@currentStatus", status == "closed" ? "pending" : "resolving");
            cm.Parameters.AddWithValue("@ID", ticketID);
            await cm.ExecuteNonQueryAsync();
        }
        private async Task CloseButton()
        {
            try
            {
                await DefaultButtonSettings();
                if (!_IsValidCode || !_IsValidDepartment || dgvTasks.Rows.Count == 0 || cmbSelectedUsers.Items.Count == 0 || string.IsNullOrEmpty(txtSolution.Text))
                {
                    Notifications.Warning("Please fill all required fields correctly!");
                    return;
                }
                if (MessageBox.Show("Are you sure you want to close this ticket?", "Close Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (_isNotTodayTask)
                    {
                        await CreateAndCloseTicketInDatabase();
                    }
                    else
                        await CloseTicketInDatabase();
                }
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message,"Error saving ticket");
                await Logger.Log(_fullname, $" | Error occured when closing ticket with ID[{ticketID}] in TicketModule Section. | Error is: {ex.Message}");
            }
            finally
            {
                this.Dispose();
            }
        }
        private async void Btn_Close_Click(object sender, EventArgs e)
        {
            await CloseButton();
        }
        #endregion
        #region Clear Button
        private async Task ClearTextsForNotValidCode()
        {
            txtWorker.Text = string.Empty;
            txtDevice.Text = string.Empty;
            txtDep.Enabled = false;
            txtWorker.Enabled = false;
            txtDevice.Enabled = false;
            txtCode.Enabled = true;
            txtDep.Text = "Department";
            txtCode.PlaceholderText = "Inventory code";
            txtDevice.PlaceholderText = "Device";
            txtWorker.PlaceholderText = "Worker";
            txtSolution.PlaceholderText = "Solution";
            cmbTask.Text = "Select a category";
            await Task.Delay(1);
        }
        public async Task HideCategoryTable()
        {
            dgvTasks.Visible = false;
            dgvTasks.Rows.Clear();
            await Task.Delay(1);
        }
        private async void BtnClear_Click(object sender, EventArgs e)
        {
            await ClearTextsForNotValidCode();
            await HideCategoryTable();
        }
        #endregion
        #region Controlbuttons
        private void BtnExit_Click(object sender, EventArgs e) => this.Dispose();
        #endregion
        #region Design
        private void cmbTasks_Leave(object sender, EventArgs e)
        {
            if (!cmbTask.Items.Contains(cmbTask.Text))
            {
                cmbTask.Text = "Select a category";
            }
        }
        private void CmbTasks_Enter(object sender, EventArgs e)
        {
            if (cmbTask.Text == "Select a category")
            {
                cmbTask.Text = "";
            }
        }
        private void TicketModule_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (btnSave.Visible == true)
                {
                    BtnSave_Click(this, EventArgs.Empty);  // Trigger the Click event
                    e.SuppressKeyPress = true;  // Prevent the default behavior
                    e.Handled = true;// Stops the event from being passed to the control
                }
                else
                {
                    if (btnClose.Enabled == false)
                        BtnUpdate_Click(this, EventArgs.Empty);
                    else
                        Btn_Close_Click(this, EventArgs.Empty);
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                }
            }
        }
        private void TxtDep_Enter(object sender, EventArgs e)
        {
            if (txtDep.Text == "Department")
            {
                txtDep.Text = "";
            }
        }
        private void TxtDep_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtDep.Text))
            {
                txtDep.Text = "Department";
            }
            if (!txtDep.Items.Contains(txtDep.Text))
            {
                _IsValidDepartment = false;
                txtDep.Text = "Department";
                return;
            }
            _IsValidDepartment = true;
        }
        private void TicketModule_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);
        private void TxtSolution_Enter(object sender, EventArgs e)
        {
            if (txtSolution.Text == "Processing")
            {
                txtSolution.Text = "";
                txtSolution.PlaceholderText = "Processing";
            }
        }
        private void TxtSolution_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSolution.Text))
            {
                txtSolution.PlaceholderText = "Solution";
            }
        }
        #endregion
    }
}