namespace ServiceDesk.Forms
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.panelLeft = new System.Windows.Forms.Panel();
            this.btnFullname = new Guna.UI2.WinForms.Guna2Button();
            this.btnLeftThinButton_User = new Bunifu.Framework.UI.BunifuProgressBar();
            this.btnLeftThinButton_Problems = new Bunifu.Framework.UI.BunifuProgressBar();
            this.btnLeftThinButton_Tickets = new Bunifu.Framework.UI.BunifuProgressBar();
            this.btnLeftThinButton_Dashboard = new Bunifu.Framework.UI.BunifuProgressBar();
            this.progressbar_left = new Bunifu.Framework.UI.BunifuProgressBar();
            this.bunifuProgressBar1 = new Bunifu.Framework.UI.BunifuProgressBar();
            this.panelTickets = new System.Windows.Forms.Panel();
            this.btnFindTickets = new System.Windows.Forms.Button();
            this.btn_ColourTickets = new System.Windows.Forms.Button();
            this.btnClosedTickets = new System.Windows.Forms.Button();
            this.btnOpenedTickets = new System.Windows.Forms.Button();
            this.btn_User = new System.Windows.Forms.Button();
            this.lblShowTime = new System.Windows.Forms.Label();
            this.lblShowWatch = new System.Windows.Forms.Label();
            this.btn_Tasks = new System.Windows.Forms.Button();
            this.btn_Tickets = new System.Windows.Forms.Button();
            this.btn_Dashboard = new System.Windows.Forms.Button();
            this.lblTitle2 = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.Watch = new System.Windows.Forms.Timer(this.components);
            this.bunifuElipse_Main = new Bunifu.Framework.UI.BunifuElipse(this.components);
            this.lblFrom = new System.Windows.Forms.Label();
            this.panelFilter = new Guna.UI2.WinForms.Guna2CustomGradientPanel();
            this.btnOk = new Guna.UI2.WinForms.Guna2ImageButton();
            this.dateTo = new Guna.UI2.WinForms.Guna2DateTimePicker();
            this.dateFrom = new Guna.UI2.WinForms.Guna2DateTimePicker();
            this.lblTo = new System.Windows.Forms.Label();
            this.panelMain = new Guna.UI2.WinForms.Guna2Panel();
            this.picture = new Guna.UI2.WinForms.Guna2ImageButton();
            this.panelTop = new Guna.UI2.WinForms.Guna2Panel();
            this.btnFilter = new Guna.UI2.WinForms.Guna2ImageButton();
            this.lblTotalResult = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.txtSearch = new Guna.UI2.WinForms.Guna2TextBox();
            this.btnAddTicket = new Guna.UI2.WinForms.Guna2ImageButton();
            this.dateFiltering = new Guna.UI2.WinForms.Guna2ComboBox();
            this.lblHome = new System.Windows.Forms.Label();
            this.btnLogout = new Guna.UI2.WinForms.Guna2ImageButton();
            this.btnExit = new Guna.UI2.WinForms.Guna2ImageButton();
            this.btnMaximize = new Guna.UI2.WinForms.Guna2ImageButton();
            this.btnMinimize = new Guna.UI2.WinForms.Guna2ImageButton();
            this.guna2AnimateWindow = new Guna.UI2.WinForms.Guna2AnimateWindow(this.components);
            this.panelLeft.SuspendLayout();
            this.panelTickets.SuspendLayout();
            this.panelFilter.SuspendLayout();
            this.panelMain.SuspendLayout();
            this.panelTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelLeft
            // 
            this.panelLeft.BackColor = System.Drawing.Color.Black;
            this.panelLeft.Controls.Add(this.btnFullname);
            this.panelLeft.Controls.Add(this.btnLeftThinButton_User);
            this.panelLeft.Controls.Add(this.btnLeftThinButton_Problems);
            this.panelLeft.Controls.Add(this.btnLeftThinButton_Tickets);
            this.panelLeft.Controls.Add(this.btnLeftThinButton_Dashboard);
            this.panelLeft.Controls.Add(this.progressbar_left);
            this.panelLeft.Controls.Add(this.bunifuProgressBar1);
            this.panelLeft.Controls.Add(this.panelTickets);
            this.panelLeft.Controls.Add(this.btn_User);
            this.panelLeft.Controls.Add(this.lblShowTime);
            this.panelLeft.Controls.Add(this.lblShowWatch);
            this.panelLeft.Controls.Add(this.btn_Tasks);
            this.panelLeft.Controls.Add(this.btn_Tickets);
            this.panelLeft.Controls.Add(this.btn_Dashboard);
            this.panelLeft.Controls.Add(this.lblTitle2);
            this.panelLeft.Controls.Add(this.lblTitle);
            this.panelLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelLeft.Location = new System.Drawing.Point(0, 0);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(258, 800);
            this.panelLeft.TabIndex = 0;
            this.panelLeft.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PanelLeft_MouseDown);
            // 
            // Watch
            // 
            this.Watch.Enabled = true;
            this.Watch.Tick += new System.EventHandler(this.Watch_Tick);
            // 
            // btnFullname
            // 
            this.btnFullname.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnFullname.Animated = true;
            this.btnFullname.AutoRoundedCorners = true;
            this.btnFullname.BackColor = System.Drawing.Color.Transparent;
            this.btnFullname.BorderRadius = 14;
            this.btnFullname.DefaultAutoSize = true;
            this.btnFullname.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnFullname.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnFullname.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnFullname.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnFullname.FillColor = System.Drawing.Color.Black;
            this.btnFullname.Font = new System.Drawing.Font("Inter", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFullname.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(133)))), ((int)(((byte)(255)))));
            this.btnFullname.Image = ((System.Drawing.Image)(resources.GetObject("btnFullname.Image")));
            this.btnFullname.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnFullname.ImageOffset = new System.Drawing.Point(-12, 0);
            this.btnFullname.ImageSize = new System.Drawing.Size(32, 32);
            this.btnFullname.Location = new System.Drawing.Point(21, 693);
            this.btnFullname.Name = "btnFullname";
            this.btnFullname.Size = new System.Drawing.Size(138, 31);
            this.btnFullname.TabIndex = 42;
            this.btnFullname.Text = "Full name";
            this.btnFullname.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnFullname.TextOffset = new System.Drawing.Point(-18, 0);
            // 
            // btnLeftThinButton_User
            // 
            this.btnLeftThinButton_User.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(133)))), ((int)(((byte)(255)))));
            this.btnLeftThinButton_User.BorderRadius = 5;
            this.btnLeftThinButton_User.Location = new System.Drawing.Point(21, 322);
            this.btnLeftThinButton_User.Margin = new System.Windows.Forms.Padding(32, 43, 32, 43);
            this.btnLeftThinButton_User.MaximumValue = 100;
            this.btnLeftThinButton_User.Name = "btnLeftThinButton_User";
            this.btnLeftThinButton_User.ProgressColor = System.Drawing.Color.Teal;
            this.btnLeftThinButton_User.Size = new System.Drawing.Size(6, 35);
            this.btnLeftThinButton_User.TabIndex = 41;
            this.btnLeftThinButton_User.Value = 0;
            this.btnLeftThinButton_User.Visible = false;
            // 
            // btnLeftThinButton_Problems
            // 
            this.btnLeftThinButton_Problems.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(133)))), ((int)(((byte)(255)))));
            this.btnLeftThinButton_Problems.BorderRadius = 5;
            this.btnLeftThinButton_Problems.Location = new System.Drawing.Point(21, 262);
            this.btnLeftThinButton_Problems.Margin = new System.Windows.Forms.Padding(21, 28, 21, 28);
            this.btnLeftThinButton_Problems.MaximumValue = 100;
            this.btnLeftThinButton_Problems.Name = "btnLeftThinButton_Problems";
            this.btnLeftThinButton_Problems.ProgressColor = System.Drawing.Color.Teal;
            this.btnLeftThinButton_Problems.Size = new System.Drawing.Size(6, 35);
            this.btnLeftThinButton_Problems.TabIndex = 40;
            this.btnLeftThinButton_Problems.Value = 0;
            this.btnLeftThinButton_Problems.Visible = false;
            // 
            // btnLeftThinButton_Tickets
            // 
            this.btnLeftThinButton_Tickets.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(133)))), ((int)(((byte)(255)))));
            this.btnLeftThinButton_Tickets.BorderRadius = 5;
            this.btnLeftThinButton_Tickets.Location = new System.Drawing.Point(21, 202);
            this.btnLeftThinButton_Tickets.Margin = new System.Windows.Forms.Padding(14, 18, 14, 18);
            this.btnLeftThinButton_Tickets.MaximumValue = 100;
            this.btnLeftThinButton_Tickets.Name = "btnLeftThinButton_Tickets";
            this.btnLeftThinButton_Tickets.ProgressColor = System.Drawing.Color.Teal;
            this.btnLeftThinButton_Tickets.Size = new System.Drawing.Size(6, 35);
            this.btnLeftThinButton_Tickets.TabIndex = 39;
            this.btnLeftThinButton_Tickets.Value = 0;
            this.btnLeftThinButton_Tickets.Visible = false;
            // 
            // btnLeftThinButton_Dashboard
            // 
            this.btnLeftThinButton_Dashboard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(133)))), ((int)(((byte)(255)))));
            this.btnLeftThinButton_Dashboard.BorderRadius = 5;
            this.btnLeftThinButton_Dashboard.Location = new System.Drawing.Point(21, 142);
            this.btnLeftThinButton_Dashboard.Margin = new System.Windows.Forms.Padding(9, 12, 9, 12);
            this.btnLeftThinButton_Dashboard.MaximumValue = 100;
            this.btnLeftThinButton_Dashboard.Name = "btnLeftThinButton_Dashboard";
            this.btnLeftThinButton_Dashboard.ProgressColor = System.Drawing.Color.Teal;
            this.btnLeftThinButton_Dashboard.Size = new System.Drawing.Size(6, 35);
            this.btnLeftThinButton_Dashboard.TabIndex = 38;
            this.btnLeftThinButton_Dashboard.Value = 0;
            this.btnLeftThinButton_Dashboard.Visible = false;
            // 
            // progressbar_left
            // 
            this.progressbar_left.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(133)))), ((int)(((byte)(255)))));
            this.progressbar_left.BorderRadius = 5;
            this.progressbar_left.Location = new System.Drawing.Point(21, 28);
            this.progressbar_left.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.progressbar_left.MaximumValue = 100;
            this.progressbar_left.Name = "progressbar_left";
            this.progressbar_left.ProgressColor = System.Drawing.Color.Teal;
            this.progressbar_left.Size = new System.Drawing.Size(4, 80);
            this.progressbar_left.TabIndex = 37;
            this.progressbar_left.Value = 0;
            // 
            // bunifuProgressBar1
            // 
            this.bunifuProgressBar1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.bunifuProgressBar1.BorderRadius = 5;
            this.bunifuProgressBar1.Location = new System.Drawing.Point(14, 118);
            this.bunifuProgressBar1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.bunifuProgressBar1.MaximumValue = 100;
            this.bunifuProgressBar1.Name = "bunifuProgressBar1";
            this.bunifuProgressBar1.ProgressColor = System.Drawing.Color.Teal;
            this.bunifuProgressBar1.Size = new System.Drawing.Size(222, 2);
            this.bunifuProgressBar1.TabIndex = 36;
            this.bunifuProgressBar1.Value = 0;
            // 
            // panelTickets
            // 
            this.panelTickets.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.panelTickets.Controls.Add(this.btnFindTickets);
            this.panelTickets.Controls.Add(this.btn_ColourTickets);
            this.panelTickets.Controls.Add(this.btnClosedTickets);
            this.panelTickets.Controls.Add(this.btnOpenedTickets);
            this.panelTickets.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F);
            this.panelTickets.Location = new System.Drawing.Point(21, 384);
            this.panelTickets.Name = "panelTickets";
            this.panelTickets.Size = new System.Drawing.Size(201, 145);
            this.panelTickets.TabIndex = 34;
            this.panelTickets.Visible = false;
            // 
            // btnFindTickets
            // 
            this.btnFindTickets.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFindTickets.AutoEllipsis = true;
            this.btnFindTickets.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFindTickets.FlatAppearance.BorderSize = 0;
            this.btnFindTickets.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnFindTickets.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFindTickets.Font = new System.Drawing.Font("Inter SemiBold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFindTickets.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(163)))), ((int)(((byte)(163)))), ((int)(((byte)(163)))));
            this.btnFindTickets.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnFindTickets.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnFindTickets.Location = new System.Drawing.Point(13, 106);
            this.btnFindTickets.Name = "btnFindTickets";
            this.btnFindTickets.Size = new System.Drawing.Size(168, 28);
            this.btnFindTickets.TabIndex = 15;
            this.btnFindTickets.Text = "Find tickets";
            this.btnFindTickets.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnFindTickets.UseVisualStyleBackColor = false;
            this.btnFindTickets.Click += new System.EventHandler(this.BtnFindTickets_Click);
            // 
            // btn_ColourTickets
            // 
            this.btn_ColourTickets.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_ColourTickets.AutoEllipsis = true;
            this.btn_ColourTickets.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_ColourTickets.FlatAppearance.BorderSize = 0;
            this.btn_ColourTickets.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btn_ColourTickets.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btn_ColourTickets.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_ColourTickets.Font = new System.Drawing.Font("Inter", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_ColourTickets.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(133)))), ((int)(((byte)(255)))));
            this.btn_ColourTickets.Image = ((System.Drawing.Image)(resources.GetObject("btn_ColourTickets.Image")));
            this.btn_ColourTickets.ImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.btn_ColourTickets.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_ColourTickets.Location = new System.Drawing.Point(13, 3);
            this.btn_ColourTickets.Name = "btn_ColourTickets";
            this.btn_ColourTickets.Size = new System.Drawing.Size(168, 39);
            this.btn_ColourTickets.TabIndex = 14;
            this.btn_ColourTickets.Text = " Tickets";
            this.btn_ColourTickets.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btn_ColourTickets.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btn_ColourTickets.UseVisualStyleBackColor = true;
            this.btn_ColourTickets.Click += new System.EventHandler(this.Btn_ColourTickets_Click);
            // 
            // btnClosedTickets
            // 
            this.btnClosedTickets.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClosedTickets.AutoEllipsis = true;
            this.btnClosedTickets.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClosedTickets.FlatAppearance.BorderSize = 0;
            this.btnClosedTickets.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnClosedTickets.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClosedTickets.Font = new System.Drawing.Font("Inter SemiBold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClosedTickets.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(163)))), ((int)(((byte)(163)))), ((int)(((byte)(163)))));
            this.btnClosedTickets.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClosedTickets.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnClosedTickets.Location = new System.Drawing.Point(13, 72);
            this.btnClosedTickets.Name = "btnClosedTickets";
            this.btnClosedTickets.Size = new System.Drawing.Size(168, 28);
            this.btnClosedTickets.TabIndex = 9;
            this.btnClosedTickets.Text = "Closed tickets";
            this.btnClosedTickets.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClosedTickets.UseVisualStyleBackColor = false;
            this.btnClosedTickets.Click += new System.EventHandler(this.BtnClosedTickets_Click);
            // 
            // btnOpenedTickets
            // 
            this.btnOpenedTickets.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpenedTickets.AutoEllipsis = true;
            this.btnOpenedTickets.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOpenedTickets.FlatAppearance.BorderSize = 0;
            this.btnOpenedTickets.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnOpenedTickets.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpenedTickets.Font = new System.Drawing.Font("Inter SemiBold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOpenedTickets.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(163)))), ((int)(((byte)(163)))), ((int)(((byte)(163)))));
            this.btnOpenedTickets.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOpenedTickets.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnOpenedTickets.Location = new System.Drawing.Point(13, 40);
            this.btnOpenedTickets.Name = "btnOpenedTickets";
            this.btnOpenedTickets.Size = new System.Drawing.Size(168, 28);
            this.btnOpenedTickets.TabIndex = 7;
            this.btnOpenedTickets.Text = "Open tickets";
            this.btnOpenedTickets.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOpenedTickets.UseVisualStyleBackColor = false;
            this.btnOpenedTickets.Click += new System.EventHandler(this.BtnOpenTickets_Click);
            // 
            // btn_User
            // 
            this.btn_User.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_User.AutoEllipsis = true;
            this.btn_User.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_User.FlatAppearance.BorderSize = 0;
            this.btn_User.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(139)))), ((int)(((byte)(255)))));
            this.btn_User.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.btn_User.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_User.Font = new System.Drawing.Font("Inter SemiBold", 12F, System.Drawing.FontStyle.Bold);
            this.btn_User.ForeColor = System.Drawing.Color.White;
            this.btn_User.Image = ((System.Drawing.Image)(resources.GetObject("btn_User.Image")));
            this.btn_User.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btn_User.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_User.Location = new System.Drawing.Point(33, 318);
            this.btn_User.Name = "btn_User";
            this.btn_User.Size = new System.Drawing.Size(207, 39);
            this.btn_User.TabIndex = 33;
            this.btn_User.Text = "  User";
            this.btn_User.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btn_User.UseVisualStyleBackColor = true;
            this.btn_User.Click += new System.EventHandler(this.Btn_User_Click);
            // 
            // lblShowTime
            // 
            this.lblShowTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblShowTime.AutoSize = true;
            this.lblShowTime.Font = new System.Drawing.Font("Inter", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblShowTime.ForeColor = System.Drawing.Color.White;
            this.lblShowTime.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblShowTime.Location = new System.Drawing.Point(17, 753);
            this.lblShowTime.Name = "lblShowTime";
            this.lblShowTime.Size = new System.Drawing.Size(56, 23);
            this.lblShowTime.TabIndex = 28;
            this.lblShowTime.Text = "Time";
            // 
            // lblShowWatch
            // 
            this.lblShowWatch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblShowWatch.AutoSize = true;
            this.lblShowWatch.Font = new System.Drawing.Font("Inter", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblShowWatch.ForeColor = System.Drawing.Color.White;
            this.lblShowWatch.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblShowWatch.Location = new System.Drawing.Point(152, 753);
            this.lblShowWatch.Name = "lblShowWatch";
            this.lblShowWatch.Size = new System.Drawing.Size(70, 23);
            this.lblShowWatch.TabIndex = 27;
            this.lblShowWatch.Text = "Watch";
            // 
            // btn_Tasks
            // 
            this.btn_Tasks.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Tasks.AutoEllipsis = true;
            this.btn_Tasks.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Tasks.FlatAppearance.BorderSize = 0;
            this.btn_Tasks.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(139)))), ((int)(((byte)(255)))));
            this.btn_Tasks.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.btn_Tasks.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Tasks.Font = new System.Drawing.Font("Inter SemiBold", 12F, System.Drawing.FontStyle.Bold);
            this.btn_Tasks.ForeColor = System.Drawing.Color.White;
            this.btn_Tasks.Image = ((System.Drawing.Image)(resources.GetObject("btn_Tasks.Image")));
            this.btn_Tasks.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btn_Tasks.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Tasks.Location = new System.Drawing.Point(33, 258);
            this.btn_Tasks.Name = "btn_Tasks";
            this.btn_Tasks.Size = new System.Drawing.Size(207, 39);
            this.btn_Tasks.TabIndex = 20;
            this.btn_Tasks.Text = "  Tasks";
            this.btn_Tasks.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btn_Tasks.UseVisualStyleBackColor = true;
            this.btn_Tasks.Click += new System.EventHandler(this.Btn_Tasks_Click);
            // 
            // btn_Tickets
            // 
            this.btn_Tickets.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Tickets.AutoEllipsis = true;
            this.btn_Tickets.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Tickets.FlatAppearance.BorderSize = 0;
            this.btn_Tickets.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(139)))), ((int)(((byte)(255)))));
            this.btn_Tickets.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.btn_Tickets.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Tickets.Font = new System.Drawing.Font("Inter SemiBold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Tickets.ForeColor = System.Drawing.Color.White;
            this.btn_Tickets.Image = ((System.Drawing.Image)(resources.GetObject("btn_Tickets.Image")));
            this.btn_Tickets.ImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.btn_Tickets.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Tickets.Location = new System.Drawing.Point(33, 198);
            this.btn_Tickets.Name = "btn_Tickets";
            this.btn_Tickets.Size = new System.Drawing.Size(211, 39);
            this.btn_Tickets.TabIndex = 13;
            this.btn_Tickets.Text = "  Tickets";
            this.btn_Tickets.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btn_Tickets.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btn_Tickets.UseVisualStyleBackColor = false;
            this.btn_Tickets.Click += new System.EventHandler(this.Btn_Tickets_Click);
            // 
            // btn_Dashboard
            // 
            this.btn_Dashboard.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Dashboard.AutoEllipsis = true;
            this.btn_Dashboard.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Dashboard.FlatAppearance.BorderSize = 0;
            this.btn_Dashboard.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(139)))), ((int)(((byte)(255)))));
            this.btn_Dashboard.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.btn_Dashboard.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Dashboard.Font = new System.Drawing.Font("Inter SemiBold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Dashboard.ForeColor = System.Drawing.Color.White;
            this.btn_Dashboard.Image = ((System.Drawing.Image)(resources.GetObject("btn_Dashboard.Image")));
            this.btn_Dashboard.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btn_Dashboard.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Dashboard.Location = new System.Drawing.Point(33, 138);
            this.btn_Dashboard.Name = "btn_Dashboard";
            this.btn_Dashboard.Size = new System.Drawing.Size(207, 39);
            this.btn_Dashboard.TabIndex = 11;
            this.btn_Dashboard.Text = "  Dashboard";
            this.btn_Dashboard.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btn_Dashboard.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btn_Dashboard.UseVisualStyleBackColor = true;
            this.btn_Dashboard.Click += new System.EventHandler(this.Btn_Dashboard_Click);
            // 
            // lblTitle2
            // 
            this.lblTitle2.AutoSize = true;
            this.lblTitle2.Font = new System.Drawing.Font("Inter", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle2.ForeColor = System.Drawing.Color.White;
            this.lblTitle2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblTitle2.Location = new System.Drawing.Point(34, 65);
            this.lblTitle2.Name = "lblTitle2";
            this.lblTitle2.Size = new System.Drawing.Size(88, 33);
            this.lblTitle2.TabIndex = 8;
            this.lblTitle2.Text = "Desk ";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Inter", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblTitle.Location = new System.Drawing.Point(34, 34);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(115, 33);
            this.lblTitle.TabIndex = 7;
            this.lblTitle.Text = "Service";
            // 
            
            // bunifuElipse_Main
            // 
            this.bunifuElipse_Main.ElipseRadius = 40;
            this.bunifuElipse_Main.TargetControl = this;
            // 
            // lblFrom
            // 
            this.lblFrom.AutoSize = true;
            this.lblFrom.Font = new System.Drawing.Font("Inter", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFrom.ForeColor = System.Drawing.Color.Black;
            this.lblFrom.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblFrom.Location = new System.Drawing.Point(3, 13);
            this.lblFrom.Name = "lblFrom";
            this.lblFrom.Size = new System.Drawing.Size(48, 20);
            this.lblFrom.TabIndex = 38;
            this.lblFrom.Text = "From";
            // 
            // panelFilter
            // 
            this.panelFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panelFilter.BackColor = System.Drawing.Color.Transparent;
            this.panelFilter.BorderColor = System.Drawing.Color.Black;
            this.panelFilter.BorderRadius = 10;
            this.panelFilter.Controls.Add(this.btnOk);
            this.panelFilter.Controls.Add(this.dateTo);
            this.panelFilter.Controls.Add(this.dateFrom);
            this.panelFilter.Controls.Add(this.lblTo);
            this.panelFilter.Controls.Add(this.lblFrom);
            this.panelFilter.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(244)))));
            this.panelFilter.FillColor2 = System.Drawing.Color.LightSteelBlue;
            this.panelFilter.FillColor4 = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(244)))));
            this.panelFilter.Location = new System.Drawing.Point(827, 73);
            this.panelFilter.Name = "panelFilter";
            this.panelFilter.ShadowDecoration.BorderRadius = 30;
            this.panelFilter.Size = new System.Drawing.Size(173, 173);
            this.panelFilter.TabIndex = 39;
            this.panelFilter.Visible = false;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.AnimatedGIF = true;
            this.btnOk.CheckedState.ImageSize = new System.Drawing.Size(64, 64);
            this.btnOk.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOk.HoverState.ImageSize = new System.Drawing.Size(16, 16);
            this.btnOk.Image = ((System.Drawing.Image)(resources.GetObject("btnOk.Image")));
            this.btnOk.ImageOffset = new System.Drawing.Point(0, 0);
            this.btnOk.ImageRotate = 0F;
            this.btnOk.ImageSize = new System.Drawing.Size(16, 16);
            this.btnOk.Location = new System.Drawing.Point(129, 143);
            this.btnOk.Name = "btnOk";
            this.btnOk.PressedState.ImageSize = new System.Drawing.Size(16, 16);
            this.btnOk.Size = new System.Drawing.Size(30, 27);
            this.btnOk.TabIndex = 42;
            this.btnOk.Click += new System.EventHandler(this.BtnOk_Click);
            // 
            // dateTo
            // 
            this.dateTo.Animated = true;
            this.dateTo.BorderRadius = 6;
            this.dateTo.Checked = true;
            this.dateTo.CustomFormat = "yyyy-MM-dd";
            this.dateTo.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(242)))), ((int)(((byte)(246)))));
            this.dateTo.Font = new System.Drawing.Font("Inter", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dateTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTo.Location = new System.Drawing.Point(7, 116);
            this.dateTo.MaxDate = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
            this.dateTo.MinDate = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            this.dateTo.Name = "dateTo";
            this.dateTo.Size = new System.Drawing.Size(152, 24);
            this.dateTo.TabIndex = 41;
            this.dateTo.UseTransparentBackground = true;
            this.dateTo.Value = new System.DateTime(2024, 12, 20, 0, 0, 0, 0);
            // 
            // dateFrom
            // 
            this.dateFrom.Animated = true;
            this.dateFrom.BorderRadius = 6;
            this.dateFrom.Checked = true;
            this.dateFrom.CustomFormat = "yyyy-MM-dd";
            this.dateFrom.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(242)))), ((int)(((byte)(246)))));
            this.dateFrom.Font = new System.Drawing.Font("Inter", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dateFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateFrom.Location = new System.Drawing.Point(7, 45);
            this.dateFrom.MaxDate = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
            this.dateFrom.MinDate = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            this.dateFrom.Name = "dateFrom";
            this.dateFrom.Size = new System.Drawing.Size(152, 24);
            this.dateFrom.TabIndex = 40;
            this.dateFrom.UseTransparentBackground = true;
            this.dateFrom.Value = new System.DateTime(2024, 12, 20, 0, 0, 0, 0);
            // 
            // lblTo
            // 
            this.lblTo.AutoSize = true;
            this.lblTo.Font = new System.Drawing.Font("Inter", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTo.ForeColor = System.Drawing.Color.Black;
            this.lblTo.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblTo.Location = new System.Drawing.Point(3, 84);
            this.lblTo.Name = "lblTo";
            this.lblTo.Size = new System.Drawing.Size(28, 20);
            this.lblTo.TabIndex = 39;
            this.lblTo.Text = "To";
            // 
            // panelMain
            // 
            this.panelMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelMain.BorderRadius = 25;
            this.panelMain.Controls.Add(this.picture);
            this.panelMain.FillColor = System.Drawing.Color.White;
            this.panelMain.Font = new System.Drawing.Font("Inter", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelMain.Location = new System.Drawing.Point(289, 86);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(1082, 690);
            this.panelMain.TabIndex = 37;
            this.panelMain.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PanelMain_MouseDown);
            // 
            // picture
            // 
            this.picture.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.picture.AnimatedGIF = true;
            this.picture.BackColor = System.Drawing.Color.White;
            this.picture.CheckedState.ImageSize = new System.Drawing.Size(64, 64);
            this.picture.HoverState.ImageSize = new System.Drawing.Size(96, 96);
            this.picture.Image = ((System.Drawing.Image)(resources.GetObject("picture.Image")));
            this.picture.ImageOffset = new System.Drawing.Point(0, 0);
            this.picture.ImageRotate = 0F;
            this.picture.Location = new System.Drawing.Point(471, 273);
            this.picture.Name = "picture";
            this.picture.PressedState.ImageSize = new System.Drawing.Size(64, 64);
            this.picture.Size = new System.Drawing.Size(133, 103);
            this.picture.TabIndex = 38;
            // 
            // panelTop
            // 
            this.panelTop.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelTop.BorderRadius = 25;
            this.panelTop.Controls.Add(this.btnFilter);
            this.panelTop.Controls.Add(this.lblTotalResult);
            this.panelTop.Controls.Add(this.lblTotal);
            this.panelTop.Controls.Add(this.txtSearch);
            this.panelTop.Controls.Add(this.btnAddTicket);
            this.panelTop.Controls.Add(this.dateFiltering);
            this.panelTop.Controls.Add(this.lblHome);
            this.panelTop.Controls.Add(this.btnLogout);
            this.panelTop.FillColor = System.Drawing.Color.White;
            this.panelTop.Font = new System.Drawing.Font("Inter", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelTop.Location = new System.Drawing.Point(289, 12);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(950, 55);
            this.panelTop.TabIndex = 40;
            this.panelTop.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PanelTop_MouseDown);
            // 
            // btnFilter
            // 
            this.btnFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFilter.BackColor = System.Drawing.Color.Transparent;
            this.btnFilter.HoverState.Image = ((System.Drawing.Image)(resources.GetObject("resource.Image6")));
            this.btnFilter.HoverState.ImageSize = new System.Drawing.Size(24, 24);
            this.btnFilter.Image = ((System.Drawing.Image)(resources.GetObject("btnFilter.Image")));
            this.btnFilter.ImageOffset = new System.Drawing.Point(0, 0);
            this.btnFilter.ImageRotate = 0F;
            this.btnFilter.ImageSize = new System.Drawing.Size(24, 24);
            this.btnFilter.Location = new System.Drawing.Point(600, 8);
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.PressedState.Image = ((System.Drawing.Image)(resources.GetObject("resource.Image7")));
            this.btnFilter.PressedState.ImageSize = new System.Drawing.Size(24, 24);
            this.btnFilter.Size = new System.Drawing.Size(31, 35);
            this.btnFilter.TabIndex = 92;
            this.btnFilter.UseTransparentBackground = true;
            this.btnFilter.Visible = false;
            this.btnFilter.Click += new System.EventHandler(this.BtnFilter_Click);
            // 
            // lblTotalResult
            // 
            this.lblTotalResult.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTotalResult.AutoSize = true;
            this.lblTotalResult.BackColor = System.Drawing.Color.Transparent;
            this.lblTotalResult.Font = new System.Drawing.Font("Inter", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalResult.ForeColor = System.Drawing.Color.Black;
            this.lblTotalResult.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblTotalResult.Location = new System.Drawing.Point(543, 15);
            this.lblTotalResult.Name = "lblTotalResult";
            this.lblTotalResult.Size = new System.Drawing.Size(0, 23);
            this.lblTotalResult.TabIndex = 91;
            this.lblTotalResult.Visible = false;
            // 
            // lblTotal
            // 
            this.lblTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTotal.AutoSize = true;
            this.lblTotal.BackColor = System.Drawing.Color.Transparent;
            this.lblTotal.Font = new System.Drawing.Font("Inter", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotal.ForeColor = System.Drawing.Color.Black;
            this.lblTotal.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblTotal.Location = new System.Drawing.Point(469, 15);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(68, 23);
            this.lblTotal.TabIndex = 90;
            this.lblTotal.Text = "Total :";
            this.lblTotal.Visible = false;
            // 
            // txtSearch
            // 
            this.txtSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearch.Animated = true;
            this.txtSearch.AutoSize = true;
            this.txtSearch.BackColor = System.Drawing.Color.Transparent;
            this.txtSearch.BorderRadius = 6;
            this.txtSearch.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtSearch.DefaultText = "";
            this.txtSearch.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtSearch.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtSearch.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtSearch.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtSearch.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtSearch.Font = new System.Drawing.Font("Inter", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSearch.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtSearch.IconRight = ((System.Drawing.Image)(resources.GetObject("txtSearch.IconRight")));
            this.txtSearch.IconRightOffset = new System.Drawing.Point(4, 0);
            this.txtSearch.Location = new System.Drawing.Point(638, 8);
            this.txtSearch.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.PasswordChar = '\0';
            this.txtSearch.PlaceholderText = "Search in ...";
            this.txtSearch.SelectedText = "";
            this.txtSearch.Size = new System.Drawing.Size(216, 35);
            this.txtSearch.TabIndex = 89;
            this.txtSearch.Visible = false;
            this.txtSearch.TextChanged += new System.EventHandler(this.TxtSearch_TextChanged);
            // 
            // btnAddTicket
            // 
            this.btnAddTicket.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddTicket.AnimatedGIF = true;
            this.btnAddTicket.BackColor = System.Drawing.Color.Transparent;
            this.btnAddTicket.CheckedState.ImageSize = new System.Drawing.Size(40, 40);
            this.btnAddTicket.HoverState.Image = ((System.Drawing.Image)(resources.GetObject("resource.Image8")));
            this.btnAddTicket.HoverState.ImageSize = new System.Drawing.Size(32, 32);
            this.btnAddTicket.Image = ((System.Drawing.Image)(resources.GetObject("btnAddTicket.Image")));
            this.btnAddTicket.ImageOffset = new System.Drawing.Point(0, 0);
            this.btnAddTicket.ImageRotate = 0F;
            this.btnAddTicket.ImageSize = new System.Drawing.Size(32, 32);
            this.btnAddTicket.Location = new System.Drawing.Point(399, 13);
            this.btnAddTicket.Name = "btnAddTicket";
            this.btnAddTicket.PressedState.Image = ((System.Drawing.Image)(resources.GetObject("resource.Image9")));
            this.btnAddTicket.PressedState.ImageSize = new System.Drawing.Size(32, 32);
            this.btnAddTicket.Size = new System.Drawing.Size(52, 32);
            this.btnAddTicket.TabIndex = 88;
            this.btnAddTicket.Visible = false;
            this.btnAddTicket.Click += new System.EventHandler(this.BtnAddTicket_Click);
            // 
            // dateFiltering
            // 
            this.dateFiltering.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.dateFiltering.BackColor = System.Drawing.Color.Transparent;
            this.dateFiltering.BorderColor = System.Drawing.Color.White;
            this.dateFiltering.Cursor = System.Windows.Forms.Cursors.Default;
            this.dateFiltering.DisplayMember = "daily";
            this.dateFiltering.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.dateFiltering.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.dateFiltering.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.dateFiltering.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.dateFiltering.Font = new System.Drawing.Font("Inter Medium", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dateFiltering.ForeColor = System.Drawing.Color.Black;
            this.dateFiltering.FormattingEnabled = true;
            this.dateFiltering.ItemHeight = 25;
            this.dateFiltering.Items.AddRange(new object[] {
            "daily",
            "weekly",
            "monthly"});
            this.dateFiltering.Location = new System.Drawing.Point(740, 12);
            this.dateFiltering.MaxDropDownItems = 3;
            this.dateFiltering.Name = "dateFiltering";
            this.dateFiltering.Size = new System.Drawing.Size(114, 31);
            this.dateFiltering.StartIndex = 0;
            this.dateFiltering.TabIndex = 87;
            this.dateFiltering.TextTransform = Guna.UI2.WinForms.Enums.TextTransform.LowerCase;
            this.dateFiltering.Visible = false;
            this.dateFiltering.SelectedIndexChanged += new System.EventHandler(this.DateFiltering_SelectedIndexChanged);
            // 
            // lblHome
            // 
            this.lblHome.AutoSize = true;
            this.lblHome.BackColor = System.Drawing.Color.Transparent;
            this.lblHome.Font = new System.Drawing.Font("Inter SemiBold", 15.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHome.ForeColor = System.Drawing.Color.Black;
            this.lblHome.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblHome.Location = new System.Drawing.Point(17, 15);
            this.lblHome.Name = "lblHome";
            this.lblHome.Size = new System.Drawing.Size(72, 25);
            this.lblHome.TabIndex = 85;
            this.lblHome.Text = "Home";
            // 
            // btnLogout
            // 
            this.btnLogout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLogout.AnimatedGIF = true;
            this.btnLogout.BackColor = System.Drawing.Color.Transparent;
            this.btnLogout.CheckedState.ImageSize = new System.Drawing.Size(24, 24);
            this.btnLogout.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLogout.HoverState.ImageSize = new System.Drawing.Size(24, 24);
            this.btnLogout.Image = ((System.Drawing.Image)(resources.GetObject("btnLogout.Image")));
            this.btnLogout.ImageOffset = new System.Drawing.Point(0, 0);
            this.btnLogout.ImageRotate = 0F;
            this.btnLogout.ImageSize = new System.Drawing.Size(28, 28);
            this.btnLogout.Location = new System.Drawing.Point(900, 5);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.PressedState.ImageSize = new System.Drawing.Size(28, 28);
            this.btnLogout.Size = new System.Drawing.Size(34, 38);
            this.btnLogout.TabIndex = 86;
            this.btnLogout.UseTransparentBackground = true;
            this.btnLogout.Click += new System.EventHandler(this.BtnLogOut_Click);
            // 
            // btnExit
            // 
            this.btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExit.BackColor = System.Drawing.Color.Transparent;
            this.btnExit.HoverState.Image = ((System.Drawing.Image)(resources.GetObject("resource.Image")));
            this.btnExit.HoverState.ImageSize = new System.Drawing.Size(30, 30);
            this.btnExit.Image = ((System.Drawing.Image)(resources.GetObject("btnExit.Image")));
            this.btnExit.ImageOffset = new System.Drawing.Point(0, 0);
            this.btnExit.ImageRotate = 0F;
            this.btnExit.ImageSize = new System.Drawing.Size(30, 30);
            this.btnExit.Location = new System.Drawing.Point(1336, 18);
            this.btnExit.Name = "btnExit";
            this.btnExit.PressedState.Image = ((System.Drawing.Image)(resources.GetObject("resource.Image1")));
            this.btnExit.PressedState.ImageSize = new System.Drawing.Size(30, 30);
            this.btnExit.Size = new System.Drawing.Size(30, 30);
            this.btnExit.TabIndex = 95;
            this.btnExit.UseTransparentBackground = true;
            this.btnExit.Click += new System.EventHandler(this.BtnExit_Click);
            // 
            // btnMaximize
            // 
            this.btnMaximize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMaximize.BackColor = System.Drawing.Color.Transparent;
            this.btnMaximize.HoverState.Image = ((System.Drawing.Image)(resources.GetObject("resource.Image2")));
            this.btnMaximize.HoverState.ImageSize = new System.Drawing.Size(24, 24);
            this.btnMaximize.Image = ((System.Drawing.Image)(resources.GetObject("btnMaximize.Image")));
            this.btnMaximize.ImageOffset = new System.Drawing.Point(0, 0);
            this.btnMaximize.ImageRotate = 0F;
            this.btnMaximize.ImageSize = new System.Drawing.Size(24, 24);
            this.btnMaximize.Location = new System.Drawing.Point(1300, 18);
            this.btnMaximize.Name = "btnMaximize";
            this.btnMaximize.PressedState.Image = ((System.Drawing.Image)(resources.GetObject("resource.Image3")));
            this.btnMaximize.PressedState.ImageSize = new System.Drawing.Size(24, 24);
            this.btnMaximize.Size = new System.Drawing.Size(30, 30);
            this.btnMaximize.TabIndex = 94;
            this.btnMaximize.UseTransparentBackground = true;
            this.btnMaximize.Click += new System.EventHandler(this.BtnMaximize_Click);
            // 
            // btnMinimize
            // 
            this.btnMinimize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMinimize.BackColor = System.Drawing.Color.Transparent;
            this.btnMinimize.HoverState.Image = ((System.Drawing.Image)(resources.GetObject("resource.Image4")));
            this.btnMinimize.HoverState.ImageSize = new System.Drawing.Size(24, 24);
            this.btnMinimize.Image = ((System.Drawing.Image)(resources.GetObject("btnMinimize.Image")));
            this.btnMinimize.ImageOffset = new System.Drawing.Point(0, 0);
            this.btnMinimize.ImageRotate = 0F;
            this.btnMinimize.ImageSize = new System.Drawing.Size(24, 24);
            this.btnMinimize.Location = new System.Drawing.Point(1264, 18);
            this.btnMinimize.Name = "btnMinimize";
            this.btnMinimize.PressedState.Image = ((System.Drawing.Image)(resources.GetObject("resource.Image5")));
            this.btnMinimize.PressedState.ImageSize = new System.Drawing.Size(24, 24);
            this.btnMinimize.Size = new System.Drawing.Size(30, 30);
            this.btnMinimize.TabIndex = 93;
            this.btnMinimize.UseTransparentBackground = true;
            this.btnMinimize.Click += new System.EventHandler(this.BtnMinimize_Click);
            // 
            // guna2AnimateWindow
            // 
            this.guna2AnimateWindow.Interval = 200;
            this.guna2AnimateWindow.TargetForm = this;
            // 
            // Main
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(244)))));
            this.ClientSize = new System.Drawing.Size(1400, 800);
            this.ControlBox = false;
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnMaximize);
            this.Controls.Add(this.btnMinimize);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.panelFilter);
            this.Controls.Add(this.panelLeft);
            this.Controls.Add(this.panelMain);
            this.Font = new System.Drawing.Font("Inter", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Main_Menu_MouseDown);
            this.panelLeft.ResumeLayout(false);
            this.panelLeft.PerformLayout();
            this.panelTickets.ResumeLayout(false);
            this.panelFilter.ResumeLayout(false);
            this.panelFilter.PerformLayout();
            this.panelMain.ResumeLayout(false);
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelLeft;
        private System.Windows.Forms.Button btn_Dashboard;
        private System.Windows.Forms.Label lblTitle2;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btn_Tickets;
        private System.Windows.Forms.Label lblShowTime;
        private System.Windows.Forms.Label lblShowWatch;
        private System.Windows.Forms.Timer Watch;
        private System.Windows.Forms.Panel panelTickets;
        private System.Windows.Forms.Button btnClosedTickets;
        private System.Windows.Forms.Button btnOpenedTickets;
        private System.Windows.Forms.Button btn_ColourTickets;
        private System.Windows.Forms.Button btnFindTickets;
        private Bunifu.Framework.UI.BunifuProgressBar bunifuProgressBar1;
        private Bunifu.Framework.UI.BunifuProgressBar progressbar_left;
        private Bunifu.Framework.UI.BunifuProgressBar btnLeftThinButton_Dashboard;
        private Bunifu.Framework.UI.BunifuProgressBar btnLeftThinButton_Tickets;
        private Bunifu.Framework.UI.BunifuElipse bunifuElipse_Main;
        private Guna.UI2.WinForms.Guna2ImageButton picture;
        private System.Windows.Forms.Label lblFrom;
        private Guna.UI2.WinForms.Guna2ImageButton btnOk;
        public Guna.UI2.WinForms.Guna2CustomGradientPanel panelFilter;
        public Guna.UI2.WinForms.Guna2DateTimePicker dateFrom;
        public Guna.UI2.WinForms.Guna2DateTimePicker dateTo;
        private System.Windows.Forms.Label lblTo;
        public Guna.UI2.WinForms.Guna2Button btnFullname;
        private Guna.UI2.WinForms.Guna2Panel panelMain;
        private Guna.UI2.WinForms.Guna2Panel panelTop;
        private Guna.UI2.WinForms.Guna2ImageButton btnFilter;
        public System.Windows.Forms.Label lblTotalResult;
        private System.Windows.Forms.Label lblTotal;
        public Guna.UI2.WinForms.Guna2TextBox txtSearch;
        private Guna.UI2.WinForms.Guna2ImageButton btnAddTicket;
        private System.Windows.Forms.Label lblHome;
        private Guna.UI2.WinForms.Guna2ImageButton btnLogout;
        private Guna.UI2.WinForms.Guna2ImageButton btnMinimize;
        private Guna.UI2.WinForms.Guna2ImageButton btnExit;
        private Guna.UI2.WinForms.Guna2ImageButton btnMaximize;
        public Guna.UI2.WinForms.Guna2ComboBox dateFiltering;
        public System.Windows.Forms.Button btn_Tasks;
        public System.Windows.Forms.Button btn_User;
        public Bunifu.Framework.UI.BunifuProgressBar btnLeftThinButton_User;
        public Bunifu.Framework.UI.BunifuProgressBar btnLeftThinButton_Problems;
        private Guna.UI2.WinForms.Guna2AnimateWindow guna2AnimateWindow;
    }
}