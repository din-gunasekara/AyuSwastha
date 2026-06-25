using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using AyuSwastha.Core;
using AyuSwastha.Data;
using AyuSwastha.Models;

namespace AyuSwastha.Forms
{
    public partial class DashboardControl : UserControl
    {
        private readonly PatientRepository _patients = new PatientRepository();
        private readonly DoctorRepository _doctors = new DoctorRepository();
        private readonly AppointmentRepository _appointments = new AppointmentRepository();
        private readonly InvoiceRepository _invoices = new InvoiceRepository();

        public DashboardControl()
        {
            InitializeComponent();
        }

        private void DashboardControl_Load(object sender, EventArgs e)
        {
            lblSub.Text = "Clinic overview — " + DateTime.Now.ToString("dddd, dd MMMM yyyy");

            try
            {
                int patientCount = _patients.GetAll().Count;
                int doctorCount = _doctors.GetAll().Count;
                int todayCount = _appointments.GetForDate(DateTime.Today).Count;
                double revenue = _invoices.GetAll().Where(i => i.Status == InvoiceStatus.Paid).Sum(i => i.Total);

                flowTiles.Controls.Add(BuildTile("Patients", patientCount.ToString(), Theme.Primary));
                flowTiles.Controls.Add(BuildTile("Doctors", doctorCount.ToString(), Theme.Accent));
                flowTiles.Controls.Add(BuildTile("Today's Appointments", todayCount.ToString(), Theme.PrimaryDark));
                flowTiles.Controls.Add(BuildTile("Total Revenue", revenue.ToString("C2"), Color.ForestGreen));
            }
            catch (AppException ex)
            {
                MessageBox.Show(ex.Message, "AyuSwastha", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private static Panel BuildTile(string caption, string value, Color accent)
        {
            var tile = new Panel
            {
                Size = new Size(230, 120),
                Margin = new Padding(8),
                BackColor = Theme.Surface,
                BorderStyle = BorderStyle.FixedSingle
            };

            var bar = new Panel { Dock = DockStyle.Left, Width = 6, BackColor = accent };

            float fontSize = value.Length > 8 ? 22f : (value.Length > 5 ? 28f : 34f);

            var lblValue = new Label
            {
                AutoSize = true,
                Text = value,
                Font = new Font("Segoe UI Semibold", fontSize, FontStyle.Bold),
                ForeColor = Theme.TextPrimary,
                Location = new Point(24, 18)
            };

            var lblCaption = new Label
            {
                AutoSize = true,
                Text = caption,
                Font = Theme.Body,
                ForeColor = Theme.TextMuted,
                Location = new Point(26, 82)
            };

            tile.Controls.Add(lblValue);
            tile.Controls.Add(lblCaption);
            tile.Controls.Add(bar);
            return tile;
        }
    }
}
