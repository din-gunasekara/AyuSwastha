using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using AyuSwastha.Core;
using AyuSwastha.Models;
using AyuSwastha.Services;

namespace AyuSwastha.Forms
{
    public class PrakritiQuestionnaireForm : Form
    {
        private readonly DoshaRecommendationService _doshaService = new DoshaRecommendationService();
        public DoshaType ResultDosha { get; private set; } = DoshaType.Unknown;

        private Label lblResult;

        // Simple radio buttons for V, P, K across 3 questions
        private RadioButton[] q1 = new RadioButton[3];
        private RadioButton[] q2 = new RadioButton[3];
        private RadioButton[] q3 = new RadioButton[3];

        public PrakritiQuestionnaireForm()
        {
            BuildUi();
        }

        private void BuildUi()
        {
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(500, 480);
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
                Text = "Assess Prakriti (Dosha)",
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

            lblResult = new Label { AutoSize = true, Font = Theme.Subheading, ForeColor = Theme.Primary, Location = new Point(20, 16), Text = "Result: -" };

            var btnSave = new Button
            {
                Text = "Apply",
                BackColor = Theme.Primary,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 9.5f),
                Size = new Size(84, 32),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(ClientSize.Width - 104, 12)
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += (s, e) => { DialogResult = DialogResult.OK; Close(); };

            var sep = new Panel { Dock = DockStyle.Bottom, Height = 1, BackColor = Theme.Border };

            bar.Controls.Add(lblResult);
            bar.Controls.Add(btnSave);
            bar.Controls.Add(sep);
            return bar;
        }

        private Panel BuildBody()
        {
            var body = new Panel { Dock = DockStyle.Fill, BackColor = Theme.Surface, Padding = new Padding(20), AutoScroll = true };

            int y = 20;
            AddQuestion(body, "1. Body Frame & Weight", new[] { "Thin, hard to gain weight (V)", "Medium, muscular (P)", "Broad, gains weight easily (K)" }, q1, ref y);
            AddQuestion(body, "2. Skin Type", new[] { "Dry, rough, cool (V)", "Warm, reddish, prone to acne (P)", "Thick, oily, cool (K)" }, q2, ref y);
            AddQuestion(body, "3. Sleep Pattern", new[] { "Light, interrupted (V)", "Sound, moderate (P)", "Heavy, prolonged (K)" }, q3, ref y);

            return body;
        }

        private void AddQuestion(Panel page, string qText, string[] options, RadioButton[] radios, ref int y)
        {
            var lbl = new Label { Text = qText, AutoSize = true, Font = Theme.Subheading, ForeColor = Theme.TextPrimary, Location = new Point(20, y) };
            page.Controls.Add(lbl);
            y += 30;

            for (int i = 0; i < 3; i++)
            {
                radios[i] = new RadioButton { Text = options[i], AutoSize = true, Font = Theme.Body, ForeColor = Theme.TextMuted, Location = new Point(40, y) };
                radios[i].CheckedChanged += (s, e) => CalculateResult();
                page.Controls.Add(radios[i]);
                y += 24;
            }
            y += 10;
        }

        private void CalculateResult()
        {
            int v = 0, p = 0, k = 0;
            if (q1[0].Checked) v++; if (q1[1].Checked) p++; if (q1[2].Checked) k++;
            if (q2[0].Checked) v++; if (q2[1].Checked) p++; if (q2[2].Checked) k++;
            if (q3[0].Checked) v++; if (q3[1].Checked) p++; if (q3[2].Checked) k++;

            ResultDosha = _doshaService.Determine(v, p, k);
            lblResult.Text = "Result: " + ResultDosha.ToString();
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
