namespace ServiceDesk.Forms
{
    partial class ClosedTickets
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvTicket = new System.Windows.Forms.DataGridView();
            this.guna2Elipse_Form = new Guna.UI2.WinForms.Guna2Elipse(this.components);
            this.Code = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Worker = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Vendor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Model = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Department = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Tasks = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Solution = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Finished_date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Fullname = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTicket)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvTicket
            // 
            this.dgvTicket.AllowUserToAddRows = false;
            this.dgvTicket.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Inter", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvTicket.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvTicket.BackgroundColor = System.Drawing.Color.White;
            this.dgvTicket.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvTicket.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SunkenHorizontal;
            this.dgvTicket.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.dgvTicket.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Inter", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.AliceBlue;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvTicket.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvTicket.ColumnHeadersHeight = 40;
            this.dgvTicket.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvTicket.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Code,
            this.Worker,
            this.Vendor,
            this.Model,
            this.Department,
            this.Tasks,
            this.Solution,
            this.Finished_date,
            this.Fullname});
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle12.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle12.Font = new System.Drawing.Font("Inter", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle12.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle12.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(133)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle12.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle12.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvTicket.DefaultCellStyle = dataGridViewCellStyle12;
            this.dgvTicket.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvTicket.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvTicket.EnableHeadersVisualStyles = false;
            this.dgvTicket.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(84)))), ((int)(((byte)(84)))), ((int)(((byte)(84)))));
            this.dgvTicket.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.dgvTicket.Location = new System.Drawing.Point(0, 0);
            this.dgvTicket.Margin = new System.Windows.Forms.Padding(0);
            this.dgvTicket.Name = "dgvTicket";
            this.dgvTicket.ReadOnly = true;
            this.dgvTicket.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle13.Font = new System.Drawing.Font("Inter", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle13.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvTicket.RowHeadersDefaultCellStyle = dataGridViewCellStyle13;
            this.dgvTicket.RowHeadersVisible = false;
            this.dgvTicket.RowHeadersWidth = 50;
            this.dgvTicket.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle14.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            dataGridViewCellStyle14.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvTicket.RowsDefaultCellStyle = dataGridViewCellStyle14;
            this.dgvTicket.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.dgvTicket.RowTemplate.DefaultCellStyle.BackColor = System.Drawing.Color.White;
            this.dgvTicket.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Inter", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvTicket.RowTemplate.DefaultCellStyle.ForeColor = System.Drawing.Color.Gray;
            this.dgvTicket.RowTemplate.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.AliceBlue;
            this.dgvTicket.RowTemplate.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.Black;
            this.dgvTicket.RowTemplate.DefaultCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvTicket.RowTemplate.Height = 41;
            this.dgvTicket.RowTemplate.ReadOnly = true;
            this.dgvTicket.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvTicket.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTicket.ShowEditingIcon = false;
            this.dgvTicket.Size = new System.Drawing.Size(1082, 690);
            this.dgvTicket.TabIndex = 5;
            // 
            // guna2Elipse_Form
            // 
            this.guna2Elipse_Form.BorderRadius = 25;
            this.guna2Elipse_Form.TargetControl = this;
            // 
            // Code
            // 
            this.Code.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.Code.DefaultCellStyle = dataGridViewCellStyle3;
            this.Code.HeaderText = "ID";
            this.Code.MinimumWidth = 40;
            this.Code.Name = "Code";
            this.Code.ReadOnly = true;
            this.Code.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Code.Width = 40;
            // 
            // Worker
            // 
            this.Worker.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.Worker.DefaultCellStyle = dataGridViewCellStyle4;
            this.Worker.HeaderText = "Code";
            this.Worker.MinimumWidth = 60;
            this.Worker.Name = "Worker";
            this.Worker.ReadOnly = true;
            this.Worker.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Worker.Width = 60;
            // 
            // Vendor
            // 
            this.Vendor.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.Vendor.DefaultCellStyle = dataGridViewCellStyle5;
            this.Vendor.HeaderText = "Department";
            this.Vendor.MinimumWidth = 150;
            this.Vendor.Name = "Vendor";
            this.Vendor.ReadOnly = true;
            this.Vendor.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Vendor.Width = 150;
            // 
            // Model
            // 
            this.Model.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.Model.DefaultCellStyle = dataGridViewCellStyle6;
            this.Model.HeaderText = "Worker";
            this.Model.MinimumWidth = 130;
            this.Model.Name = "Model";
            this.Model.ReadOnly = true;
            this.Model.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Model.Width = 130;
            // 
            // Department
            // 
            this.Department.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.Department.DefaultCellStyle = dataGridViewCellStyle7;
            this.Department.HeaderText = "Device";
            this.Department.MinimumWidth = 60;
            this.Department.Name = "Department";
            this.Department.ReadOnly = true;
            this.Department.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Department.Width = 80;
            // 
            // Tasks
            // 
            this.Tasks.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.Tasks.DefaultCellStyle = dataGridViewCellStyle8;
            this.Tasks.HeaderText = "Task";
            this.Tasks.MinimumWidth = 150;
            this.Tasks.Name = "Tasks";
            this.Tasks.ReadOnly = true;
            this.Tasks.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Tasks.Width = 200;
            // 
            // Solution
            // 
            this.Solution.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.Solution.DefaultCellStyle = dataGridViewCellStyle9;
            this.Solution.HeaderText = "Solution";
            this.Solution.MinimumWidth = 200;
            this.Solution.Name = "Solution";
            this.Solution.ReadOnly = true;
            this.Solution.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Solution.Width = 400;
            // 
            // Finished_date
            // 
            this.Finished_date.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.Finished_date.DefaultCellStyle = dataGridViewCellStyle10;
            this.Finished_date.HeaderText = "Finished Date";
            this.Finished_date.MinimumWidth = 120;
            this.Finished_date.Name = "Finished_date";
            this.Finished_date.ReadOnly = true;
            this.Finished_date.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Finished_date.Width = 120;
            // 
            // Fullname
            // 
            this.Fullname.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.Fullname.DefaultCellStyle = dataGridViewCellStyle11;
            this.Fullname.HeaderText = "Fullname";
            this.Fullname.MinimumWidth = 20;
            this.Fullname.Name = "Fullname";
            this.Fullname.ReadOnly = true;
            this.Fullname.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ClosedTickets
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1082, 690);
            this.Controls.Add(this.dgvTicket);
            this.Font = new System.Drawing.Font("Inter", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
            this.Name = "ClosedTickets";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ClosedTickets";
            ((System.ComponentModel.ISupportInitialize)(this.dgvTicket)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.DataGridView dgvTicket;
        private Guna.UI2.WinForms.Guna2Elipse guna2Elipse_Form;
        private System.Windows.Forms.DataGridViewTextBoxColumn Code;
        private System.Windows.Forms.DataGridViewTextBoxColumn Worker;
        private System.Windows.Forms.DataGridViewTextBoxColumn Vendor;
        private System.Windows.Forms.DataGridViewTextBoxColumn Model;
        private System.Windows.Forms.DataGridViewTextBoxColumn Department;
        private System.Windows.Forms.DataGridViewTextBoxColumn Tasks;
        private System.Windows.Forms.DataGridViewTextBoxColumn Solution;
        private System.Windows.Forms.DataGridViewTextBoxColumn Finished_date;
        private System.Windows.Forms.DataGridViewTextBoxColumn Fullname;
    }
}