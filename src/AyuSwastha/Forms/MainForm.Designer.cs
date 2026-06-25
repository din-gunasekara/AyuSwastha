using System.Drawing;
using System.Windows.Forms;
using AyuSwastha.Core;

namespace AyuSwastha.Forms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        private Panel panelTop;
        private Label lblAppTitle;
        private PictureBox picLogo;
        private Label lblUser;
        private Button btnLogout;
        private Panel panelNav;
        private Panel panelContent;

        private Button btnDashboard;
        private Button btnPatients;
        private Button btnAppointments;
        private Button btnTherapy;
        private Button btnPrescriptions;
        private Button btnBilling;
        private Button btnDoctors;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.panelTop = new Panel();
            this.lblAppTitle = new Label();
            this.picLogo = new PictureBox();
            this.lblUser = new Label();
            this.btnLogout = new Button();
            this.panelNav = new Panel();
            this.panelContent = new Panel();
            this.btnDashboard = new Button();
            this.btnPatients = new Button();
            this.btnAppointments = new Button();
            this.btnTherapy = new Button();
            this.btnPrescriptions = new Button();
            this.btnBilling = new Button();
            this.btnDoctors = new Button();
            this.SuspendLayout();
            //
            // panelTop
            //
            this.panelTop.BackColor = Theme.Primary;
            this.panelTop.Dock = DockStyle.Top;
            this.panelTop.Height = 56;
            this.panelTop.Controls.Add(this.picLogo);
            this.panelTop.Controls.Add(this.lblAppTitle);
            this.panelTop.Controls.Add(this.lblUser);
            this.panelTop.Controls.Add(this.btnLogout);
            //
            // picLogo
            //
            this.picLogo.Location = new Point(16, 8);
            this.picLogo.Size = new Size(40, 40);
            this.picLogo.SizeMode = PictureBoxSizeMode.Zoom;
            try { this.picLogo.Image = Image.FromFile(@"Resources\logo.png"); } catch { }
            //
            // lblAppTitle
            //
            this.lblAppTitle.AutoSize = true;
            this.lblAppTitle.ForeColor = Color.White;
            this.lblAppTitle.Font = new Font("Segoe UI Semibold", 15f, FontStyle.Bold);
            this.lblAppTitle.Location = new Point(64, 13);
            this.lblAppTitle.Text = "AyuSwastha";
            //
            // lblUser
            //
            this.lblUser.Dock = DockStyle.Right;
            this.lblUser.AutoSize = false;
            this.lblUser.TextAlign = ContentAlignment.MiddleRight;
            this.lblUser.ForeColor = Color.FromArgb(225, 240, 230);
            this.lblUser.Font = Theme.Body;
            this.lblUser.Padding = new Padding(0, 0, 20, 0); // Spacing from button
            this.lblUser.Size = new Size(300, 56);
            this.lblUser.Text = "";
            //
            // btnLogout
            //
            this.btnLogout.Dock = DockStyle.Right;
            this.btnLogout.FlatStyle = FlatStyle.Flat;
            this.btnLogout.FlatAppearance.BorderColor = Color.FromArgb(120, 170, 145);
            this.btnLogout.ForeColor = Color.White;
            this.btnLogout.Font = Theme.Body;
            this.btnLogout.Size = new Size(90, 56);
            this.btnLogout.Text = "Logout";
            this.btnLogout.Cursor = Cursors.Hand;
            this.btnLogout.UseVisualStyleBackColor = true;
            this.btnLogout.Click += this.btnLogout_Click;
            //
            // panelNav
            //
            this.panelNav.BackColor = Theme.SidebarBg;
            this.panelNav.Dock = DockStyle.Left;
            this.panelNav.Width = 200;
            this.panelNav.Controls.Add(this.btnDoctors);
            this.panelNav.Controls.Add(this.btnBilling);
            this.panelNav.Controls.Add(this.btnPrescriptions);
            this.panelNav.Controls.Add(this.btnTherapy);
            this.panelNav.Controls.Add(this.btnAppointments);
            this.panelNav.Controls.Add(this.btnPatients);
            this.panelNav.Controls.Add(this.btnDashboard);
            //
            // panelContent
            //
            this.panelContent.BackColor = Theme.Background;
            this.panelContent.BackgroundImageLayout = ImageLayout.Stretch;
            try { this.panelContent.BackgroundImage = Image.FromFile(@"Resources\bg_leaf.png"); } catch { }
            this.panelContent.Dock = DockStyle.Fill;
            this.panelContent.Padding = new Padding(16);
            //
            // nav buttons
            //
            ConfigureNavButton(this.btnDashboard, "  Dashboard", 0, "dashboard");
            ConfigureNavButton(this.btnPatients, "  Patients", 1, "patients");
            ConfigureNavButton(this.btnAppointments, "  Appointments", 2, "appointments");
            ConfigureNavButton(this.btnTherapy, "  Therapy Schedule", 3, "therapy");
            ConfigureNavButton(this.btnPrescriptions, "  Prescriptions", 4, "prescriptions");
            ConfigureNavButton(this.btnBilling, "  Billing", 5, "billing");
            ConfigureNavButton(this.btnDoctors, "  Doctors", 6, "doctors");
            //
            // MainForm
            //
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(1000, 640);
            this.Controls.Add(this.panelContent);
            this.Controls.Add(this.panelNav);
            this.Controls.Add(this.panelTop);
            this.MinimumSize = new Size(860, 560);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "AyuSwastha — Ayurveda Clinic Management System";
            this.Load += this.MainForm_Load;
            this.ResumeLayout(false);
        }

        /// <summary>Applies the shared look to a sidebar button and wires its click.</summary>
        private void ConfigureNavButton(Button b, string text, int index, string key)
        {
            b.Text = text;
            b.Dock = DockStyle.Top;
            b.Height = 46;
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 0;
            b.TextAlign = ContentAlignment.MiddleLeft;
            b.Padding = new Padding(12, 0, 0, 0);
            b.Font = new Font("Segoe UI", 10.5f);
            b.ForeColor = Theme.TextPrimary;
            b.BackColor = Theme.SidebarBg;
            b.Tag = key;
            b.Click += this.Nav_Click;
            // Docked-top buttons stack in reverse add order; TabIndex keeps ordering readable.
            b.TabIndex = index;
        }
    }
}
