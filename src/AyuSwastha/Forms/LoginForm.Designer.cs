using System.Drawing;
using System.Windows.Forms;
using AyuSwastha.Core;

namespace AyuSwastha.Forms
{
    partial class LoginForm
    {
        private System.ComponentModel.IContainer components = null;

        private Panel panelHeader;
        private PictureBox picLogo;
        private Label lblTitle;
        private Label lblSubtitle;
        private Label lblUser;
        private TextBox txtUsername;
        private Label lblPass;
        private TextBox txtPassword;
        private Button btnLogin;
        private Button btnCancel;
        private Label lblError;
        private Label lblHint;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.panelHeader = new Panel();
            this.picLogo = new PictureBox();
            this.lblTitle = new Label();
            this.lblSubtitle = new Label();
            this.lblUser = new Label();
            this.txtUsername = new TextBox();
            this.lblPass = new Label();
            this.txtPassword = new TextBox();
            this.btnLogin = new Button();
            this.btnCancel = new Button();
            this.lblError = new Label();
            this.lblHint = new Label();
            this.SuspendLayout();
            //
            // panelHeader
            //
            this.panelHeader.BackColor = Theme.Primary;
            this.panelHeader.Dock = DockStyle.Top;
            this.panelHeader.Height = 96;
            this.panelHeader.Controls.Add(this.picLogo);
            this.panelHeader.Controls.Add(this.lblTitle);
            this.panelHeader.Controls.Add(this.lblSubtitle);
            //
            // picLogo
            //
            this.picLogo.Location = new Point(24, 20);
            this.picLogo.Size = new Size(56, 56);
            this.picLogo.SizeMode = PictureBoxSizeMode.Zoom;
            try { this.picLogo.Image = Image.FromFile(@"Resources\logo.png"); } catch { }
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = true;
            this.lblTitle.ForeColor = Color.White;
            this.lblTitle.Font = new Font("Segoe UI Semibold", 22f, FontStyle.Bold);
            this.lblTitle.Location = new Point(88, 20);
            this.lblTitle.Text = "AyuSwastha";
            //
            // lblSubtitle
            //
            this.lblSubtitle.AutoSize = true;
            this.lblSubtitle.ForeColor = Color.FromArgb(220, 235, 225);
            this.lblSubtitle.Font = new Font("Segoe UI", 10f);
            this.lblSubtitle.Location = new Point(92, 62);
            this.lblSubtitle.Text = "Ayurveda Clinic Management System";
            //
            // lblUser
            //
            this.lblUser.AutoSize = true;
            this.lblUser.Font = Theme.Body;
            this.lblUser.ForeColor = Theme.TextPrimary;
            this.lblUser.Location = new Point(30, 128);
            this.lblUser.Text = "Username";
            //
            // txtUsername
            //
            this.txtUsername.Font = new Font("Segoe UI", 11f);
            this.txtUsername.Location = new Point(30, 150);
            this.txtUsername.Size = new Size(360, 27);
            //
            // lblPass
            //
            this.lblPass.AutoSize = true;
            this.lblPass.Font = Theme.Body;
            this.lblPass.ForeColor = Theme.TextPrimary;
            this.lblPass.Location = new Point(30, 190);
            this.lblPass.Text = "Password";
            //
            // txtPassword
            //
            this.txtPassword.Font = new Font("Segoe UI", 11f);
            this.txtPassword.Location = new Point(30, 212);
            this.txtPassword.Size = new Size(360, 27);
            this.txtPassword.UseSystemPasswordChar = true;
            //
            // lblError
            //
            this.lblError.AutoSize = false;
            this.lblError.ForeColor = Theme.Danger;
            this.lblError.Font = Theme.Body;
            this.lblError.Location = new Point(30, 246);
            this.lblError.Size = new Size(360, 20);
            this.lblError.Text = "";
            //
            // btnLogin
            //
            this.btnLogin.BackColor = Theme.Primary;
            this.btnLogin.FlatStyle = FlatStyle.Flat;
            this.btnLogin.FlatAppearance.BorderSize = 0;
            this.btnLogin.ForeColor = Color.White;
            this.btnLogin.Font = new Font("Segoe UI Semibold", 10.5f);
            this.btnLogin.Location = new Point(30, 274);
            this.btnLogin.Size = new Size(250, 40);
            this.btnLogin.Text = "Sign in";
            this.btnLogin.UseVisualStyleBackColor = false;
            this.btnLogin.Click += this.btnLogin_Click;
            //
            // btnCancel
            //
            this.btnCancel.FlatStyle = FlatStyle.Flat;
            this.btnCancel.FlatAppearance.BorderColor = Theme.Border;
            this.btnCancel.ForeColor = Theme.TextPrimary;
            this.btnCancel.Font = new Font("Segoe UI", 10.5f);
            this.btnCancel.Location = new Point(290, 274);
            this.btnCancel.Size = new Size(100, 40);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += this.btnCancel_Click;
            //
            // lblHint
            //
            this.lblHint.AutoSize = false;
            this.lblHint.ForeColor = Theme.TextMuted;
            this.lblHint.Font = new Font("Segoe UI", 8.5f, FontStyle.Italic);
            this.lblHint.Location = new Point(30, 326);
            this.lblHint.Size = new Size(360, 20);
            this.lblHint.Text = "Default login — admin / admin123";
            //
            // LoginForm
            //
            this.AcceptButton = this.btnLogin;
            this.CancelButton = this.btnCancel;
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Theme.Surface;
            this.ClientSize = new Size(420, 360);
            this.Controls.Add(this.lblHint);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.lblError);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.lblPass);
            this.Controls.Add(this.txtUsername);
            this.Controls.Add(this.lblUser);
            this.Controls.Add(this.panelHeader);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Sign in — AyuSwastha";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
