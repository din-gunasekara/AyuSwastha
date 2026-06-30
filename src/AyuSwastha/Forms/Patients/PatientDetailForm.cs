using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using AyuSwastha.Core;
using AyuSwastha.Data;
using AyuSwastha.Models;
using AyuSwastha.Services;

namespace AyuSwastha.Forms
{
    /// <summary>
    /// Tabbed patient editor styled after the design mockup: green title bar with
    /// Save/Cancel, a left tab strip, centre form, and a photo panel on the right.
    /// Saves through <see cref="PatientRepository"/> and returns <c>DialogResult.OK</c>.
    /// </summary>
    public class PatientDetailForm : Form
    {
        private readonly PatientRepository _repo = new PatientRepository();
        private readonly DoshaRecommendationService _dosha = new DoshaRecommendationService();
        private readonly Patient _patient;
        private string _photoPath;

        // Tab strip
        private readonly List<Button> _tabButtons = new List<Button>();
        private readonly List<Panel> _tabPages = new List<Panel>();
        private Panel _formHost;

        // Personal Info fields
        private TextBox txtCode, txtName, txtPhone, txtAddress;
        private ComboBox cmbGender, cmbPrakriti;
        private Label lblAgeValue;
        private DateTimePicker dtpDob;

        // Other tabs
        private TextBox txtHistory, txtAllergies, txtLifestyle, txtNotes, txtRec;
        private PictureBox picPhoto;

        public PatientDetailForm(Patient patient)
        {
            _patient = patient ?? new Patient();
            _photoPath = _patient.PhotoPath;
            BuildUi();
            Populate();
        }

        // ---------- UI construction ----------

        private void BuildUi()
        {
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(860, 520);
            BackColor = Theme.Surface;

            Controls.Add(BuildBody());     // fill (added first so it sits under the docked bars)
            Controls.Add(BuildActionBar()); // top
            Controls.Add(BuildTitleBar());  // top-most
        }

        private Panel BuildTitleBar()
        {
            var bar = new Panel { Dock = DockStyle.Top, Height = 44, BackColor = Theme.Primary };

            var title = new Label
            {
                Text = "Patient Details",
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

            // Let the user drag the window by the title bar.
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
            var body = new Panel { Dock = DockStyle.Fill, BackColor = Theme.Surface };

            body.Controls.Add(BuildFormHost());  // fill
            body.Controls.Add(BuildPhotoPanel()); // right
            body.Controls.Add(BuildTabStrip());   // left
            return body;
        }

        private Panel BuildTabStrip()
        {
            var strip = new Panel { Dock = DockStyle.Left, Width = 160, BackColor = Theme.SidebarBg };
            string[] names = { "Personal Info", "Medical History", "Allergies", "Lifestyle", "Notes", "★ Dosha Guidance" };

            // Add in reverse so docked-top order reads top-to-bottom.
            for (int i = names.Length - 1; i >= 0; i--)
            {
                int index = i;
                var b = new Button
                {
                    Text = "  " + names[i],
                    Dock = DockStyle.Top,
                    Height = 46,
                    FlatStyle = FlatStyle.Flat,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Font = new Font("Segoe UI", 10f),
                    ForeColor = Theme.TextPrimary,
                    BackColor = Theme.SidebarBg
                };
                b.FlatAppearance.BorderSize = 0;
                b.Click += (s, e) => ShowTab(index);
                _tabButtons.Insert(0, b);
                strip.Controls.Add(b);
            }
            return strip;
        }

        private Panel BuildPhotoPanel()
        {
            var panel = new Panel { Dock = DockStyle.Right, Width = 200, BackColor = Theme.Surface, Padding = new Padding(16) };

            picPhoto = new PictureBox
            {
                Size = new Size(160, 160),
                Location = new Point(20, 24),
                BackColor = Theme.Background,
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom
            };

            var btnUpload = new Button
            {
                Text = "Upload Photo",
                Location = new Point(20, 200),
                Size = new Size(160, 34),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5f),
                ForeColor = Theme.TextPrimary,
                BackColor = Theme.Surface
            };
            btnUpload.FlatAppearance.BorderColor = Theme.Border;
            btnUpload.Click += Upload_Click;

            var btnRemove = new Button
            {
                Text = "Remove",
                Location = new Point(20, 242),
                Size = new Size(160, 34),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5f),
                ForeColor = Theme.TextPrimary,
                BackColor = Theme.Surface
            };
            btnRemove.FlatAppearance.BorderColor = Theme.Border;
            btnRemove.Click += (s, e) => { _photoPath = null; picPhoto.Image = null; };

            panel.Controls.Add(picPhoto);
            panel.Controls.Add(btnUpload);
            panel.Controls.Add(btnRemove);
            return panel;
        }

        private Panel BuildFormHost()
        {
            _formHost = new Panel { Dock = DockStyle.Fill, BackColor = Theme.Surface, Padding = new Padding(24, 18, 24, 18) };

            _tabPages.Add(BuildPersonalPage());
            _tabPages.Add(BuildMemoPage(out txtHistory, "Record past conditions, treatments and diagnoses."));
            _tabPages.Add(BuildMemoPage(out txtAllergies, "List known allergies and adverse reactions."));
            _tabPages.Add(BuildMemoPage(out txtLifestyle, "Diet, sleep, activity, occupation and habits."));
            _tabPages.Add(BuildMemoPage(out txtNotes, "General clinical notes."));
            _tabPages.Add(BuildDoshaPage());

            foreach (var page in _tabPages)
            {
                page.Dock = DockStyle.Fill;
                page.Visible = false;
                _formHost.Controls.Add(page);
            }
            return _formHost;
        }

        private Panel BuildPersonalPage()
        {
            var page = new Panel { AutoScroll = true };

            txtCode = MakeText(readOnly: true, width: 150);
            txtName = MakeText(width: 260);
            cmbGender = MakeCombo(typeof(Gender), 160);
            dtpDob = new DateTimePicker { Width = 200, Format = DateTimePickerFormat.Short, ShowCheckBox = true, Font = Theme.Body };
            dtpDob.ValueChanged += (s, e) => UpdateAge();
            txtPhone = MakeText(width: 200);
            txtAddress = MakeText(width: 260, multiline: true, height: 56);
            cmbPrakriti = MakeCombo(typeof(DoshaType), 200);
            if (txtRec == null) {
                txtRec = new TextBox
                {
                    Dock = DockStyle.Fill, Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Vertical,
                    Font = Theme.Body, BorderStyle = BorderStyle.FixedSingle, BackColor = Theme.Surface
                };
            }
            cmbPrakriti.SelectedIndexChanged += (s, e) => UpdateRecommendation();
            lblAgeValue = new Label { AutoSize = true, Font = new Font("Segoe UI Semibold", 9.75f), ForeColor = Theme.TextPrimary };

            int y = 8;
            AddRow(page, "Patient ID", txtCode, ref y);
            AddRow(page, "Full Name", txtName, ref y);
            AddRow(page, "Gender", cmbGender, ref y);
            AddRow(page, "Age", lblAgeValue, ref y);
            AddRow(page, "Date of Birth", dtpDob, ref y);
            AddRow(page, "Phone", txtPhone, ref y);
            AddRow(page, "Address", txtAddress, ref y);
            AddRow(page, "Prakriti (Dosha)", cmbPrakriti, ref y);
            return page;
        }

        private Panel BuildMemoPage(out TextBox box, string hint)
        {
            var page = new Panel();
            var lbl = new Label
            {
                Dock = DockStyle.Top, Height = 28, Text = hint,
                Font = Theme.Body, ForeColor = Theme.TextMuted
            };
            box = new TextBox
            {
                Dock = DockStyle.Fill, Multiline = true, ScrollBars = ScrollBars.Vertical,
                Font = Theme.Body, BorderStyle = BorderStyle.FixedSingle
            };
            page.Controls.Add(box);
            page.Controls.Add(lbl);
            return page;
        }

        private Panel BuildDoshaPage()
        {
            var page = new Panel();
            var lbl = new Label
            {
                Dock = DockStyle.Top, Height = 30, Text = "Ayurvedic guidance for the selected Prakriti",
                Font = Theme.Subheading, ForeColor = Theme.Primary
            };
            if (txtRec == null) {
                txtRec = new TextBox
                {
                    Dock = DockStyle.Fill, Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Vertical,
                    Font = Theme.Body, BorderStyle = BorderStyle.FixedSingle, BackColor = Theme.Surface
                };
            }
            page.Controls.Add(txtRec);
            page.Controls.Add(lbl);
            return page;
        }

        // ---------- small builders ----------

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
                Location = new Point(4, y + 4)
            };
            input.Location = new Point(150, y);
            page.Controls.Add(lbl);
            page.Controls.Add(input);
            y += Math.Max(input.Height, 24) + 14;
        }

        // ---------- behaviour ----------

        private void ShowTab(int index)
        {
            for (int i = 0; i < _tabPages.Count; i++)
                _tabPages[i].Visible = (i == index);

            for (int i = 0; i < _tabButtons.Count; i++)
            {
                bool active = (i == index);
                _tabButtons[i].BackColor = active ? Theme.Surface : Theme.SidebarBg;
                _tabButtons[i].ForeColor = active ? Theme.Primary : Theme.TextPrimary;
                _tabButtons[i].Font = new Font("Segoe UI" + (active ? " Semibold" : ""), 10f);
            }
        }

        private void Populate()
        {
            txtCode.Text = string.IsNullOrEmpty(_patient.PatientCode) ? "(auto)" : _patient.PatientCode;
            txtName.Text = _patient.FullName;
            cmbGender.SelectedItem = _patient.Gender;
            cmbPrakriti.SelectedItem = _patient.Prakriti;
            txtPhone.Text = _patient.Phone;
            txtAddress.Text = _patient.Address;
            txtHistory.Text = _patient.MedicalHistory;
            txtAllergies.Text = _patient.Allergies;
            txtLifestyle.Text = _patient.Lifestyle;
            txtNotes.Text = _patient.Notes;

            if (_patient.DateOfBirth.HasValue) { dtpDob.Value = _patient.DateOfBirth.Value; dtpDob.Checked = true; }
            else dtpDob.Checked = false;

            LoadPhoto(_photoPath);
            UpdateAge();
            UpdateRecommendation();
            ShowTab(0);
        }

        private void UpdateAge()
        {
            if (!dtpDob.Checked) { lblAgeValue.Text = "—"; return; }
            var probe = new Patient { DateOfBirth = dtpDob.Value.Date };
            lblAgeValue.Text = probe.Age.HasValue ? probe.Age.Value + " yrs" : "—";
        }

        private void UpdateRecommendation()
        {
            if (!(cmbPrakriti.SelectedItem is DoshaType dosha)) return;
            DoshaRecommendation rec = _dosha.GetRecommendation(dosha);

            var sb = new StringBuilder();
            sb.AppendLine(rec.Summary).AppendLine();
            AppendSection(sb, "Herbs", rec.RecommendedHerbs);
            AppendSection(sb, "Therapies", rec.RecommendedTherapies);
            AppendSection(sb, "Diet", rec.DietTips);
            AppendSection(sb, "Lifestyle", rec.LifestyleTips);
            txtRec.Text = sb.ToString();
        }

        private static void AppendSection(StringBuilder sb, string heading, IReadOnlyList<string> items)
        {
            if (items == null || items.Count == 0) return;
            sb.AppendLine(heading + ":");
            foreach (var item in items) sb.AppendLine("  • " + item);
            sb.AppendLine();
        }

        private void Upload_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog
            {
                Title = "Select patient photo",
                Filter = "Image files|*.jpg;*.jpeg;*.png;*.bmp|All files|*.*"
            })
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    _photoPath = dlg.FileName;
                    LoadPhoto(_photoPath);
                }
            }
        }

        private void LoadPhoto(string path)
        {
            try
            {
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    // Load via a stream so the file is not locked on disk.
                    using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                        picPhoto.Image = Image.FromStream(fs);
                }
                else
                {
                    picPhoto.Image = null;
                }
            }
            catch
            {
                picPhoto.Image = null; // ignore unreadable images
            }
        }

        private void Save_Click(object sender, EventArgs e)
        {
            try
            {
                _patient.FullName = txtName.Text.Trim();
                _patient.Gender = (Gender)cmbGender.SelectedItem;
                _patient.Prakriti = (DoshaType)cmbPrakriti.SelectedItem;
                _patient.Phone = txtPhone.Text.Trim();
                _patient.Address = txtAddress.Text.Trim();
                _patient.MedicalHistory = txtHistory.Text.Trim();
                _patient.Allergies = txtAllergies.Text.Trim();
                _patient.Lifestyle = txtLifestyle.Text.Trim();
                _patient.Notes = txtNotes.Text.Trim();
                _patient.PhotoPath = _photoPath;
                _patient.DateOfBirth = dtpDob.Checked ? dtpDob.Value.Date : (DateTime?)null;

                if (_patient.Id == 0) _repo.Add(_patient);
                else _repo.Update(_patient);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (ValidationException ex)
            {
                MessageBox.Show(ex.Message, "Please check the form", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ShowTab(0);
            }
            catch (AppException ex)
            {
                MessageBox.Show(ex.Message, "AyuSwastha", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ---------- draggable borderless window ----------

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
