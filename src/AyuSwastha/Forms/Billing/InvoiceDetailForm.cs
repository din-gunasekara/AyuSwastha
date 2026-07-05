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
    public class InvoiceDetailForm : Form
    {
        private readonly InvoiceRepository _repo = new InvoiceRepository();
        private readonly PatientRepository _patientRepo = new PatientRepository();
        private readonly Invoice _invoice;

        private ComboBox cmbPatient, cmbStatus;
        private DateTimePicker dtpIssuedAt;
        private TextBox txtNotes, txtSubTotal, txtTax, txtTotal;
        private DataGridView gridItems;

        public InvoiceDetailForm(Invoice invoice)
        {
            _invoice = invoice ?? new Invoice { IssuedAt = DateTime.Now, Status = InvoiceStatus.Draft };
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
                Text = "Invoice Details",
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

            cmbPatient = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 260, Font = Theme.Body, DisplayMember = "FullName", ValueMember = "Id" };
            try { cmbPatient.DataSource = _patientRepo.GetAll().ToList(); } catch (Exception) { /* Ignored */ }

            dtpIssuedAt = new DateTimePicker { Width = 150, Format = DateTimePickerFormat.Short, Font = Theme.Body };
            cmbStatus = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 150, Font = Theme.Body };
            cmbStatus.DataSource = Enum.GetValues(typeof(InvoiceStatus));

            txtNotes = new TextBox { Width = 260, Multiline = true, Height = 60, Font = Theme.Body, ScrollBars = ScrollBars.Vertical };

            int y = 20;
            AddRow(body, "Patient", cmbPatient, ref y);
            AddRow(body, "Date", dtpIssuedAt, ref y);
            AddRow(body, "Status", cmbStatus, ref y);
            AddRow(body, "Notes", txtNotes, ref y);

            // Totals panel
            var pnlTotals = new Panel { Location = new Point(450, 20), Size = new Size(300, 150), BorderStyle = BorderStyle.FixedSingle };
            int ty = 10;
            txtSubTotal = AddTotalRow(pnlTotals, "SubTotal:", ref ty);
            txtTax = AddTotalRow(pnlTotals, "Tax:", ref ty);
            txtTotal = AddTotalRow(pnlTotals, "Total:", ref ty);
            txtTotal.Font = new Font(txtTotal.Font, FontStyle.Bold);
            
            var btnCalc = new Button { Text = "Recalculate", Location = new Point(190, ty), Size = new Size(100, 30) };
            btnCalc.Click += (s, e) => CalculateTotals();
            pnlTotals.Controls.Add(btnCalc);
            
            body.Controls.Add(pnlTotals);

            // Grid
            var lblGrid = new Label { Text = "Line Items", AutoSize = true, Font = Theme.Subheading, ForeColor = Theme.TextPrimary, Location = new Point(20, y + 10) };
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
            
            gridItems.Columns.Add(new DataGridViewTextBoxColumn { Name = "Description", HeaderText = "Description", DataPropertyName = "Description" });
            gridItems.Columns.Add(new DataGridViewTextBoxColumn { Name = "Quantity", HeaderText = "Qty", DataPropertyName = "Quantity" });
            gridItems.Columns.Add(new DataGridViewTextBoxColumn { Name = "UnitPrice", HeaderText = "Unit Price", DataPropertyName = "UnitPrice" });
            gridItems.Columns.Add(new DataGridViewTextBoxColumn { Name = "LineTotal", HeaderText = "Line Total", DataPropertyName = "LineTotal" });
            
            gridItems.CellValueChanged += (s, e) => CalculateTotals();

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

        private TextBox AddTotalRow(Panel pnl, string label, ref int y)
        {
            var lbl = new Label { Text = label, AutoSize = true, Location = new Point(10, y + 4) };
            var txt = new TextBox { Location = new Point(140, y), Width = 150, ReadOnly = true, TextAlign = HorizontalAlignment.Right };
            pnl.Controls.Add(lbl);
            pnl.Controls.Add(txt);
            y += 35;
            return txt;
        }

        private void CalculateTotals()
        {
            double subTotal = 0;
            foreach (DataGridViewRow row in gridItems.Rows)
            {
                if (row.IsNewRow) continue;
                
                int qty = 0;
                double price = 0;
                
                if (row.Cells["Quantity"].Value != null) int.TryParse(row.Cells["Quantity"].Value.ToString(), out qty);
                if (row.Cells["UnitPrice"].Value != null) double.TryParse(row.Cells["UnitPrice"].Value.ToString(), out price);
                
                double line = qty * price;
                row.Cells["LineTotal"].Value = line;
                subTotal += line;
            }

            double tax = subTotal * 0.18; // Example 18% tax
            double total = subTotal + tax;

            txtSubTotal.Text = subTotal.ToString("C");
            txtTax.Text = tax.ToString("C");
            txtTotal.Text = total.ToString("C");
        }

        private void Populate()
        {
            if (_invoice.PatientId > 0) cmbPatient.SelectedValue = _invoice.PatientId;
            dtpIssuedAt.Value = _invoice.IssuedAt.Date;
            cmbStatus.SelectedItem = _invoice.Status;
            txtNotes.Text = _invoice.Notes;

            var bindingList = new System.ComponentModel.BindingList<InvoiceItem>(_invoice.Items);
            gridItems.DataSource = bindingList;

            CalculateTotals();
        }

        private void Save_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbPatient.SelectedValue == null) throw new ValidationException("Please select a patient.");

                _invoice.PatientId = (int)cmbPatient.SelectedValue;
                _invoice.IssuedAt = dtpIssuedAt.Value.Date;
                _invoice.Status = (InvoiceStatus)cmbStatus.SelectedItem;
                _invoice.Notes = txtNotes.Text.Trim();

                CalculateTotals();
                
                double subTotal = 0;
                foreach (var item in _invoice.Items) subTotal += item.LineTotal;
                _invoice.SubTotal = subTotal;
                _invoice.Tax = subTotal * 0.18;
                _invoice.Total = _invoice.SubTotal + _invoice.Tax;

                if (_invoice.Id == 0) _repo.Add(_invoice);
                else _repo.Update(_invoice);

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
