using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using AyuSwastha.Core;
using AyuSwastha.Data;
using AyuSwastha.Models;

namespace AyuSwastha.Forms
{
    public partial class PatientsControl : UserControl
    {
        private readonly PatientRepository _repo = new PatientRepository();
        private List<Patient> _all = new List<Patient>();

        public PatientsControl()
        {
            InitializeComponent();

            txtSearch.TextChanged += (s, e) => ApplyFilter();
            btnNew.Click += (s, e) => OpenEditor(new Patient());
            btnEdit.Click += (s, e) => EditSelected();
            btnDelete.Click += (s, e) => DeleteSelected();
            lstPatients.DoubleClick += (s, e) => EditSelected();

            lstPatients.AutoGenerateColumns = false;
            lstPatients.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "PatientCode", HeaderText = "ID", Width = 80 });
            lstPatients.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "FullName", HeaderText = "Patient Name", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            lstPatients.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Gender", HeaderText = "Gender", Width = 100 });
            lstPatients.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Age", HeaderText = "Age", Width = 80 });
            lstPatients.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Prakriti", HeaderText = "Dosha (Prakriti)", Width = 120 });
            lstPatients.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Phone", HeaderText = "Phone", Width = 120 });
        }

        private void PatientsControl_Load(object sender, EventArgs e)
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
                    (p.FullName ?? "").ToLowerInvariant().Contains(q) ||
                    (p.PatientCode ?? "").ToLowerInvariant().Contains(q) ||
                    (p.Phone ?? "").ToLowerInvariant().Contains(q)).ToList();

            lstPatients.DataSource = null;
            lstPatients.DataSource = filtered;
        }

        private void EditSelected()
        {
            if (lstPatients.CurrentRow?.DataBoundItem is Patient p)
                OpenEditor(p);
        }

        private void OpenEditor(Patient patient)
        {
            int previousId = patient.Id;
            using (var dlg = new PatientDetailForm(patient))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    ReloadList();
                    SelectById(patient.Id != 0 ? patient.Id : previousId);
                }
            }
        }

        private void DeleteSelected()
        {
            if (!(lstPatients.CurrentRow?.DataBoundItem is Patient p) || p.Id == 0)
                return;

            if (MessageBox.Show("Delete " + p.FullName + "?", "Confirm",
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
            foreach (DataGridViewRow row in lstPatients.Rows)
            {
                if (row.DataBoundItem is Patient p && p.Id == id)
                {
                    row.Selected = true;
                    lstPatients.CurrentCell = row.Cells[0];
                    return;
                }
            }
        }
    }
}
