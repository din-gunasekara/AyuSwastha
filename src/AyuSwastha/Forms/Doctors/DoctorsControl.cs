using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using AyuSwastha.Core;
using AyuSwastha.Data;
using AyuSwastha.Models;

namespace AyuSwastha.Forms
{
    public partial class DoctorsControl : UserControl
    {
        private readonly DoctorRepository _repo = new DoctorRepository();
        private List<Doctor> _all = new List<Doctor>();

        public DoctorsControl()
        {
            InitializeComponent();

            txtSearch.TextChanged += (s, e) => ApplyFilter();
            btnNew.Click += (s, e) => OpenEditor(new Doctor());
            btnEdit.Click += (s, e) => EditSelected();
            btnDelete.Click += (s, e) => DeleteSelected();
            lstDoctors.DoubleClick += (s, e) => EditSelected();

            lstDoctors.AutoGenerateColumns = false;
            lstDoctors.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DoctorCode", HeaderText = "ID", Width = 80 });
            lstDoctors.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "LicenseNo", HeaderText = "License No.", Width = 100 });
            lstDoctors.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "FullName", HeaderText = "Doctor Name", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            lstDoctors.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Specialization", HeaderText = "Specialization", Width = 150 });
            lstDoctors.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Phone", HeaderText = "Phone", Width = 120 });
        }

        private void DoctorsControl_Load(object sender, EventArgs e)
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
                    (p.DoctorCode ?? "").ToLowerInvariant().Contains(q) ||
                    (p.Specialization ?? "").ToLowerInvariant().Contains(q)).ToList();

            lstDoctors.DataSource = null;
            lstDoctors.DataSource = filtered;
        }

        private void EditSelected()
        {
            if (lstDoctors.CurrentRow?.DataBoundItem is Doctor p)
                OpenEditor(p);
        }

        private void OpenEditor(Doctor doctor)
        {
            int previousId = doctor.Id;
            using (var dlg = new DoctorDetailForm(doctor))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    ReloadList();
                    SelectById(doctor.Id != 0 ? doctor.Id : previousId);
                }
            }
        }

        private void DeleteSelected()
        {
            if (!(lstDoctors.CurrentRow?.DataBoundItem is Doctor p) || p.Id == 0)
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
            foreach (DataGridViewRow row in lstDoctors.Rows)
            {
                if (row.DataBoundItem is Doctor p && p.Id == id)
                {
                    row.Selected = true;
                    lstDoctors.CurrentCell = row.Cells[0];
                    return;
                }
            }
        }
    }
}
