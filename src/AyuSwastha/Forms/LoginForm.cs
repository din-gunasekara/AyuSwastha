using System;
using System.Windows.Forms;
using AyuSwastha.Core;
using AyuSwastha.Services;

namespace AyuSwastha.Forms
{
    public partial class LoginForm : Form
    {
        private readonly AuthService _auth = new AuthService();

        public LoginForm()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            lblError.Text = string.Empty;
            try
            {
                _auth.Login(txtUsername.Text, txtPassword.Text);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (AppException ex)
            {
                // Expected, user-facing errors (validation / auth / data).
                lblError.Text = ex.Message;
                txtPassword.SelectAll();
                txtPassword.Focus();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
