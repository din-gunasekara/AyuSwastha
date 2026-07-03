using System.Drawing;
using System.Windows.Forms;
using AyuSwastha.Core;

namespace AyuSwastha.Forms
{
    partial class TherapiesControl
    {
        private System.ComponentModel.IContainer components = null;

        private Panel panelHeader;
        private Label lblTitle;
        private TextBox txtSearch;
        private Button btnNew;
        private Button btnEdit;
        private Button btnDelete;
        private Panel panelListHost;
        private DataGridView lstTherapies;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.panelHeader = new Panel();
            this.lblTitle = new Label();
            this.txtSearch = new TextBox();
            this.btnNew = new Button();
            this.btnEdit = new Button();
            this.btnDelete = new Button();
            this.panelListHost = new Panel();
            this.lstTherapies = new DataGridView();
            this.SuspendLayout();
            //
            // panelHeader
            //
            this.panelHeader.BackColor = Theme.Surface;
            this.panelHeader.Dock = DockStyle.Top;
            this.panelHeader.Height = 100;
            this.panelHeader.Controls.Add(this.lblTitle);
            this.panelHeader.Controls.Add(this.txtSearch);
            this.panelHeader.Controls.Add(this.btnNew);
            this.panelHeader.Controls.Add(this.btnEdit);
            this.panelHeader.Controls.Add(this.btnDelete);
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new Font("Segoe UI Semibold", 16f, FontStyle.Bold);
            this.lblTitle.ForeColor = Theme.TextPrimary;
            this.lblTitle.Location = new Point(20, 14);
            this.lblTitle.Text = "Therapy Sessions";
            //
            // txtSearch
            //
            this.txtSearch.Location = new Point(22, 56);
            this.txtSearch.Size = new Size(340, 27);
            this.txtSearch.Font = new Font("Segoe UI", 10f);
            //
            // btnNew
            //
            this.btnNew.BackColor = Theme.Primary;
            this.btnNew.FlatStyle = FlatStyle.Flat;
            this.btnNew.FlatAppearance.BorderSize = 0;
            this.btnNew.ForeColor = Color.White;
            this.btnNew.Font = new Font("Segoe UI Semibold", 9.5f);
            this.btnNew.Location = new Point(380, 52);
            this.btnNew.Size = new Size(96, 34);
            this.btnNew.Text = "+ New";
            this.btnNew.UseVisualStyleBackColor = false;
            //
            // btnEdit
            //
            this.btnEdit.FlatStyle = FlatStyle.Flat;
            this.btnEdit.FlatAppearance.BorderColor = Theme.Border;
            this.btnEdit.ForeColor = Theme.TextPrimary;
            this.btnEdit.Font = new Font("Segoe UI", 9.5f);
            this.btnEdit.Location = new Point(482, 52);
            this.btnEdit.Size = new Size(84, 34);
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            //
            // btnDelete
            //
            this.btnDelete.FlatStyle = FlatStyle.Flat;
            this.btnDelete.FlatAppearance.BorderColor = Theme.Border;
            this.btnDelete.ForeColor = Theme.Danger;
            this.btnDelete.Font = new Font("Segoe UI", 9.5f);
            this.btnDelete.Location = new Point(572, 52);
            this.btnDelete.Size = new Size(84, 34);
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            //
            // panelListHost
            //
            this.panelListHost.BackColor = Theme.Background;
            this.panelListHost.Dock = DockStyle.Fill;
            this.panelListHost.Padding = new Padding(20, 12, 20, 20);
            this.panelListHost.Controls.Add(this.lstTherapies);
            //
            // lstTherapies
            //
            this.lstTherapies.BorderStyle = BorderStyle.FixedSingle;
            this.lstTherapies.Dock = DockStyle.Fill;
            this.lstTherapies.Font = new Font("Segoe UI", 10.5f);
            this.lstTherapies.BackgroundColor = Theme.Surface;
            this.lstTherapies.ReadOnly = true;
            this.lstTherapies.AllowUserToAddRows = false;
            this.lstTherapies.AllowUserToDeleteRows = false;
            this.lstTherapies.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.lstTherapies.RowHeadersVisible = false;
            this.lstTherapies.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.lstTherapies.MultiSelect = false;
            //
            // TherapiesControl
            //
            this.BackColor = Theme.Background;
            this.Controls.Add(this.panelListHost);
            this.Controls.Add(this.panelHeader);
            this.Size = new Size(790, 520);
            this.Load += this.TherapiesControl_Load;
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
