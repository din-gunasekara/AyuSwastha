using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using AyuSwastha.Core;
using AyuSwastha.Data;
using AyuSwastha.Models;

namespace AyuSwastha.Forms
{
    public class DoctorDetailForm : Form
    {
        private readonly DoctorRepository _repo = new DoctorRepository();
        private readonly Doctor _doctor;

        private TextBox txtCode, txtName, txtPhone, txtAddress, txtSpecialization, txtLicense, txtFee;
        private ComboBox cmbGender;

        public DoctorDetailForm(Doctor doctor)
        {
            _doctor = doctor ?? new Doctor();
            BuildUi();
            Populate();
        }

        private void BuildUi()
        {
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(500, 500);
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
                Text = "Doctor Details",
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

            txtCode = MakeText(readOnly: true, width: 150);
            txtName = MakeText(width: 260);
            cmbGender = MakeCombo(typeof(Gender), 160);
            txtPhone = MakeText(width: 200);
            txtAddress = MakeText(width: 260, multiline: true, height: 56);
            txtSpecialization = MakeText(width: 260);
            txtLicense = MakeText(width: 200);
            txtFee = MakeText(width: 150);

            int y = 20;
            AddRow(body, "Doctor ID", txtCode, ref y);
            AddRow(body, "Full Name", txtName, ref y);
            AddRow(body, "Gender", cmbGender, ref y);
            AddRow(body, "Phone", txtPhone, ref y);
            AddRow(body, "Address", txtAddress, ref y);
            AddRow(body, "Specialization", txtSpecialization, ref y);
            AddRow(body, "License No", txtLicense, ref y);
            AddRow(body, "Consultation Fee", txtFee, ref y);

            return body;
        }

        private TextBox MakeText(bool readOnly = false, int width = 200, bool multiline = false, int height = 24)
        {
            return new TextBox
            {
                ReadOnly = readOnly,
                Width = width,
                Multiline = multiline,
                Height = multiline ? height : 24,
                Font = Theme.Body,
                BackColor = readOnly ? Theme.Background : Color.White
            };
        }

        private ComboBox MakeCombo(Type enumType, int width)
        {
            return new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = width,
                Font = Theme.Body,
                DataSource = Enum.GetValues(enumType)
            };
        }

        private void AddRow(Panel page, string caption, Control input, ref int y)
        {
            var lbl = new Label
            {
                Text = caption, AutoSize = true, Font = Theme.Body, ForeColor = Theme.TextMuted,
                Location = new Point(20, y + 4)
            };
            input.Location = new Point(150, y);
            page.Controls.Add(lbl);
            page.Controls.Add(input);
            y += Math.Max(input.Height, 24) + 14;
        }

        private void Populate()
        {
            txtCode.Text = string.IsNullOrEmpty(_doctor.DoctorCode) ? "(auto)" : _doctor.DoctorCode;
            txtName.Text = _doctor.FullName;
            cmbGender.SelectedItem = _doctor.Gender;
            txtPhone.Text = _doctor.Phone;
            txtAddress.Text = _doctor.Address;
            txtSpecialization.Text = _doctor.Specialization;
            txtLicense.Text = _doctor.LicenseNo;
            txtFee.Text = _doctor.ConsultationFee.ToString("F2");
        }

        private void Save_Click(object sender, EventArgs e)
        {
            try
            {
                _doctor.FullName = txtName.Text.Trim();
                _doctor.Gender = (Gender)cmbGender.SelectedItem;
                _doctor.Phone = txtPhone.Text.Trim();
                _doctor.Address = txtAddress.Text.Trim();
                _doctor.Specialization = txtSpecialization.Text.Trim();
                _doctor.LicenseNo = txtLicense.Text.Trim();
                if (decimal.TryParse(txtFee.Text, out decimal fee))
                    _doctor.ConsultationFee = fee;
                else
                    throw new ValidationException("Consultation Fee must be a valid number.");

                if (_doctor.Id == 0) _repo.Add(_doctor);
                else _repo.Update(_doctor);

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
