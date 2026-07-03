using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using AyuSwastha.Core;
using AyuSwastha.Data;
using AyuSwastha.Models;

namespace AyuSwastha.Forms
{
    public class TherapySessionForm : Form
    {
        private readonly TherapySessionRepository _repo = new TherapySessionRepository();
        private readonly PatientRepository _patientRepo = new PatientRepository();
        private readonly DoctorRepository _doctorRepo = new DoctorRepository();
        private readonly TherapySession _session;

        private ComboBox cmbTherapy, cmbPatient, cmbDoctor, cmbStatus;
        private DateTimePicker dtpDate, dtpTime;
        private TextBox txtNotes;

        public TherapySessionForm(TherapySession session)
        {
            _session = session ?? new TherapySession { ScheduledAt = DateTime.Today.AddHours(9) };
            BuildUi();
            Populate();
        }

        private void BuildUi()
        {
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(500, 520);
            BackColor = Theme.Surface;

            Controls.Add(BuildBody());
            Controls.Add(BuildActionBar());
            Controls.Add(BuildTitleBar());
        }

        private Panel BuildTitleBar()
        {
            var bar = new Panel { Dock = DockStyle.Top, Height = 44, BackColor = Theme.Primary };

            var title = new Label
            {
                Text = "Therapy Session Details",
                ForeColor = Color.White,
                Font = new Font("Segoe UI Semibold", 13f, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(16, 10)
            };

            var close = new Button
            {
                Text = "✕",
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11f),
                Size = new Size(44, 44),
                Dock = DockStyle.Right,
                TabStop = false
            };
            close.FlatAppearance.BorderSize = 0;
            close.FlatAppearance.MouseOverBackColor = Theme.PrimaryDark;
            close.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            bar.MouseDown += TitleBar_MouseDown;
            title.MouseDown += TitleBar_MouseDown;

            bar.Controls.Add(title);
            bar.Controls.Add(close);
            return bar;
        }

        private Panel BuildActionBar()
        {
            var bar = new Panel { Dock = DockStyle.Top, Height = 56, BackColor = Theme.Surface };

            var btnSave = new Button
            {
                Text = "💾  Save",
                BackColor = Theme.Primary,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 10f),
                Size = new Size(110, 36),
                Location = new Point(20, 10)
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += Save_Click;

            var btnCancel = new Button
            {
                Text = "Cancel",
                BackColor = Theme.Surface,
                ForeColor = Theme.TextPrimary,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10f),
                Size = new Size(90, 36),
                Location = new Point(140, 10)
            };
            btnCancel.FlatAppearance.BorderColor = Theme.Border;
            btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            var sep = new Panel { Dock = DockStyle.Bottom, Height = 1, BackColor = Theme.Border };

            bar.Controls.Add(btnSave);
            bar.Controls.Add(btnCancel);
            bar.Controls.Add(sep);
            return bar;
        }

        private Panel BuildBody()
        {
            var body = new Panel { Dock = DockStyle.Fill, BackColor = Theme.Surface, Padding = new Padding(20), AutoScroll = true };

            cmbTherapy = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 260, Font = Theme.Body, DisplayMember = "Name", ValueMember = "Id" };
            cmbPatient = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 260, Font = Theme.Body, DisplayMember = "FullName", ValueMember = "Id" };
            cmbDoctor = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 260, Font = Theme.Body, DisplayMember = "FullName", ValueMember = "Id" };
            
            try {
                cmbTherapy.DataSource = _repo.GetAllTherapies().ToList();
                cmbPatient.DataSource = _patientRepo.GetAll().ToList();
                
                var doctors = _doctorRepo.GetAll().ToList();
                doctors.Insert(0, new Doctor { Id = 0, FullName = "(None / Not required)" });
                cmbDoctor.DataSource = doctors;
            } catch (Exception) { /* Ignored for designer/empty DB */ }

            dtpDate = new DateTimePicker { Width = 150, Format = DateTimePickerFormat.Short, Font = Theme.Body };
            dtpTime = new DateTimePicker { Width = 100, Format = DateTimePickerFormat.Custom, CustomFormat = "HH:mm", ShowUpDown = true, Font = Theme.Body };
            
            var timePanel = new Panel { Width = 260, Height = 30 };
            timePanel.Controls.Add(dtpDate);
            dtpTime.Location = new Point(160, 0);
            timePanel.Controls.Add(dtpTime);

            txtNotes = new TextBox { Width = 260, Multiline = true, Height = 80, Font = Theme.Body };
            
            cmbStatus = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 150, Font = Theme.Body };
            cmbStatus.DataSource = Enum.GetValues(typeof(TherapyStatus));

            int y = 20;
            AddRow(body, "Therapy", cmbTherapy, ref y);
            AddRow(body, "Patient", cmbPatient, ref y);
            AddRow(body, "Supervising Doctor", cmbDoctor, ref y);
            AddRow(body, "Date & Time", timePanel, ref y);
            AddRow(body, "Status", cmbStatus, ref y);
            AddRow(body, "Notes", txtNotes, ref y);

            return body;
        }

        private void AddRow(Panel page, string caption, Control input, ref int y)
        {
            var lbl = new Label
            {
                Text = caption, AutoSize = true, Font = Theme.Body, ForeColor = Theme.TextMuted,
                Location = new Point(20, y + 4)
            };
            input.Location = new Point(160, y);
            page.Controls.Add(lbl);
            page.Controls.Add(input);
            y += Math.Max(input.Height, 24) + 14;
        }

        private void Populate()
        {
            if (_session.TherapyId > 0) cmbTherapy.SelectedValue = _session.TherapyId;
            else if (cmbTherapy.Items.Count > 0) cmbTherapy.SelectedIndex = 0;

            if (_session.PatientId > 0) cmbPatient.SelectedValue = _session.PatientId;
            else if (cmbPatient.Items.Count > 0) cmbPatient.SelectedIndex = 0;

            if (_session.DoctorId > 0) cmbDoctor.SelectedValue = _session.DoctorId;
            else if (cmbDoctor.Items.Count > 0) cmbDoctor.SelectedIndex = 0; // None
            
            dtpDate.Value = _session.ScheduledAt.Date;
            dtpTime.Value = _session.ScheduledAt;
            cmbStatus.SelectedItem = _session.Status;
            txtNotes.Text = _session.Notes;
        }

        private void Save_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbTherapy.SelectedValue == null) throw new ValidationException("Please select a therapy.");
                if (cmbPatient.SelectedValue == null) throw new ValidationException("Please select a patient.");

                _session.TherapyId = (int)cmbTherapy.SelectedValue;
                _session.PatientId = (int)cmbPatient.SelectedValue;
                
                int docId = (int)cmbDoctor.SelectedValue;
                _session.DoctorId = docId > 0 ? (int?)docId : null;

                _session.ScheduledAt = dtpDate.Value.Date.Add(dtpTime.Value.TimeOfDay);
                _session.Status = (TherapyStatus)cmbStatus.SelectedItem;
                _session.Notes = txtNotes.Text.Trim();

                if (_session.Id == 0) _repo.Add(_session);
                else _repo.Update(_session);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (ValidationException ex)
            {
                MessageBox.Show(ex.Message, "Please check the form", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (AppException ex)
            {
                MessageBox.Show(ex.Message, "AyuSwastha", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        private void TitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            const int WM_NCLBUTTONDOWN = 0xA1;
            const int HTCAPTION = 0x2;
            ReleaseCapture();
            SendMessage(Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
        }
    }
}
