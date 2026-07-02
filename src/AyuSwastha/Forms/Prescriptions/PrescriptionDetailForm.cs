using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using AyuSwastha.Core;
using AyuSwastha.Data;
using AyuSwastha.Models;
using AyuSwastha.Services;

namespace AyuSwastha.Forms
{
    public class PrescriptionDetailForm : Form
    {
        private readonly PrescriptionRepository _repo = new PrescriptionRepository();
        private readonly PatientRepository _patientRepo = new PatientRepository();
        private readonly DoctorRepository _doctorRepo = new DoctorRepository();
        private readonly DoshaRecommendationService _doshaService = new DoshaRecommendationService();
        private readonly Prescription _prescription;

        private ComboBox cmbPatient, cmbDoctor;
        private DateTimePicker dtpIssuedOn;
        private TextBox txtGeneralInstructions, txtDoshaGuidance;
        private DataGridView gridItems;

        public PrescriptionDetailForm(Prescription prescription)
        {
            _prescription = prescription ?? new Prescription { IssuedOn = DateTime.Now };
            BuildUi();
            Populate();
        }

        private void BuildUi()
        {
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(800, 600);
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
                Text = "Prescription Details",
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
            var body = new Panel { Dock = DockStyle.Fill, BackColor = Theme.Surface, Padding = new Padding(20) };

            txtDoshaGuidance = new TextBox { Width = 300, Height = 170, Multiline = true, ReadOnly = true, Font = new Font("Segoe UI", 9f), BackColor = Theme.Background, ScrollBars = ScrollBars.Vertical };

            cmbPatient = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 260, Font = Theme.Body, DisplayMember = "FullName", ValueMember = "Id" };
            cmbDoctor = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 260, Font = Theme.Body, DisplayMember = "FullName", ValueMember = "Id" };
            cmbPatient.SelectedIndexChanged += (s, e) => UpdateDoshaGuidance();

            try {
                cmbPatient.DataSource = _patientRepo.GetAll().ToList();
                cmbDoctor.DataSource = _doctorRepo.GetAll().ToList();
            } catch (Exception) { /* Ignored for designer/empty DB */ }

            dtpIssuedOn = new DateTimePicker { Width = 150, Format = DateTimePickerFormat.Short, Font = Theme.Body };
            txtGeneralInstructions = new TextBox { Width = 260, Multiline = true, Height = 60, Font = Theme.Body, ScrollBars = ScrollBars.Vertical };

            int y = 20;
            AddRow(body, "Patient", cmbPatient, ref y);
            AddRow(body, "Doctor", cmbDoctor, ref y);
            AddRow(body, "Issued On", dtpIssuedOn, ref y);
            AddRow(body, "Gen. Instr.", txtGeneralInstructions, ref y);

            // Dosha Guidance Panel
            var lblDosha = new Label { Text = "Dosha Guidance", AutoSize = true, Font = Theme.Subheading, ForeColor = Theme.Primary, Location = new Point(450, 24) };
            txtDoshaGuidance.Location = new Point(450, 50);
            body.Controls.Add(lblDosha);
            body.Controls.Add(txtDoshaGuidance);

            // Grid for items
            var lblGrid = new Label { Text = "Herbal Items", AutoSize = true, Font = Theme.Subheading, ForeColor = Theme.TextPrimary, Location = new Point(20, y + 10) };
            
            gridItems = new DataGridView
            {
                Location = new Point(20, y + 40),
                Width = 760,
                Height = 220,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = true,
                AllowUserToDeleteRows = true,
                BackgroundColor = Theme.Background,
                Font = Theme.Body
            };
            
            gridItems.Columns.Add(new DataGridViewTextBoxColumn { Name = "HerbName", HeaderText = "Herb Name", DataPropertyName = "HerbName" });
            gridItems.Columns.Add(new DataGridViewTextBoxColumn { Name = "Dosage", HeaderText = "Dosage", DataPropertyName = "Dosage" });
            gridItems.Columns.Add(new DataGridViewTextBoxColumn { Name = "Duration", HeaderText = "Duration", DataPropertyName = "Duration" });
            gridItems.Columns.Add(new DataGridViewTextBoxColumn { Name = "Instructions", HeaderText = "Instructions", DataPropertyName = "Instructions" });

            body.Controls.Add(lblGrid);
            body.Controls.Add(gridItems);

            return body;
        }

        private void AddRow(Panel page, string caption, Control input, ref int y)
        {
            var lbl = new Label
            {
                Text = caption, AutoSize = true, Font = Theme.Body, ForeColor = Theme.TextMuted,
                Location = new Point(20, y + 4)
            };
            input.Location = new Point(110, y);
            page.Controls.Add(lbl);
            page.Controls.Add(input);
            y += Math.Max(input.Height, 24) + 14;
        }

        private void UpdateDoshaGuidance()
        {
            if (cmbPatient.SelectedItem is Patient p)
            {
                var rec = _doshaService.GetRecommendation(p.Prakriti);
                var sb = new StringBuilder();
                sb.AppendLine("Prakriti: " + p.Prakriti.ToString());
                sb.AppendLine(rec.Summary).AppendLine();
                if (rec.RecommendedHerbs.Count > 0)
                {
                    sb.AppendLine("Suggested Herbs:");
                    foreach (var h in rec.RecommendedHerbs) sb.AppendLine("• " + h);
                }
                txtDoshaGuidance.Text = sb.ToString();
            }
            else
            {
                txtDoshaGuidance.Text = string.Empty;
            }
        }

        private void Populate()
        {
            if (_prescription.PatientId > 0) cmbPatient.SelectedValue = _prescription.PatientId;
            if (_prescription.DoctorId > 0) cmbDoctor.SelectedValue = _prescription.DoctorId;
            
            dtpIssuedOn.Value = _prescription.IssuedOn.Date;
            txtGeneralInstructions.Text = _prescription.GeneralInstructions;

            var bindingList = new System.ComponentModel.BindingList<PrescriptionItem>(_prescription.Items);
            gridItems.DataSource = bindingList;

            UpdateDoshaGuidance();
        }

        private void Save_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbPatient.SelectedValue == null) throw new ValidationException("Please select a patient.");
                if (cmbDoctor.SelectedValue == null) throw new ValidationException("Please select a doctor.");

                _prescription.PatientId = (int)cmbPatient.SelectedValue;
                _prescription.DoctorId = (int)cmbDoctor.SelectedValue;
                _prescription.IssuedOn = dtpIssuedOn.Value.Date;
                _prescription.GeneralInstructions = txtGeneralInstructions.Text.Trim();

                if (_prescription.Id == 0) _repo.Add(_prescription);
                else _repo.Update(_prescription);

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
