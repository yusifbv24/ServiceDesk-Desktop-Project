using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using ServiceDesk.Class;
using ServiceDesk.Properties;

namespace ServiceDesk.Forms
{
    public partial class Main : Form
    {
        private readonly Connect connect = Connect.Instance;
        public string fromDate;
        public string toDate;
        private OpenedTickets _openedTicketsForUser;
        private OpenedTicketsForAdminPanel _openedTicketsForAdmin;
        private ClosedTickets _closedTicketsForUser;
        private ClosedTicketsForAdminPanel _closedTicketsForAdmin;
        private FindTickets _findTickets;
        private Tasks _problems;
        private Users _users;
        private readonly string _fullname = default;
        public readonly string _userType = default;
        private bool _isActiveUser = true; // Prevent double logout
        public readonly Guid _sessionId;
        private Timer checkActivityTimer;
        private Form _activeForm = null;

        private readonly Dictionary<string, string> _titleForForms = new()
        {
            {"Main","Home"},
            {"Dashboard","Dashboard"},
            {"OpenTicketsForUser","Create a ticket or modify it" },
            {"OpenTicketsForAdmin","Show open tickets" },
            {"ClosedTicketForUser","Show closed tickets" },
            {"ClosedTicketForAdmin","Give a feedback to closed tickets" },
            {"FindTickets","Find tickets in detailed information"},
            {"Problems","Create or modify problems" },
            {"Users","Create or modify users"}
        };
        public Main(string userType, string fullname,Guid sessionId)
        {
            InitializeComponent();
            _userType = userType;
            _fullname = fullname;
            _sessionId = sessionId;
            SettingsForUser();
            _=StartupSettings();
            checkActivityTimer = new Timer
            {
                Interval = 60000 // Check every 60 seconds
            };
            checkActivityTimer.Tick += async (sender, e) => await CheckUserActivity();
            checkActivityTimer.Start();
        }
        #region Control Buttons
        private void BtnMinimize_Click(object sender, EventArgs e) => this.WindowState = FormWindowState.Minimized;
        private void BtnMaximize_Click(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Maximized;
            }
            else
                this.WindowState = FormWindowState.Normal;
        }
        private void BtnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion
        #region Timer and Watch
        private void Watch_Tick(object sender, EventArgs e)
        {
            lblShowWatch.Text = String.Format("{0:HH:mm}", DateTime.Now);
            lblShowTime.Text = String.Format("{0:dd.MM.yy}", DateTime.Now);
        }
        #endregion
        #region Add button
        private async void BtnAddTicket_Click(object sender, EventArgs e)
        {
            if (lblHome.Text == _titleForForms["OpenTicketsForUser"] || lblHome.Text == _titleForForms["OpenTicketsForAdmin"])
            {
                TicketModule ticketModule = new (_fullname, _userType,this);
                ticketModule.txtSolution.Visible = false;
                ticketModule.btnSave.Visible = true;
                ticketModule.btnClear.Visible=true;
                await ticketModule.HideTxtSolution();
                ticketModule.NotToday.Visible = true;
                ticketModule.ShowDialog();
                if (_userType == "Admin")
                    await _openedTicketsForAdmin.LoadTickets();
                else
                    await _openedTicketsForUser.LoadTickets();
            }
            if (lblHome.Text == _titleForForms["Problems"])
            {
                TaskModule problemModule = new (_fullname,this);
                problemModule.btnSave.Visible = true;
                problemModule.btnClear.Visible = true;
                problemModule.ShowDialog();
                await _problems.LoadTasks();
            }
            if (lblHome.Text == _titleForForms["Users"])
            {
                UserModule userModule = new (_fullname,this);
                userModule.btnSave.Visible = true;
                userModule.btnClear.Visible = true;
                userModule.ShowDialog();
                await _users.LoadUsers();
            }
        }
        private async void DateFiltering_SelectedIndexChanged(object sender, EventArgs e)
        {
            await OpeningDashboardForm();
        }
        private void BtnOk_Click(object sender, EventArgs e)
        {
            panelFilter.Visible = false;
            btnFilter.Image = btnFilter.PressedState.Image;
            TxtSearch_TextChanged(sender, e);
        }
        private void BtnFilter_Click(object sender, EventArgs e)
        {
            if (panelFilter.Visible == false)
            {
                panelFilter.Visible = true;
                btnFilter.Image = Resources.adjust_filter_color;
            }
            else
            {
                panelFilter.Visible = false;
                btnFilter.Image = Resources.adjust_filter;
            }
        }
        private async void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadDates();
            if (lblHome.Text == _titleForForms["OpenTicketsForUser"])
            {
                await _openedTicketsForUser.LoadTickets();
            }
            if (lblHome.Text == _titleForForms["OpenTicketsForAdmin"])
            {
                await _openedTicketsForAdmin.LoadTickets();
            }
            if (lblHome.Text == _titleForForms["ClosedTicketForUser"])
            {
                if (_closedTicketsForUser == null || _closedTicketsForUser.IsDisposed)
                {
                    await OpenChildForm(new ClosedTickets(_fullname, this, out _closedTicketsForUser));
                }
                await _closedTicketsForUser.LoadTickets();
            }
            if (lblHome.Text == _titleForForms["ClosedTicketForAdmin"])
            {
                if (_closedTicketsForAdmin == null || _closedTicketsForAdmin.IsDisposed)
                {
                    await OpenChildForm(new ClosedTicketsForAdminPanel(_fullname, this, out _closedTicketsForAdmin));
                }
                await _closedTicketsForAdmin.LoadTickets();
            }
            if (lblHome.Text == _titleForForms["FindTickets"])
            {
                if (_findTickets == null || _findTickets.IsDisposed)
                {
                    await OpenChildForm(new FindTickets(_fullname, this, out _findTickets));
                }
                await _findTickets.LoadDefaultSettings();
            }
            if (lblHome.Text == _titleForForms["Problems"])
            {
                if (_problems == null || _problems.IsDisposed)
                {
                    await OpenChildForm(new Tasks(_fullname, this, out _problems));
                }
                await _problems.LoadTasks();
            }
            if (lblHome.Text == _titleForForms["Users"])
            {
                if (_users == null || _users.IsDisposed)
                {
                    await OpenChildForm(new Users(_fullname, this, out _users));
                }
                await _users.LoadUsers();
            }
        }
        #endregion
        #region Settings
        private async Task TitleNameToDefault()
        {
            lblHome.Text = _titleForForms["Main"];
            await Task.Delay(1);
        }
        private async Task LoadingDefaultDate()
        {
            dateFrom.Value = DateTime.Now.AddDays(-7);
            dateTo.Value = DateTime.Now;
            LoadDates();
            await Task.Delay(1);
        }
        private void LoadDates()
        {
            fromDate = dateFrom.Value.ToString("yyyy-MM-dd");
            toDate = dateTo.Value.ToString("yyyy-MM-dd");
        }
        private async Task StartupSettings()
        {
            MaximizedBounds = Screen.FromHandle(Handle).WorkingArea;
            this.ControlBox = false;
            await LoadingDefaultDate();
        }
        private async Task HideButtonsForSomeActions()
        {
            lblTotalResult.Text = string.Empty;
            dateFiltering.Visible = false;
            lblTotal.Visible = false;
            lblTotalResult.Visible = false;
            btnAddTicket.Visible = false;
            txtSearch.Visible = false;
            btnFilter.Visible = false;
            panelFilter.Visible = false;
            txtSearch.Text = string.Empty;
            this.ControlBox = false;
            await Task.Delay(1);
        }
        private void SettingsForUser()
        {
            if (_userType == "User")
            {
                btnLeftThinButton_Problems.Visible = false;
                btn_Tasks.Visible = false;
                btnLeftThinButton_User.Visible = false;
                btn_User.Visible = false;
            }
        }
        private async Task ShowButtonsToDefault()
        {
            txtSearch.Visible = true;
            lblTotal.Visible = true;
            lblTotalResult.Visible = true;
            await Task.Delay(1);
        }
        #endregion
        #region CallingChildForms
        private void CloseActiveForm()
        {
            if (_activeForm != null)
            {
                _activeForm.Close();
                _activeForm.Dispose();
                _activeForm = null;
                this.panelMain.Controls.Clear();
                _ = HideButtonsForSomeActions();
            }
        }
        private async Task OpenChildForm(Form childForm, string title = "Home")
        {
            CloseActiveForm();
            try
            {
                _activeForm = childForm;
                childForm.TopLevel = false;
                childForm.FormBorderStyle = FormBorderStyle.None;
                childForm.Dock = DockStyle.Fill;

                this.panelMain.Controls.Add(childForm);
                this.panelMain.Tag = childForm;
                childForm.BringToFront();
                childForm.Show();
                lblHome.Text = title;
                await Task.Delay(1);
            }
            catch (ObjectDisposedException)
            {
                Notifications.Warning("Be patient. Please click one by one : ", "Object is disposed");
            }
            finally
            {
                await Task.Delay(1);
            }
        }
        #endregion
        #region LogoutSection
        //Check for user is active or not
        private async Task CheckUserActivity()
        {
            await IsActiveOrNot();
            if (!_isActiveUser)
            {
                try
                {
                    checkActivityTimer.Stop();
                    await Logout(Dns.GetHostName());
                    Notifications.Warning("Your session has been terminated. The application will now close.", "Session Inactive");
                    Application.Exit();
                }
                catch (Exception ex)
                {
                    Notifications.Error($"{ex.Message}", "Error occured while running CheckUserActivity");
                    await Logger.Log(_fullname, $" | Error is occured while running CheckUserActivity in Main panel. | Error is: {ex.Message}");
                }
            }
        }
        private async Task IsActiveOrNot()
        {
            try
            {
                using var connection =  connect.LoginToTheServerAsync();
                string query = @"SELECT IsActive FROM UserSessions WHERE SessionId = @SessionId";
                if (connection is null) return;
                using var cm = new SqlCommand(query, connection);
                cm.Parameters.AddWithValue("@SessionId", _sessionId);
                using var dr = await cm.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                _isActiveUser = await dr.ReadAsync() && dr.GetBoolean(0); // Assumes IsActive is a BIT column
            }
            catch (Exception ex)
            {
                Notifications.Error($"{ex.Message}", "Error occured while running IsActiveOrNot");
                await Logger.Log(_fullname, $" | Error is occured while running IsActiveOrNot in Main panel. | Error is: {ex.Message}");
            }
        }
        public async Task Logout(string session="")
        {
            if (_isActiveUser)
            {
                try
                {
                    using var connection = connect.LoginToTheServerAsync();
                    if (connection is null) return;
                    string query = @"  UPDATE UserSessions 
                                           SET IsActive = 0 
                                           WHERE SessionId = @SessionId;
                                           UPDATE Users
                                           SET session=@session,
                                           ip_address=@ip_address
                                           WHERE fullname LIKE @fullname;";
                    using var cm = new SqlCommand(query, connection);
                    cm.Parameters.AddWithValue("@SessionId", _sessionId);
                    cm.Parameters.AddWithValue("@session", session);
                    cm.Parameters.AddWithValue("@ip_address", "");
                    cm.Parameters.AddWithValue("@fullname", _fullname);
                    await cm.ExecuteNonQueryAsync();
                    await Logger.Log(_fullname, " logged out of the system ");
                    _isActiveUser = false;
                }
                catch (Exception ex)
                {
                    Notifications.Error($"{ex.Message}", "Error occured while logging out");
                    await Logger.Log(_fullname, $" | Error is occured when logging from account. | Error is: {ex.Message}");
                }
            }
        }
        private async void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            await Logout(Dns.GetHostName());
            Application.Exit();
        }
        private async void BtnLogOut_Click(object sender, EventArgs e)
        {
            this.Close();
            await Logout();
            CredentialManager.RemoveCredentials(_fullname);
            Application.Restart();
        }
        #endregion
        #region MainButtons
        //Dashboard button
        private async void Btn_Dashboard_Click(object sender, EventArgs e)
        {
            await NormalPanelConf();
            CloseActiveForm();
            if (btnLeftThinButton_Dashboard.Visible == true)
            {
                await NotVisibleThinButton();
                await TitleNameToDefault();
                btn_Dashboard.ForeColor = Color.White;
                btn_Dashboard.Image = Resources.dashboard;
            }
            else
            {
                await NotVisibleThinButton();
                dateFiltering.Text = "daily";
                btnLeftThinButton_Dashboard.Visible = true;
                btn_Dashboard.ForeColor = Color.FromArgb(0, 139, 255);
                btn_Dashboard.Image = Resources.dashboard_color;
                await OpeningDashboardForm();
            }
        }
        private async Task OpeningDashboardForm()
        {
            if (_userType == "Admin")
            {
                await OpenChildForm(new DashboardForAdminPanel(_fullname, this),  _titleForForms["Dashboard"]);
            }
            else
            {
                await OpenChildForm(new Dashboard(_fullname, this), _titleForForms["Dashboard"]);
            }
            dateFiltering.Visible = true;
        }
        //Normal ticket button
        private async void Btn_Tickets_Click(object sender, EventArgs e)
        {
            CloseActiveForm();
            await TitleNameToDefault();
            await NotVisibleThinButton();
            await ExpandPanelConf();
            await ForeColorForMainButtons();
            await ForeColorForTicketButtons();
        }
        //Expanded ticket button
        private async void Btn_ColourTickets_Click(object sender, EventArgs e)
        {
            await NormalPanelConf();
            CloseActiveForm();
            await TitleNameToDefault();
            btnLeftThinButton_Tickets.Visible = false;
        }
        //Open ticket button
        private async void BtnOpenTickets_Click(object sender, EventArgs e)
        {
            await ForeColorForTicketButtons();
            if (lblHome.Text == _titleForForms["OpenTicketsForAdmin"] || lblHome.Text == _titleForForms["OpenTicketsForUser"])
            {
                Btn_Tickets_Click(sender, e);
            }
            else
            {
                CloseActiveForm();
                await ShowButtonsToDefault();
                btnAddTicket.Visible = true;
                btnOpenedTickets.ForeColor = Color.FromArgb(0, 139, 255);
                if (_userType == "Admin")
                {
                    await OpenChildForm(new OpenedTicketsForAdminPanel(_fullname, this, out _openedTicketsForAdmin),  _titleForForms["OpenTicketsForAdmin"]);
                }
                else
                {
                    await OpenChildForm(new OpenedTickets(_fullname, this, out _openedTicketsForUser),  _titleForForms["OpenTicketsForUser"]);
                }
            }
        }
        //Closed ticket button
        private async void BtnClosedTickets_Click(object sender, EventArgs e)
        {
            await ForeColorForTicketButtons();
            if (lblHome.Text == _titleForForms["ClosedTicketForAdmin"] || lblHome.Text == _titleForForms["ClosedTicketForUser"])
            {
                Btn_Tickets_Click(sender, e);
            }
            else
            {
                CloseActiveForm();
                await ShowButtonsToDefault();
                btnClosedTickets.ForeColor = Color.FromArgb(0, 139, 255);
                btnFilter.Visible = true;
                if (_userType == "Admin")
                {
                    await OpenChildForm(new ClosedTicketsForAdminPanel(_fullname, this, out _closedTicketsForAdmin),  _titleForForms["ClosedTicketForAdmin"]);
                }
                else
                {
                    await OpenChildForm(new ClosedTickets(_fullname, this, out _closedTicketsForUser), _titleForForms["ClosedTicketForUser"]);
                }
            }
        }
        //Find tickets in detailed information
        private async void BtnFindTickets_Click(object sender, EventArgs e)
        {
            await ForeColorForTicketButtons();
            if (lblHome.Text == _titleForForms["FindTickets"])
            {
                Btn_Tickets_Click(sender, e);
            }
            else
            {
                CloseActiveForm();
                await ShowButtonsToDefault();
                btnFilter.Visible = true;
                btnFindTickets.ForeColor = Color.FromArgb(0, 139, 255);
                await OpenChildForm(new FindTickets(_fullname, this, out _findTickets), _titleForForms["FindTickets"]);
            }
        }
        //Task button
        private async void Btn_Tasks_Click(object sender, EventArgs e)
        {
            await NormalPanelConf();
            CloseActiveForm();
            if (btnLeftThinButton_Problems.Visible == true)
            {
                await NotVisibleThinButton();
                btn_Tasks.ForeColor = Color.White;
                btn_Tasks.Image = Resources.problem;
                await TitleNameToDefault();
            }
            else
            {
                await NotVisibleThinButton();
                await ShowButtonsToDefault();
                btnAddTicket.Visible = true;
                btnLeftThinButton_Problems.Visible = true;
                btn_Tasks.ForeColor = Color.FromArgb(0, 139, 255);
                btn_Tasks.Image = Resources.problem_color;
                await OpenChildForm(new Tasks(_fullname, this, out _problems), _titleForForms["Problems"]);
            }
        }
        //User controls button
        private async void Btn_User_Click(object sender, EventArgs e)
        {
            await NormalPanelConf();
            CloseActiveForm();
            if (btnLeftThinButton_User.Visible == true)
            {
                await NotVisibleThinButton();
                btn_User.ForeColor = Color.White;
                btn_User.Image = Resources.user_icon;
                await TitleNameToDefault();
            }
            else
            {
                await NotVisibleThinButton();
                await ShowButtonsToDefault();
                btnAddTicket.Visible = true;
                btnLeftThinButton_User.Visible = true;
                btn_User.ForeColor = Color.FromArgb(0, 139, 255);
                btn_User.Image = Resources.color_user;
                await OpenChildForm(new Users(_fullname, this, out _users), _titleForForms["Users"]);
            }
        }
        #endregion
        #region Design
        //To control Menu Panel
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);
        //function to open menus
        private async void Main_Menu_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
            await HideFilterPanelWhenItIsUnused(e);
        }
        private async void PanelMain_MouseDown(object sender, MouseEventArgs e)
        {
            await HideFilterPanelWhenItIsUnused(e);
        }
        private async void PanelLeft_MouseDown(object sender, MouseEventArgs e)
        {
            await HideFilterPanelWhenItIsUnused(e);
        }
        private async Task HideFilterPanelWhenItIsUnused(MouseEventArgs e)
        {
            if (panelFilter.Visible && !panelFilter.Bounds.Contains(e.Location))
            {
                panelFilter.Visible = false; // Hide the panel
                btnFilter.Image = btnFilter.HoverState.Image;
            }
            await Task.Delay(1);
        }
        private async void PanelTop_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
            await HideFilterPanelWhenItIsUnused(e);
        }
        private async Task ForeColorForMainButtons()
        {
            btn_Dashboard.ForeColor = Color.White;
            btn_Tickets.ForeColor = Color.White;
            btn_Tasks.ForeColor = Color.White;
            btn_User.ForeColor = Color.White;
            btn_Dashboard.Image = Resources.dashboard;
            btn_Tickets.Image = Resources.ticket;
            btn_Tasks.Image = Resources.problem;
            btn_User.Image = Resources.user_icon;
            await Task.Delay(1);
        }
        private async Task ForeColorForTicketButtons()
        {
            btnOpenedTickets.ForeColor = Color.White;
            btnClosedTickets.ForeColor = Color.White;
            btnFindTickets.ForeColor = Color.White;
            await Task.Delay(1);
        }
        private async Task NormalPanelConf()
        {
            if (_userType == "Admin")
            {
                btn_Tasks.Visible = true;
                btn_User.Visible = true;
                btn_Tasks.Location = new Point(33, 258);
                btn_User.Location = new Point(35, 318);
            }
            btn_Dashboard.Visible = true;
            btn_Tickets.Visible = true;
            panelTickets.Visible = false;
            await ForeColorForMainButtons();
        }
        private async Task ExpandPanelConf()
        {
            if (_userType == "Admin")
            {
                btn_Tasks.Visible = true;
                btn_User.Visible = true;
                btn_Tasks.Location = new Point(33, 364);
                btn_User.Location = new Point(33, 424);
            }
            if (_userType == "User")
            {
                btnFindTickets.Visible = false;
                panelTickets.Size = new Size(201, 126);
            }
            panelTickets.Visible = true;
            btnLeftThinButton_Tickets.Visible = true;
            btn_Tickets.Visible = false;
            panelTickets.Location = new Point(33, 198);
            await Task.Delay(1);
        }
        private async Task NotVisibleThinButton()
        {
            btnLeftThinButton_Dashboard.Visible = false;
            btnLeftThinButton_Tickets.Visible = false;
            btnLeftThinButton_Problems.Visible = false;
            btnLeftThinButton_User.Visible = false;
            await Task.Delay(1);
        }
        #endregion
    }
}