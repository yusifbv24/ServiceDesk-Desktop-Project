namespace ServiceDesk.Forms
{
    partial class LoginTitle
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginTitle));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.bunifuElipse = new Bunifu.Framework.UI.BunifuElipse(this.components);
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.guna2ProgressIndicator1 = new Guna.UI2.WinForms.Guna2ProgressIndicator();
            this.circle = new Guna.UI2.WinForms.Guna2CircleProgressBar();
            this.guna2AnimateWindow = new Guna.UI2.WinForms.Guna2AnimateWindow(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.circle.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.Timer1_Tick);
            // 
            // bunifuElipse
            // 
            this.bunifuElipse.ElipseRadius = 40;
            this.bunifuElipse.TargetControl = this;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::ServiceDesk.Properties.Resources.logo_2;
            this.pictureBox1.Location = new System.Drawing.Point(35, 100);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(230, 86);
            this.pictureBox1.TabIndex = 148;
            this.pictureBox1.TabStop = false;
            // 
            // guna2ProgressIndicator1
            // 
            this.guna2ProgressIndicator1.AutoStart = true;
            this.guna2ProgressIndicator1.Location = new System.Drawing.Point(132, 203);
            this.guna2ProgressIndicator1.Name = "guna2ProgressIndicator1";
            this.guna2ProgressIndicator1.Size = new System.Drawing.Size(42, 43);
            this.guna2ProgressIndicator1.TabIndex = 150;
            // 
            // circle
            // 
            this.circle.Controls.Add(this.pictureBox1);
            this.circle.Controls.Add(this.guna2ProgressIndicator1);
            this.circle.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(213)))), ((int)(((byte)(218)))), ((int)(((byte)(223)))));
            this.circle.FillThickness = 20;
            this.circle.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.circle.ForeColor = System.Drawing.Color.White;
            this.circle.ImageSize = new System.Drawing.Size(50, 200);
            this.circle.Location = new System.Drawing.Point(12, 12);
            this.circle.Minimum = 0;
            this.circle.Name = "circle";
            this.circle.ProgressColor = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(130)))), ((int)(((byte)(202)))));
            this.circle.ProgressColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(130)))), ((int)(((byte)(202)))));
            this.circle.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            this.circle.Size = new System.Drawing.Size(302, 302);
            this.circle.TabIndex = 151;
            this.circle.Text = "guna2CircleProgressBar1";
            // 
            // guna2AnimateWindow
            // 
            this.guna2AnimateWindow.Interval = 200;
            this.guna2AnimateWindow.TargetForm = this;
            // 
            // LoginTitle
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(326, 325);
            this.Controls.Add(this.circle);
            this.Font = new System.Drawing.Font("Inter", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "LoginTitle";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.circle.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer timer1;
        private Bunifu.Framework.UI.BunifuElipse bunifuElipse;
        private Guna.UI2.WinForms.Guna2ProgressIndicator guna2ProgressIndicator1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private Guna.UI2.WinForms.Guna2CircleProgressBar circle;
        private Guna.UI2.WinForms.Guna2AnimateWindow guna2AnimateWindow;
    }
}