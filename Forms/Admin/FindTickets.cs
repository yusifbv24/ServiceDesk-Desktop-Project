using ServiceDesk.Class;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServiceDesk.Forms
{
    public partial class FindTickets : Form
    {
        private Main _mainMenu;
        private readonly string _fullname = default;
        private ProductServiceClient _productServiceClient;
        private SqlConnection _connection_servicedesk { get; set; } = null;
        public FindTickets(string fullname,Main mainMenu,out FindTickets findTickets)
        {
            InitializeComponent();
            _fullname = fullname;
            _mainMenu = mainMenu;
            findTickets = this;
            _productServiceClient=new ProductServiceClient();
            _ = LoadUsers();
            _ = LoadDepartments();
        }
        private async Task ConnectToTheServiceDeskDatabase()
        {
            if (_connection_servicedesk == null)
            {
                _connection_servicedesk = await ConnectionDatabase.ConnectToTheServer(_mainMenu._sessionId);
                await _connection_servicedesk.OpenAsync();
            }
            if (_connection_servicedesk.State == ConnectionState.Closed)
            {
                await _connection_servicedesk.OpenAsync();
            }
        }
        private string RemoveStringFromTime(string text)
        {
            if (text.Contains("ago"))
            {
                text = text.Replace("ago", "");
                return text;
            }
            else
                return text;
        }
        public async Task LoadDefaultSettings()
        {
            if (string.IsNullOrEmpty(cmbUserSearch.Text)||string.IsNullOrEmpty(cmbDepartmentSearch.Text))
            {
                return;
            }
            if (cmbUserSearch.Text != "Select a user")
            {
                await CheckingDepartmentText();
            }
            if (cmbDepartmentSearch.Text != "Select a department")
            {
                await CheckingUserText();
            }
        }
        private async Task LoadUsers()
        {
            cmbUserSearch.Items.Clear();
            try
            {
                if (_connection_servicedesk == null || _connection_servicedesk.State == ConnectionState.Closed)
                {
                    await ConnectToTheServiceDeskDatabase();
                }
                using var cm = new SqlCommand("SELECT fullname FROM Users WHERE type=@type ORDER BY fullname ASC", _connection_servicedesk);
                cm.Parameters.AddWithValue("@type", "User");
                using var dr = await cm.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                while (await dr.ReadAsync())
                {
                    cmbUserSearch.Items.Add(dr["fullname"].ToString());
                }
                _connection_servicedesk?.Close();
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message, "Error occured while loading users");
                await Logger.Log(_fullname, $" | Error is occured when loading users in FindTicket Panel. | Error is: {ex.Message}");
            }
        }
        private async Task LoadDepartments()
        {
            try
            {
                var departmentsNames = await _productServiceClient.GetDepartmentsAsync();
                foreach (var department in departmentsNames)
                {
                    cmbDepartmentSearch.Items.Add(department.Name);
                }
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message, "Error occured while loading departments");
                await Logger.Log(_fullname, $" | Error is occured when loading departments in FindTicket Panel. | Error is: {ex.Message}");
            }
        }
        private async Task CheckingDepartmentText()
        {
            if (cmbDepartmentSearch.Text!="Select a department")
            {
                await LoadTicketsForUser(cmbDepartmentSearch.Text);
            }
            else
            {
                await LoadTicketsForUser();
            }
        }
        private async Task CheckingUserText()
        {
            if (cmbUserSearch.Text!="Select a user")
            {
                await LoadTicketsForDepartment(cmbUserSearch.Text);
            }
            else
            {
                await LoadTicketsForDepartment();
            }
        }
        private async Task LoadTicketsForDepartment(string _user=null)
        {
            try
            {
                txtSearchCode.Text = string.Empty;
                dgvTicket.Rows.Clear();
                dgvTicket.Columns["Dep"].Visible = false;
                dgvTicket.Columns["User"].Visible = true;
                string query = @"SELECT Ticket.*
                                FROM Ticket 
                                INNER JOIN Status ON Ticket.ID=Status.ID
                                WHERE dep_name LIKE @dep_name
                                AND Status.time BETWEEN @fromDate AND @toDate";
                if (!string.IsNullOrEmpty(_user))
                {
                    query += " AND fullname LIKE @fullname";
                }
                if (!string.IsNullOrEmpty(_mainMenu.txtSearch.Text))
                {
                    query = @" SELECT Ticket.*
                                FROM Ticket 
                                INNER JOIN Status ON Ticket.ID=Status.ID
                                WHERE dep_name LIKE @dep_name
                                AND Status.time BETWEEN @fromDate AND @toDate
                                AND (Ticket.ID LIKE @searchText 
                                OR code LIKE @searchText 
                                OR worker LIKE @searchText 
                                OR device LIKE @searchText 
                                OR task LIKE @searchText 
                                OR solution LIKE @searchText 
                                OR creation_date LIKE @searchText 
                                OR fullname LIKE @searchText 
                                OR finished_time LIKE @searchText 
                                OR taken_time LIKE @searchText)
                                ORDER BY ID desc";
                }
                if (_connection_servicedesk == null || _connection_servicedesk.State == ConnectionState.Closed)
                {
                    await ConnectToTheServiceDeskDatabase();
                }
                using var cm = new SqlCommand(query, _connection_servicedesk);
                cm.Parameters.AddWithValue("@dep_name", cmbDepartmentSearch.Text);
                cm.Parameters.AddWithValue("@fromDate", _mainMenu.fromDate);
                cm.Parameters.AddWithValue("@toDate", _mainMenu.toDate);
                if (!string.IsNullOrEmpty(_user))
                {
                    cm.Parameters.AddWithValue("@fullname", $"%{cmbUserSearch.Text}%");
                }
                // Add search parameter only if searchText is not empty
                if (!string.IsNullOrEmpty(_mainMenu.txtSearch.Text))
                {
                    cm.Parameters.AddWithValue("@searchText", $"%{_mainMenu.txtSearch.Text}%");
                }
                using var dr = await cm.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                while (await dr.ReadAsync())
                {
                    dgvTicket.Rows.Add(
                        dr[0].ToString(),
                        dr[1].ToString(),
                        dr[3].ToString(),
                        dr[4].ToString(),
                        dr[5].ToString(),
                        dr[6].ToString(),
                        dr[7].ToString(),
                        RemoveStringFromTime(dr[10].ToString()),
                        dr[9].ToString(),
                        dr[2].ToString(),
                        dr[8].ToString());
                }
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message, "Error occured while running LoadTicketsForDepartment");
                await Logger.Log(_fullname, $" | Error occured in FindTickets Panel while running FindTickets. | Error is: {ex.Message}");
            }
            finally
            {
                _mainMenu.lblTotalResult.Text= dgvTicket.Rows.Count.ToString();
            }
        }
        private async Task LoadTicketsForUser(string _department=null)
        {
            try
            {
                txtSearchCode.Text = string.Empty;
                dgvTicket.Rows.Clear();
                dgvTicket.Columns["User"].Visible = false;
                dgvTicket.Columns["Dep"].Visible = true;
                string query = @"SELECT Ticket.* 
                                FROM Ticket 
                                INNER JOIN Status ON Ticket.ID=Status.ID 
                                WHERE fullname LIKE @fullname 
                                AND Status.time BETWEEN @fromDate AND @toDate";
                if (!string.IsNullOrEmpty(_department))
                {
                    query += " AND dep_name LIKE @dep_name ";
                }
                if (!string.IsNullOrEmpty(_mainMenu.txtSearch.Text))
                {
                    query = @" SELECT Ticket.*
                                FROM Ticket 
                                INNER JOIN Status ON Ticket.ID=Status.ID
                                WHERE fullname LIKE @fullname
                                AND Status.time BETWEEN @fromDate AND @toDate 
                                AND (Ticket.ID LIKE @searchText 
                                OR code LIKE @searchText 
                                OR dep_name LIKE @searchText 
                                OR worker LIKE @searchText 
                                OR device LIKE @searchText 
                                OR task LIKE @searchText 
                                OR solution LIKE @searchText 
                                OR creation_date LIKE @searchText 
                                OR fullname LIKE @searchText 
                                OR finished_time LIKE @searchText 
                                OR taken_time LIKE @searchText)
                                ORDER BY ID DESC";
                }
                if (_connection_servicedesk == null || _connection_servicedesk.State == ConnectionState.Closed)
                {
                    await ConnectToTheServiceDeskDatabase();
                }
                using var cm = new SqlCommand(query, _connection_servicedesk);
                cm.Parameters.AddWithValue("@fullname", $"%{cmbUserSearch.Text}%");
                cm.Parameters.AddWithValue("@fromDate", _mainMenu.fromDate);
                cm.Parameters.AddWithValue("@toDate", _mainMenu.toDate);
                if (!string.IsNullOrEmpty(_department))
                {
                    cm.Parameters.AddWithValue("@dep_name", $"%{cmbDepartmentSearch.Text}%");
                }
                // Add search parameter only if searchText is not empty
                if (!string.IsNullOrEmpty(_mainMenu.txtSearch.Text))
                {
                    cm.Parameters.AddWithValue("@searchText", $"%{_mainMenu.txtSearch.Text}%");
                }
                using var dr = await cm.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                while (dr.Read())
                {
                    dgvTicket.Rows.Add(
                        dr[0].ToString(),
                        dr[1].ToString(),
                        dr[3].ToString(),
                        dr[4].ToString(),
                        dr[5].ToString(), 
                        dr[6].ToString(), 
                        dr[7].ToString(), 
                        RemoveStringFromTime(dr[10].ToString()),
                        dr[9].ToString(), 
                        dr[2].ToString(),
                        dr[8].ToString());
                }
            }
            catch (Exception ex)
            {
                Notifications.Error(ex.Message, "Error occured while running LoadTicketsForUser");
                await Logger.Log(_fullname, $" | Error occured in FindTickets Panel while running LoadTicketsByUser. | Error is: {ex.Message}");
            }
            finally
            {
                _mainMenu.lblTotalResult.Text = dgvTicket.Rows.Count.ToString();
            }
        }
        private async void CmbUserSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            await CheckingDepartmentText();
        }
        private async void CmbDepartmentSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            await CheckingUserText();
        }
        private void CmbUserSearch_Leave(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(cmbUserSearch.Text))
            {
                cmbUserSearch.Text = "Select a user";
            }
        }
        private void CmbUserSearch_Enter(object sender, EventArgs e)
        {
            if (cmbUserSearch.Text == "Select a user")
            {
                cmbUserSearch.Text = string.Empty;
            }
        }
        private void CmbDepartmentSearch_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cmbDepartmentSearch.Text))
            {
                cmbDepartmentSearch.Text = "Select a department";
            }
        }
        private void CmbDepartmentSearch_Enter(object sender, EventArgs e)
        {
            if(cmbDepartmentSearch.Text=="Select a department")
            {
                cmbDepartmentSearch.Text=string.Empty;
            }
        }
        private async void TxtSearchCode_TextChanged(object sender, EventArgs e)
        {
            await LoadTicketsByInventoryCode();
        }
        private async Task LoadTicketsByInventoryCode()
        {
            if (!string.IsNullOrEmpty(txtSearchCode.Text))
            {
                var _connection = await ConnectionDatabase.ConnectToTheServer(_mainMenu._sessionId);
                try
                {
                    cmbDepartmentSearch.Text = "Select a department";
                    cmbUserSearch.Text = "Select a user";
                    dgvTicket.Rows.Clear();
                    dgvTicket.Columns["User"].Visible = true;
                    dgvTicket.Columns["Dep"].Visible = true;
                    string query = @"SELECT * FROM Ticket 
                                WHERE code LIKE @code1 
                                OR code LIKE @code2 
                                OR code LIKE @code3";

                    if (!string.IsNullOrEmpty(_mainMenu.txtSearch.Text))
                    {
                        query = @" SELECT Ticket.* FROM Ticket 
                                    INNER JOIN Status ON Ticket.ID=Status.ID
                                    WHERE code LIKE @code1 
                                    OR code LIKE @code2 
                                    OR code LIKE @code3
                                    AND Status.time BETWEEN @fromDate AND @toDate
                                    AND (Ticket.ID LIKE @searchText 
                                    OR code LIKE @searchText 
                                    OR dep_name LIKE @searchText 
                                    OR worker LIKE @searchText 
                                    OR device LIKE @searchText 
                                    OR task LIKE @searchText 
                                    OR solution LIKE @searchText 
                                    OR creation_date LIKE @searchText 
                                    OR fullname LIKE @searchText 
                                    OR finished_time LIKE @searchText 
                                    OR taken_time LIKE @searchText)
                                    ORDER BY ID desc";
                    }
                    if (_connection == null) return;
                    await _connection.OpenAsync();
                    if (_connection.State == ConnectionState.Closed) return;
                    using var cm = new SqlCommand(query, _connection);
                    cm.Parameters.AddWithValue("@code", txtSearchCode.Text);
                    // Add search parameter only if searchText is not empty
                    if (!string.IsNullOrEmpty(_mainMenu.txtSearch.Text))
                    {
                        cm.Parameters.AddWithValue("@searchText", $"{_mainMenu.txtSearch.Text}");
                        cm.Parameters.AddWithValue("@fromDate", _mainMenu.fromDate);
                        cm.Parameters.AddWithValue("@toDate", _mainMenu.toDate);
                    }
                    using var dr = await cm.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                    while (await dr.ReadAsync())
                    {
                        dgvTicket.Rows.Add(
                            dr[0].ToString(),
                            dr[1].ToString(),
                            dr[3].ToString(),
                            dr[4].ToString(),
                            dr[5].ToString(),
                            dr[6].ToString(),
                            dr[7].ToString(),
                            RemoveStringFromTime(dr[10].ToString()),
                            dr[9].ToString(),
                            dr[2].ToString(),
                            dr[8].ToString());
                    }
                }
                catch (Exception ex)
                {
                    Notifications.Error(ex.Message, "Error occured while running LoadTicketsByInventoryCode");
                    await Logger.Log(_fullname, $" | Error occured in FindTicket Panel while running LoadTicketsByInventoryCode. | Error is: {ex.Message}");
                }
            }
            else
            {
                dgvTicket.Rows.Clear();
            }
        }
    }
}