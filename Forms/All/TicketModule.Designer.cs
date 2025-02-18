namespace ServiceDesk.Forms
{
    partial class TicketModule
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TicketModule));
            this.lblTitle = new System.Windows.Forms.Label();
            this.cmbTask = new System.Windows.Forms.ComboBox();
            this.txtDep = new System.Windows.Forms.ComboBox();
            this.guna2ShadowForm = new Guna.UI2.WinForms.Guna2ShadowForm(this.components);
            this.cmbUsers = new System.Windows.Forms.ComboBox();
            this.cmbSelectedUsers = new System.Windows.Forms.ComboBox();
            this.dgvTasks = new System.Windows.Forms.DataGridView();
            this.Problems = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Delete = new System.Windows.Forms.DataGridViewImageColumn();
            this.IfNoInventoryCode = new Guna.UI2.WinForms.Guna2CheckBox();
            this.dataGridViewImageColumn1 = new System.Windows.Forms.DataGridViewImageColumn();
            this.CheckInventoryCode = new Guna.UI2.WinForms.Guna2ImageButton();
            this.txtSolution = new Guna.UI2.WinForms.Guna2TextBox();
            this.btnExit = new Guna.UI2.WinForms.Guna2ImageButton();
            this.txtCode = new Guna.UI2.WinForms.Guna2TextBox();
            this.txtWorker = new Guna.UI2.WinForms.Guna2TextBox();
            this.txtDevice = new Guna.UI2.WinForms.Guna2TextBox();
            this.btnDeleteUsers = new Bunifu.Framework.UI.BunifuImageButton();
            this.ProblemPic = new Guna.UI2.WinForms.Guna2PictureBox();
            this.UserPic = new Guna.UI2.WinForms.Guna2PictureBox();
            this.DepartmentPic = new Guna.UI2.WinForms.Guna2PictureBox();
            this.btnClose = new Guna.UI2.WinForms.Guna2Button();
            this.btnSave = new Guna.UI2.WinForms.Guna2Button();
            this.btnUpdate = new Guna.UI2.WinForms.Guna2Button();
            this.bunifuElipse_panel = new Bunifu.Framework.UI.BunifuElipse(this.components);
            this.btnClear = new Guna.UI2.WinForms.Guna2Button();
            this.NotToday = new Guna.UI2.WinForms.Guna2CheckBox();
            this.date = new Bunifu.Framework.UI.BunifuDatepicker();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTasks)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnDeleteUsers)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ProblemPic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UserPic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DepartmentPic)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Inter", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblTitle.Location = new System.Drawing.Point(17, 14);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(154, 25);
            this.lblTitle.TabIndex = 35;
            this.lblTitle.Text = "Ticket Module";
            // 
            // cmbTask
            // 
            this.cmbTask.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbTask.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbTask.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbTask.BackColor = System.Drawing.Color.WhiteSmoke;
            this.cmbTask.DropDownHeight = 120;
            this.cmbTask.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbTask.Font = new System.Drawing.Font("Inter", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbTask.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.cmbTask.FormattingEnabled = true;
            this.cmbTask.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.cmbTask.IntegralHeight = false;
            this.cmbTask.Location = new System.Drawing.Point(55, 174);
            this.cmbTask.Name = "cmbTask";
            this.cmbTask.Size = new System.Drawing.Size(291, 28);
            this.cmbTask.Sorted = true;
            this.cmbTask.TabIndex = 5;
            this.cmbTask.Text = "Select a task";
            this.cmbTask.SelectedIndexChanged += new System.EventHandler(this.cmbTasks_SelectedIndexChanged);
            this.cmbTask.Enter += new System.EventHandler(this.CmbTasks_Enter);
            this.cmbTask.Leave += new System.EventHandler(this.cmbTasks_Leave);
            // 
            // txtDep
            // 
            this.txtDep.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDep.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.txtDep.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.txtDep.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtDep.DropDownHeight = 100;
            this.txtDep.Enabled = false;
            this.txtDep.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.txtDep.Font = new System.Drawing.Font("Inter", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDep.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.txtDep.FormattingEnabled = true;
            this.txtDep.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.txtDep.IntegralHeight = false;
            this.txtDep.Location = new System.Drawing.Point(55, 124);
            this.txtDep.Name = "txtDep";
            this.txtDep.Size = new System.Drawing.Size(291, 28);
            this.txtDep.Sorted = true;
            this.txtDep.TabIndex = 2;
            this.txtDep.TabStop = false;
            this.txtDep.Text = "Department";
            this.txtDep.Enter += new System.EventHandler(this.TxtDep_Enter);
            this.txtDep.Leave += new System.EventHandler(this.TxtDep_Leave);
            // 
            // guna2ShadowForm
            // 
            this.guna2ShadowForm.TargetForm = this;
            // 
            // cmbUsers
            // 
            this.cmbUsers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbUsers.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbUsers.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbUsers.BackColor = System.Drawing.Color.WhiteSmoke;
            this.cmbUsers.DropDownHeight = 100;
            this.cmbUsers.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbUsers.Font = new System.Drawing.Font("Inter", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbUsers.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.cmbUsers.FormattingEnabled = true;
            this.cmbUsers.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmbUsers.IntegralHeight = false;
            this.cmbUsers.Location = new System.Drawing.Point(433, 333);
            this.cmbUsers.Name = "cmbUsers";
            this.cmbUsers.Size = new System.Drawing.Size(252, 28);
            this.cmbUsers.Sorted = true;
            this.cmbUsers.TabIndex = 155;
            this.cmbUsers.Text = "Users";
            this.cmbUsers.Visible = false;
            this.cmbUsers.SelectedIndexChanged += new System.EventHandler(this.CmbUsers_SelectedIndexChanged);
            // 
            // cmbSelectedUsers
            // 
            this.cmbSelectedUsers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbSelectedUsers.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbSelectedUsers.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbSelectedUsers.BackColor = System.Drawing.Color.WhiteSmoke;
            this.cmbSelectedUsers.DropDownHeight = 120;
            this.cmbSelectedUsers.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbSelectedUsers.Font = new System.Drawing.Font("Inter", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbSelectedUsers.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.cmbSelectedUsers.FormattingEnabled = true;
            this.cmbSelectedUsers.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.cmbSelectedUsers.IntegralHeight = false;
            this.cmbSelectedUsers.Location = new System.Drawing.Point(398, 382);
            this.cmbSelectedUsers.Name = "cmbSelectedUsers";
            this.cmbSelectedUsers.Size = new System.Drawing.Size(252, 28);
            this.cmbSelectedUsers.Sorted = true;
            this.cmbSelectedUsers.TabIndex = 158;
            this.cmbSelectedUsers.Text = "Selected users";
            this.cmbSelectedUsers.Visible = false;
            this.cmbSelectedUsers.SelectedIndexChanged += new System.EventHandler(this.CmbSelectedUsers_SelectedIndexChanged);
            // 
            // dgvTasks
            // 
            this.dgvTasks.AllowUserToAddRows = false;
            this.dgvTasks.AllowUserToDeleteRows = false;
            this.dgvTasks.AllowUserToResizeColumns = false;
            this.dgvTasks.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvTasks.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvTasks.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvTasks.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvTasks.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            this.dgvTasks.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvTasks.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.RaisedHorizontal;
            this.dgvTasks.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.dgvTasks.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Inter", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvTasks.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvTasks.ColumnHeadersHeight = 40;
            this.dgvTasks.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvTasks.ColumnHeadersVisible = false;
            this.dgvTasks.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Problems,
            this.Delete});
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Inter", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvTasks.DefaultCellStyle = dataGridViewCellStyle4;
            this.dgvTasks.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvTasks.EnableHeadersVisualStyles = false;
            this.dgvTasks.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(84)))), ((int)(((byte)(84)))), ((int)(((byte)(84)))));
            this.dgvTasks.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.dgvTasks.Location = new System.Drawing.Point(25, 220);
            this.dgvTasks.Margin = new System.Windows.Forms.Padding(0);
            this.dgvTasks.Name = "dgvTasks";
            this.dgvTasks.ReadOnly = true;
            this.dgvTasks.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Inter", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvTasks.RowHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvTasks.RowHeadersVisible = false;
            this.dgvTasks.RowHeadersWidth = 30;
            this.dgvTasks.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.dgvTasks.RowsDefaultCellStyle = dataGridViewCellStyle6;
            this.dgvTasks.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.dgvTasks.RowTemplate.DefaultCellStyle.BackColor = System.Drawing.Color.WhiteSmoke;
            this.dgvTasks.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Inter", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvTasks.RowTemplate.DefaultCellStyle.ForeColor = System.Drawing.Color.Gray;
            this.dgvTasks.RowTemplate.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.Black;
            this.dgvTasks.RowTemplate.DefaultCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvTasks.RowTemplate.Height = 30;
            this.dgvTasks.RowTemplate.ReadOnly = true;
            this.dgvTasks.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvTasks.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTasks.ShowEditingIcon = false;
            this.dgvTasks.Size = new System.Drawing.Size(321, 151);
            this.dgvTasks.TabIndex = 162;
            this.dgvTasks.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvTasks_CellContentClick);
            // 
            // Problems
            // 
            this.Problems.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.Problems.DefaultCellStyle = dataGridViewCellStyle3;
            this.Problems.HeaderText = "";
            this.Problems.MinimumWidth = 100;
            this.Problems.Name = "Problems";
            this.Problems.ReadOnly = true;
            this.Problems.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Problems.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Delete
            // 
            this.Delete.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Delete.HeaderText = "";
            this.Delete.Image = global::ServiceDesk.Properties.Resources.exit_hover;
            this.Delete.MinimumWidth = 25;
            this.Delete.Name = "Delete";
            this.Delete.ReadOnly = true;
            this.Delete.Width = 25;
            // 
            // IfNoInventoryCode
            // 
            this.IfNoInventoryCode.AutoSize = true;
            this.IfNoInventoryCode.CheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.IfNoInventoryCode.CheckedState.BorderRadius = 0;
            this.IfNoInventoryCode.CheckedState.BorderThickness = 0;
            this.IfNoInventoryCode.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.IfNoInventoryCode.Font = new System.Drawing.Font("Inter", 9F);
            this.IfNoInventoryCode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(133)))), ((int)(((byte)(255)))));
            this.IfNoInventoryCode.Location = new System.Drawing.Point(239, 83);
            this.IfNoInventoryCode.Name = "IfNoInventoryCode";
            this.IfNoInventoryCode.Size = new System.Drawing.Size(107, 19);
            this.IfNoInventoryCode.TabIndex = 164;
            this.IfNoInventoryCode.Text = "Doesn\'t have?";
            this.IfNoInventoryCode.UncheckedState.BorderColor = System.Drawing.Color.LightGray;
            this.IfNoInventoryCode.UncheckedState.BorderRadius = 0;
            this.IfNoInventoryCode.UncheckedState.BorderThickness = 0;
            this.IfNoInventoryCode.UncheckedState.FillColor = System.Drawing.Color.LightGray;
            this.IfNoInventoryCode.CheckStateChanged += new System.EventHandler(this.IfNoInventoryCode_CheckStateChanged);
            // 
            // dataGridViewImageColumn1
            // 
            this.dataGridViewImageColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewImageColumn1.HeaderText = "";
            this.dataGridViewImageColumn1.Image = global::ServiceDesk.Properties.Resources.exit_hover;
            this.dataGridViewImageColumn1.MinimumWidth = 25;
            this.dataGridViewImageColumn1.Name = "dataGridViewImageColumn1";
            // 
            // CheckInventoryCode
            // 
            this.CheckInventoryCode.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.CheckInventoryCode.AnimatedGIF = true;
            this.CheckInventoryCode.CheckedState.ImageSize = new System.Drawing.Size(64, 64);
            this.CheckInventoryCode.Cursor = System.Windows.Forms.Cursors.Hand;
            this.CheckInventoryCode.HoverState.ImageSize = new System.Drawing.Size(24, 24);
            this.CheckInventoryCode.Image = global::ServiceDesk.Properties.Resources.check;
            this.CheckInventoryCode.ImageOffset = new System.Drawing.Point(0, 0);
            this.CheckInventoryCode.ImageRotate = 0F;
            this.CheckInventoryCode.ImageSize = new System.Drawing.Size(24, 24);
            this.CheckInventoryCode.Location = new System.Drawing.Point(195, 74);
            this.CheckInventoryCode.Name = "CheckInventoryCode";
            this.CheckInventoryCode.PressedState.ImageSize = new System.Drawing.Size(24, 24);
            this.CheckInventoryCode.Size = new System.Drawing.Size(30, 28);
            this.CheckInventoryCode.TabIndex = 165;
            this.CheckInventoryCode.Visible = false;
            // 
            // txtSolution
            // 
            this.txtSolution.AllowDrop = true;
            this.txtSolution.AutoScroll = true;
            this.txtSolution.AutoSize = true;
            this.txtSolution.AutoValidate = System.Windows.Forms.AutoValidate.EnablePreventFocusChange;
            this.txtSolution.BackColor = System.Drawing.Color.Transparent;
            this.txtSolution.BorderRadius = 6;
            this.txtSolution.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtSolution.DefaultText = "";
            this.txtSolution.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtSolution.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtSolution.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtSolution.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtSolution.FillColor = System.Drawing.Color.WhiteSmoke;
            this.txtSolution.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtSolution.Font = new System.Drawing.Font("Inter", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSolution.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtSolution.IconLeft = global::ServiceDesk.Properties.Resources.solutions;
            this.txtSolution.IconLeftOffset = new System.Drawing.Point(0, -30);
            this.txtSolution.IconLeftSize = new System.Drawing.Size(24, 24);
            this.txtSolution.Location = new System.Drawing.Point(398, 174);
            this.txtSolution.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtSolution.MaximumSize = new System.Drawing.Size(296, 128);
            this.txtSolution.MinimumSize = new System.Drawing.Size(291, 34);
            this.txtSolution.Multiline = true;
            this.txtSolution.Name = "txtSolution";
            this.txtSolution.PasswordChar = '\0';
            this.txtSolution.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.txtSolution.PlaceholderText = "Solution";
            this.txtSolution.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtSolution.SelectedText = "";
            this.txtSolution.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            this.txtSolution.Size = new System.Drawing.Size(291, 97);
            this.txtSolution.Style = Guna.UI2.WinForms.Enums.TextBoxStyle.Material;
            this.txtSolution.TabIndex = 8;
            this.txtSolution.Enter += new System.EventHandler(this.TxtSolution_Enter);
            this.txtSolution.Leave += new System.EventHandler(this.TxtSolution_Leave);
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
            this.btnExit.Location = new System.Drawing.Point(671, 9);
            this.btnExit.Name = "btnExit";
            this.btnExit.PressedState.Image = ((System.Drawing.Image)(resources.GetObject("resource.Image1")));
            this.btnExit.PressedState.ImageSize = new System.Drawing.Size(30, 30);
            this.btnExit.Size = new System.Drawing.Size(30, 30);
            this.btnExit.TabIndex = 161;
            this.btnExit.UseTransparentBackground = true;
            this.btnExit.Click += new System.EventHandler(this.BtnExit_Click);
            // 
            // txtCode
            // 
            this.txtCode.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtCode.BorderRadius = 6;
            this.txtCode.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtCode.DefaultText = "";
            this.txtCode.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtCode.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtCode.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtCode.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtCode.FillColor = System.Drawing.Color.WhiteSmoke;
            this.txtCode.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtCode.Font = new System.Drawing.Font("Inter", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCode.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtCode.IconLeft = global::ServiceDesk.Properties.Resources.code;
            this.txtCode.IconLeftSize = new System.Drawing.Size(24, 24);
            this.txtCode.Location = new System.Drawing.Point(22, 74);
            this.txtCode.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtCode.Name = "txtCode";
            this.txtCode.PasswordChar = '\0';
            this.txtCode.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.txtCode.PlaceholderText = "Inventory code";
            this.txtCode.SelectedText = "";
            this.txtCode.Size = new System.Drawing.Size(166, 28);
            this.txtCode.Style = Guna.UI2.WinForms.Enums.TextBoxStyle.Material;
            this.txtCode.TabIndex = 1;
            this.txtCode.Leave += new System.EventHandler(this.TxtCode_Leave);
            // 
            // txtWorker
            // 
            this.txtWorker.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtWorker.BorderRadius = 6;
            this.txtWorker.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtWorker.DefaultText = "";
            this.txtWorker.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtWorker.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtWorker.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtWorker.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtWorker.Enabled = false;
            this.txtWorker.FillColor = System.Drawing.Color.WhiteSmoke;
            this.txtWorker.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtWorker.Font = new System.Drawing.Font("Inter", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtWorker.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtWorker.IconLeft = global::ServiceDesk.Properties.Resources.worker;
            this.txtWorker.IconLeftSize = new System.Drawing.Size(24, 24);
            this.txtWorker.Location = new System.Drawing.Point(398, 124);
            this.txtWorker.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtWorker.Name = "txtWorker";
            this.txtWorker.PasswordChar = '\0';
            this.txtWorker.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.txtWorker.PlaceholderText = "Worker";
            this.txtWorker.SelectedText = "";
            this.txtWorker.Size = new System.Drawing.Size(287, 28);
            this.txtWorker.Style = Guna.UI2.WinForms.Enums.TextBoxStyle.Material;
            this.txtWorker.TabIndex = 3;
            // 
            // txtDevice
            // 
            this.txtDevice.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtDevice.BorderRadius = 6;
            this.txtDevice.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtDevice.DefaultText = "";
            this.txtDevice.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtDevice.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtDevice.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtDevice.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtDevice.Enabled = false;
            this.txtDevice.FillColor = System.Drawing.Color.WhiteSmoke;
            this.txtDevice.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtDevice.Font = new System.Drawing.Font("Inter", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDevice.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtDevice.IconLeft = global::ServiceDesk.Properties.Resources.device;
            this.txtDevice.IconLeftSize = new System.Drawing.Size(24, 24);
            this.txtDevice.Location = new System.Drawing.Point(398, 74);
            this.txtDevice.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtDevice.Name = "txtDevice";
            this.txtDevice.PasswordChar = '\0';
            this.txtDevice.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.txtDevice.PlaceholderText = "Device";
            this.txtDevice.SelectedText = "";
            this.txtDevice.Size = new System.Drawing.Size(287, 28);
            this.txtDevice.Style = Guna.UI2.WinForms.Enums.TextBoxStyle.Material;
            this.txtDevice.TabIndex = 4;
            // 
            // btnDeleteUsers
            // 
            this.btnDeleteUsers.BackColor = System.Drawing.Color.Transparent;
            this.btnDeleteUsers.Image = ((System.Drawing.Image)(resources.GetObject("btnDeleteUsers.Image")));
            this.btnDeleteUsers.ImageActive = null;
            this.btnDeleteUsers.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnDeleteUsers.Location = new System.Drawing.Point(660, 382);
            this.btnDeleteUsers.Name = "btnDeleteUsers";
            this.btnDeleteUsers.Size = new System.Drawing.Size(25, 28);
            this.btnDeleteUsers.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.btnDeleteUsers.TabIndex = 159;
            this.btnDeleteUsers.TabStop = false;
            this.btnDeleteUsers.Visible = false;
            this.btnDeleteUsers.Zoom = 0;
            this.btnDeleteUsers.Click += new System.EventHandler(this.BtnDeleteUsers_Click);
            // 
            // ProblemPic
            // 
            this.ProblemPic.Image = global::ServiceDesk.Properties.Resources.problem_solving;
            this.ProblemPic.ImageRotate = 0F;
            this.ProblemPic.Location = new System.Drawing.Point(25, 174);
            this.ProblemPic.Name = "ProblemPic";
            this.ProblemPic.Size = new System.Drawing.Size(24, 24);
            this.ProblemPic.TabIndex = 152;
            this.ProblemPic.TabStop = false;
            // 
            // UserPic
            // 
            this.UserPic.Image = global::ServiceDesk.Properties.Resources.users;
            this.UserPic.ImageRotate = 0F;
            this.UserPic.Location = new System.Drawing.Point(398, 333);
            this.UserPic.Name = "UserPic";
            this.UserPic.Size = new System.Drawing.Size(24, 24);
            this.UserPic.TabIndex = 156;
            this.UserPic.TabStop = false;
            this.UserPic.Visible = false;
            // 
            // DepartmentPic
            // 
            this.DepartmentPic.Enabled = false;
            this.DepartmentPic.Image = global::ServiceDesk.Properties.Resources.department;
            this.DepartmentPic.ImageRotate = 0F;
            this.DepartmentPic.Location = new System.Drawing.Point(25, 124);
            this.DepartmentPic.Name = "DepartmentPic";
            this.DepartmentPic.Size = new System.Drawing.Size(24, 24);
            this.DepartmentPic.TabIndex = 154;
            this.DepartmentPic.TabStop = false;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.Transparent;
            this.btnClose.BorderRadius = 10;
            this.btnClose.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.ToogleButton;
            this.btnClose.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnClose.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnClose.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnClose.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnClose.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(179)))), ((int)(((byte)(133)))));
            this.btnClose.Font = new System.Drawing.Font("Inter", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Image = global::ServiceDesk.Properties.Resources.close;
            this.btnClose.Location = new System.Drawing.Point(195, 454);
            this.btnClose.Name = "btnClose";
            this.btnClose.PressedColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(179)))), ((int)(((byte)(133)))));
            this.btnClose.Size = new System.Drawing.Size(116, 40);
            this.btnClose.TabIndex = 11;
            this.btnClose.Text = "Close";
            this.btnClose.Visible = false;
            this.btnClose.Click += new System.EventHandler(this.Btn_Close_Click);
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.Transparent;
            this.btnSave.BorderRadius = 10;
            this.btnSave.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.ToogleButton;
            this.btnSave.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnSave.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnSave.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnSave.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnSave.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(179)))), ((int)(((byte)(133)))));
            this.btnSave.Font = new System.Drawing.Font("Inter", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Image = global::ServiceDesk.Properties.Resources.upload;
            this.btnSave.Location = new System.Drawing.Point(195, 454);
            this.btnSave.Name = "btnSave";
            this.btnSave.PressedColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(179)))), ((int)(((byte)(133)))));
            this.btnSave.Size = new System.Drawing.Size(116, 40);
            this.btnSave.TabIndex = 10;
            this.btnSave.Text = "Save";
            this.btnSave.Visible = false;
            this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.BackColor = System.Drawing.Color.Transparent;
            this.btnUpdate.BorderRadius = 10;
            this.btnUpdate.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.ToogleButton;
            this.btnUpdate.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnUpdate.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnUpdate.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnUpdate.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnUpdate.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(120)))), ((int)(((byte)(32)))));
            this.btnUpdate.Font = new System.Drawing.Font("Inter", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUpdate.ForeColor = System.Drawing.Color.White;
            this.btnUpdate.Image = global::ServiceDesk.Properties.Resources.refresh;
            this.btnUpdate.Location = new System.Drawing.Point(195, 454);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.PressedColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(120)))), ((int)(((byte)(32)))));
            this.btnUpdate.Size = new System.Drawing.Size(116, 40);
            this.btnUpdate.TabIndex = 9;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.Visible = false;
            this.btnUpdate.Click += new System.EventHandler(this.BtnUpdate_Click);
            // 
            // bunifuElipse_panel
            // 
            this.bunifuElipse_panel.ElipseRadius = 25;
            this.bunifuElipse_panel.TargetControl = this;
            // 
            // btnClear
            // 
            this.btnClear.BackColor = System.Drawing.Color.Transparent;
            this.btnClear.BorderRadius = 10;
            this.btnClear.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.ToogleButton;
            this.btnClear.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnClear.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnClear.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnClear.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnClear.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(50)))), ((int)(((byte)(61)))));
            this.btnClear.Font = new System.Drawing.Font("Inter", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.ForeColor = System.Drawing.Color.White;
            this.btnClear.Image = global::ServiceDesk.Properties.Resources.clear;
            this.btnClear.Location = new System.Drawing.Point(398, 454);
            this.btnClear.Name = "btnClear";
            this.btnClear.PressedColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(50)))), ((int)(((byte)(61)))));
            this.btnClear.Size = new System.Drawing.Size(116, 40);
            this.btnClear.TabIndex = 12;
            this.btnClear.Text = "Clear";
            this.btnClear.Visible = false;
            this.btnClear.Click += new System.EventHandler(this.BtnClear_Click);
            // 
            // NotToday
            // 
            this.NotToday.AutoSize = true;
            this.NotToday.CheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.NotToday.CheckedState.BorderRadius = 0;
            this.NotToday.CheckedState.BorderThickness = 0;
            this.NotToday.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.NotToday.Font = new System.Drawing.Font("Inter", 9F);
            this.NotToday.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(133)))), ((int)(((byte)(255)))));
            this.NotToday.Location = new System.Drawing.Point(398, 290);
            this.NotToday.Name = "NotToday";
            this.NotToday.Size = new System.Drawing.Size(87, 19);
            this.NotToday.TabIndex = 166;
            this.NotToday.Text = "Not today?";
            this.NotToday.UncheckedState.BorderColor = System.Drawing.Color.LightGray;
            this.NotToday.UncheckedState.BorderRadius = 0;
            this.NotToday.UncheckedState.BorderThickness = 0;
            this.NotToday.UncheckedState.FillColor = System.Drawing.Color.LightGray;
            this.NotToday.Visible = false;
            this.NotToday.CheckStateChanged += new System.EventHandler(this.NotToday_CheckStateChanged);
            // 
            // date
            // 
            this.date.BackColor = System.Drawing.Color.Transparent;
            this.date.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.date.BorderRadius = 10;
            this.date.Font = new System.Drawing.Font("Inter", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(1)), true);
            this.date.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.date.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.date.FormatCustom = "yyyy-MM-dd";
            this.date.Location = new System.Drawing.Point(511, 285);
            this.date.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.date.Name = "date";
            this.date.Size = new System.Drawing.Size(174, 28);
            this.date.TabIndex = 168;
            this.date.Value = new System.DateTime(2025, 1, 29, 0, 0, 0, 0);
            this.date.Visible = false;
            // 
            // TicketModule
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(719, 516);
            this.Controls.Add(this.date);
            this.Controls.Add(this.NotToday);
            this.Controls.Add(this.CheckInventoryCode);
            this.Controls.Add(this.IfNoInventoryCode);
            this.Controls.Add(this.dgvTasks);
            this.Controls.Add(this.txtDep);
            this.Controls.Add(this.txtSolution);
            this.Controls.Add(this.cmbTask);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.txtCode);
            this.Controls.Add(this.txtWorker);
            this.Controls.Add(this.cmbSelectedUsers);
            this.Controls.Add(this.txtDevice);
            this.Controls.Add(this.btnDeleteUsers);
            this.Controls.Add(this.ProblemPic);
            this.Controls.Add(this.UserPic);
            this.Controls.Add(this.DepartmentPic);
            this.Controls.Add(this.cmbUsers);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.lblTitle);
            this.Font = new System.Drawing.Font("Inter", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.Name = "TicketModule";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.TopMost = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TicketModule_KeyDown);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TicketModule_MouseDown);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTasks)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnDeleteUsers)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ProblemPic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UserPic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DepartmentPic)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblTitle;
        public System.Windows.Forms.ComboBox cmbTask;
        public Guna.UI2.WinForms.Guna2TextBox txtCode;
        public Guna.UI2.WinForms.Guna2TextBox txtDevice;
        public Guna.UI2.WinForms.Guna2TextBox txtWorker;
        public Guna.UI2.WinForms.Guna2TextBox txtSolution;
        private Guna.UI2.WinForms.Guna2PictureBox DepartmentPic;
        public System.Windows.Forms.ComboBox txtDep;
        private Guna.UI2.WinForms.Guna2PictureBox ProblemPic;
        public Guna.UI2.WinForms.Guna2Button btnUpdate;
        public Guna.UI2.WinForms.Guna2Button btnSave;
        public Guna.UI2.WinForms.Guna2Button btnClose;
        private Guna.UI2.WinForms.Guna2ShadowForm guna2ShadowForm;
        private Guna.UI2.WinForms.Guna2PictureBox UserPic;
        public System.Windows.Forms.ComboBox cmbUsers;
        public System.Windows.Forms.ComboBox cmbSelectedUsers;
        public Bunifu.Framework.UI.BunifuImageButton btnDeleteUsers;
        private Guna.UI2.WinForms.Guna2ImageButton btnExit;
        private System.Windows.Forms.DataGridViewTextBoxColumn Problems;
        private System.Windows.Forms.DataGridViewImageColumn Delete;
        public System.Windows.Forms.DataGridView dgvTasks;
        private Guna.UI2.WinForms.Guna2CheckBox IfNoInventoryCode;
        private Guna.UI2.WinForms.Guna2ImageButton CheckInventoryCode;
        private System.Windows.Forms.DataGridViewImageColumn dataGridViewImageColumn1;
        private Bunifu.Framework.UI.BunifuElipse bunifuElipse_panel;
        public Guna.UI2.WinForms.Guna2Button btnClear;
        private Bunifu.Framework.UI.BunifuDatepicker date;
        public Guna.UI2.WinForms.Guna2CheckBox NotToday;
    }
}