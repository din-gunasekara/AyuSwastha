using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using AyuSwastha.Core;
using AyuSwastha.Data;
using AyuSwastha.Models;

namespace AyuSwastha.Forms
{
    public partial class PrescriptionsControl : UserControl
    {
        private readonly PrescriptionRepository _repo = new PrescriptionRepository();
        private List<Prescription> _all = new List<Prescription>();

        public PrescriptionsControl()
        {
            InitializeComponent();

            txtSearch.TextChanged += (s, e) => ApplyFilter();
            btnNew.Click += (s, e) => OpenEditor(new Prescription { IssuedOn = DateTime.Now });
            btnEdit.Click += (s, e) => EditSelected();
            btnDelete.Click += (s, e) => DeleteSelected();
            lstPrescriptions.DoubleClick += (s, e) => EditSelected();

            lstPrescriptions.AutoGenerateColumns = false;
            lstPrescriptions.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "PrescriptionCode", HeaderText = "ID", Width = 80 });
            lstPrescriptions.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "PatientName", HeaderText = "Patient Name", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            lstPrescriptions.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DoctorName", HeaderText = "Doctor", Width = 150 });
            lstPrescriptions.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "IssuedOn", HeaderText = "Date", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "d" } });
        }

        private void PrescriptionsControl_Load(object sender, EventArgs e)
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
                : _all.Where(p =>
                    (p.PatientName ?? "").ToLowerInvariant().Contains(q) ||
                    (p.DoctorName ?? "").ToLowerInvariant().Contains(q)).ToList();

            lstPrescriptions.DataSource = null;
            lstPrescriptions.DataSource = filtered;
        }

        private void EditSelected()
        {
            if (lstPrescriptions.CurrentRow?.DataBoundItem is Prescription p)
                OpenEditor(p);
        }

        private void OpenEditor(Prescription prescription)
        {
            int previousId = prescription.Id;
            using (var dlg = new PrescriptionDetailForm(prescription))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    ReloadList();
                    SelectById(prescription.Id != 0 ? prescription.Id : previousId);
                }
            }
        }

        private void DeleteSelected()
        {
            if (!(lstPrescriptions.CurrentRow?.DataBoundItem is Prescription p) || p.Id == 0)
                return;

            if (MessageBox.Show("Delete prescription for " + p.PatientName + "?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            try
            {
                _repo.Delete(p.Id);
                ReloadList();
            }
            catch (AppException ex)
            {
                MessageBox.Show(ex.Message, "AyuSwastha", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SelectById(int id)
        {
            foreach (DataGridViewRow row in lstPrescriptions.Rows)
            {
                if (row.DataBoundItem is Prescription p && p.Id == id)
                {
                    row.Selected = true;
                    lstPrescriptions.CurrentCell = row.Cells[0];
                    return;
                }
            }
        }
    }
}
