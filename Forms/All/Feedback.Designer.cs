namespace ServiceDesk.Forms
{
    partial class Feedback
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Feedback));
            this.lblTitle = new System.Windows.Forms.Label();
            this.bunifuElipse_Apply = new Bunifu.Framework.UI.BunifuElipse(this.components);
            this.bunifuElipse_Module = new Bunifu.Framework.UI.BunifuElipse(this.components);
            this.Rating = new Guna.UI2.WinForms.Guna2RatingStar();
            this.guna2ShadowForm = new Guna.UI2.WinForms.Guna2ShadowForm(this.components);
            this.btnExit = new Guna.UI2.WinForms.Guna2ImageButton();
            this.txtMessage = new Guna.UI2.WinForms.Guna2TextBox();
            this.btnApply = new Guna.UI2.WinForms.Guna2Button();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Inter", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(8, 12);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(160, 20);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "Evaluate the ticket";
            // 
            // bunifuElipse_Apply
            // 
            this.bunifuElipse_Apply.ElipseRadius = 15;
            this.bunifuElipse_Apply.TargetControl = this;
            // 
            // bunifuElipse_Module
            // 
            this.bunifuElipse_Module.ElipseRadius = 40;
            this.bunifuElipse_Module.TargetControl = this;
            // 
            // Rating
            // 
            this.Rating.Location = new System.Drawing.Point(62, 60);
            this.Rating.Name = "Rating";
            this.Rating.RatingColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(179)))), ((int)(((byte)(133)))));
            this.Rating.Size = new System.Drawing.Size(157, 42);
            this.Rating.TabIndex = 90;
            // 
            // guna2ShadowForm
            // 
            this.guna2ShadowForm.TargetForm = this;
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
            this.btnExit.Location = new System.Drawing.Point(263, 5);
            this.btnExit.Name = "btnExit";
            this.btnExit.PressedState.Image = ((System.Drawing.Image)(resources.GetObject("resource.Image1")));
            this.btnExit.PressedState.ImageSize = new System.Drawing.Size(30, 30);
            this.btnExit.Size = new System.Drawing.Size(30, 30);
            this.btnExit.TabIndex = 159;
            this.btnExit.UseTransparentBackground = true;
            this.btnExit.Click += new System.EventHandler(this.BtnExit_Click);
            // 
            // txtMessage
            // 
            this.txtMessage.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtMessage.BorderRadius = 6;
            this.txtMessage.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtMessage.DefaultText = "";
            this.txtMessage.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtMessage.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtMessage.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtMessage.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtMessage.FillColor = System.Drawing.Color.WhiteSmoke;
            this.txtMessage.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtMessage.Font = new System.Drawing.Font("Inter", 12F);
            this.txtMessage.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtMessage.IconLeft = global::ServiceDesk.Properties.Resources.feedback_messager;
            this.txtMessage.IconLeftSize = new System.Drawing.Size(24, 24);
            this.txtMessage.Location = new System.Drawing.Point(28, 109);
            this.txtMessage.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.PasswordChar = '\0';
            this.txtMessage.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.txtMessage.PlaceholderText = "Leave a message";
            this.txtMessage.SelectedText = "";
            this.txtMessage.Size = new System.Drawing.Size(245, 34);
            this.txtMessage.Style = Guna.UI2.WinForms.Enums.TextBoxStyle.Material;
            this.txtMessage.TabIndex = 91;
            // 
            // btnApply
            // 
            this.btnApply.Animated = true;
            this.btnApply.BackColor = System.Drawing.Color.Transparent;
            this.btnApply.BorderRadius = 10;
            this.btnApply.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.ToogleButton;
            this.btnApply.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnApply.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnApply.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnApply.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnApply.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(179)))), ((int)(((byte)(133)))));
            this.btnApply.Font = new System.Drawing.Font("Inter", 14.25F);
            this.btnApply.ForeColor = System.Drawing.Color.White;
            this.btnApply.Image = global::ServiceDesk.Properties.Resources.apply;
            this.btnApply.Location = new System.Drawing.Point(85, 168);
            this.btnApply.Name = "btnApply";
            this.btnApply.PressedColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(179)))), ((int)(((byte)(133)))));
            this.btnApply.Size = new System.Drawing.Size(116, 40);
            this.btnApply.TabIndex = 89;
            this.btnApply.Text = "Apply";
            this.btnApply.Click += new System.EventHandler(this.BtnApply_Click);
            // 
            // Feedback
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(305, 217);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.txtMessage);
            this.Controls.Add(this.Rating);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.lblTitle);
            this.Font = new System.Drawing.Font("Inter", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.Name = "Feedback";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Feedback";
            this.TopMost = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Feedback_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblTitle;
        private Bunifu.Framework.UI.BunifuElipse bunifuElipse_Apply;
        private Bunifu.Framework.UI.BunifuElipse bunifuElipse_Module;
        public Guna.UI2.WinForms.Guna2Button btnApply;
        public Guna.UI2.WinForms.Guna2TextBox txtMessage;
        public Guna.UI2.WinForms.Guna2RatingStar Rating;
        private Guna.UI2.WinForms.Guna2ShadowForm guna2ShadowForm;
        private Guna.UI2.WinForms.Guna2ImageButton btnExit;
    }
}