using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using AyuSwastha.Core;
using AyuSwastha.Data;
using AyuSwastha.Models;

namespace AyuSwastha.Forms
{
    public partial class AppointmentsControl : UserControl
    {
        private readonly AppointmentRepository _repo = new AppointmentRepository();
        private List<Appointment> _all = new List<Appointment>();

        public AppointmentsControl()
        {
            InitializeComponent();

            txtSearch.TextChanged += (s, e) => ApplyFilter();
            btnNew.Click += (s, e) => OpenEditor(new Appointment { ScheduledAt = DateTime.Today.AddHours(9) });
            btnEdit.Click += (s, e) => EditSelected();
            btnDelete.Click += (s, e) => DeleteSelected();
            lstAppointments.DoubleClick += (s, e) => EditSelected();

            lstAppointments.AutoGenerateColumns = false;
            lstAppointments.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "AppointmentCode", HeaderText = "ID", Width = 80 });
            lstAppointments.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "PatientName", HeaderText = "Patient Name", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            lstAppointments.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DoctorName", HeaderText = "Doctor", Width = 150 });
            lstAppointments.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "AppointmentDate", HeaderText = "Scheduled At", Width = 150, DefaultCellStyle = new DataGridViewCellStyle { Format = "g" } });
            lstAppointments.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Status", HeaderText = "Status", Width = 100 });
        }

        private void AppointmentsControl_Load(object sender, EventArgs e)
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
                : _all.Where(a =>
                    (a.PatientName ?? "").ToLowerInvariant().Contains(q) ||
                    (a.DoctorName ?? "").ToLowerInvariant().Contains(q) ||
                    (a.Reason ?? "").ToLowerInvariant().Contains(q)).ToList();

            lstAppointments.DataSource = null;
            lstAppointments.DataSource = filtered;
        }

        private void EditSelected()
        {
            if (lstAppointments.CurrentRow?.DataBoundItem is Appointment a)
                OpenEditor(a);
        }

        private void OpenEditor(Appointment appointment)
        {
            int previousId = appointment.Id;
            using (var dlg = new AppointmentDetailForm(appointment))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    ReloadList();
                    SelectById(appointment.Id != 0 ? appointment.Id : previousId);
                }
            }
        }

        private void DeleteSelected()
        {
            if (!(lstAppointments.CurrentRow?.DataBoundItem is Appointment a) || a.Id == 0)
                return;

            if (MessageBox.Show("Delete appointment for " + a.PatientName + "?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            try
            {
                _repo.Delete(a.Id);
                ReloadList();
            }
            catch (AppException ex)
            {
                MessageBox.Show(ex.Message, "AyuSwastha", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SelectById(int id)
        {
            foreach (DataGridViewRow row in lstAppointments.Rows)
            {
                if (row.DataBoundItem is Appointment a && a.Id == id)
                {
                    row.Selected = true;
                    lstAppointments.CurrentCell = row.Cells[0];
                    return;
                }
            }
        }
    }
}
