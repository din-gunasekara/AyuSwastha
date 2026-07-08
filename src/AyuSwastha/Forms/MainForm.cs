using System;
using System.Windows.Forms;
using AyuSwastha.Core;
using AyuSwastha.Services;

namespace AyuSwastha.Forms
{
    public partial class MainForm : Form
    {
        private Button _activeNav;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            var user = Session.CurrentUser;
            lblUser.Text = user != null
                ? "Signed in as " + (user.DisplayName ?? user.Username) + "  •  " + user.Role
                : string.Empty;

            ShowView("dashboard", btnDashboard);
        }

        private void Nav_Click(object sender, EventArgs e)
        {
            var button = (Button)sender;
            ShowView((string)button.Tag, button);
        }

        private void ShowView(string key, Button navButton)
        {
            HighlightNav(navButton);
            panelContent.Controls.Clear();

            UserControl view;
            switch (key)
            {
                case "dashboard":
                    view = new DashboardControl();
                    break;
                case "patients":
                    view = new PatientsControl();
                    break;
                case "appointments":
                    view = new AppointmentsControl();
                    break;
                case "therapy":
                    view = new TherapiesControl();
                    break;
                case "billing":
                    view = new BillingControl();
                    break;
                case "doctors":
                    view = new DoctorsControl();
                    break;
                default:
                    view = new PlaceholderControl("Coming soon", "This module is not available yet.");
                    break;
            }

            view.Dock = DockStyle.Fill;
            panelContent.Controls.Add(view);
        }

        private void HighlightNav(Button navButton)
        {
            if (_activeNav != null)
            {
                _activeNav.BackColor = Theme.SidebarBg;
                _activeNav.ForeColor = Theme.TextPrimary;
                _activeNav.Font = new System.Drawing.Font("Segoe UI", 10.5f);
            }

            if (navButton != null)
            {
                navButton.BackColor = Theme.PrimaryLight;
                navButton.ForeColor = System.Drawing.Color.White;
                navButton.Font = new System.Drawing.Font("Segoe UI Semibold", 10.5f);
            }
            _activeNav = navButton;
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Log out of AyuSwastha?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            Session.Clear();
            Application.Restart();
        }
    }
}
