using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using AyuSwastha.Core;
using AyuSwastha.Data;
using AyuSwastha.Models;

namespace AyuSwastha.Forms
{
    public partial class BillingControl : UserControl
    {
        private readonly InvoiceRepository _repo = new InvoiceRepository();
        private List<Invoice> _all = new List<Invoice>();

        public BillingControl()
        {
            InitializeComponent();

            txtSearch.TextChanged += (s, e) => ApplyFilter();
            btnNew.Click += (s, e) => OpenEditor(new Invoice { IssuedAt = DateTime.Now, Status = InvoiceStatus.Draft });
            btnEdit.Click += (s, e) => EditSelected();
            btnDelete.Click += (s, e) => DeleteSelected();
            lstInvoices.DoubleClick += (s, e) => EditSelected();

            lstInvoices.AutoGenerateColumns = false;
            lstInvoices.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "InvoiceCode", HeaderText = "Invoice Code", Width = 120 });
            lstInvoices.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "PatientName", HeaderText = "Patient", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            lstInvoices.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TotalAmount", HeaderText = "Amount", Width = 120, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });
            lstInvoices.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "IssuedAt", HeaderText = "Date", Width = 120, DefaultCellStyle = new DataGridViewCellStyle { Format = "d" } });
            lstInvoices.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Status", HeaderText = "Status", Width = 100 });
        }

        private void BillingControl_Load(object sender, EventArgs e)
        {
            ReloadList();
        }

        private void ReloadList()
        {
            try
            {
                _all = _repo.GetAll().ToList();
                ApplyFilter();
            }
            catch (AppException ex)
            {
                MessageBox.Show(ex.Message, "AyuSwastha", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ApplyFilter()
        {
            string q = txtSearch.Text.Trim().ToLowerInvariant();
            var filtered = string.IsNullOrEmpty(q)
                ? _all
                : _all.Where(i =>
                    (i.PatientName ?? "").ToLowerInvariant().Contains(q) ||
                    i.Status.ToString().ToLowerInvariant().Contains(q)).ToList();

            lstInvoices.DataSource = null;
            lstInvoices.DataSource = filtered;
        }

        private void EditSelected()
        {
            if (lstInvoices.CurrentRow?.DataBoundItem is Invoice i)
                OpenEditor(i);
        }

        private void OpenEditor(Invoice invoice)
        {
            int previousId = invoice.Id;
            using (var dlg = new InvoiceDetailForm(invoice))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    ReloadList();
                    SelectById(invoice.Id != 0 ? invoice.Id : previousId);
                }
            }
        }

        private void DeleteSelected()
        {
            if (!(lstInvoices.CurrentRow?.DataBoundItem is Invoice i) || i.Id == 0)
                return;

            if (MessageBox.Show("Delete invoice for " + i.PatientName + "?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            try
            {
                _repo.Delete(i.Id);
                ReloadList();
            }
            catch (AppException ex)
            {
                MessageBox.Show(ex.Message, "AyuSwastha", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SelectById(int id)
        {
            foreach (DataGridViewRow row in lstInvoices.Rows)
            {
                if (row.DataBoundItem is Invoice inv && inv.Id == id)
                {
                    row.Selected = true;
                    lstInvoices.CurrentCell = row.Cells[0];
                    return;
                }
            }
        }
    }
}
