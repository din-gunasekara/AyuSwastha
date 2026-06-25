using System.Drawing;
using System.Windows.Forms;
using AyuSwastha.Core;

namespace AyuSwastha.Forms
{
    partial class PlaceholderControl
    {
        private System.ComponentModel.IContainer components = null;

        private Panel card;
        private Label lblTitle;
        private Label lblMessage;
        private Label lblBadge;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.card = new Panel();
            this.lblBadge = new Label();
            this.lblTitle = new Label();
            this.lblMessage = new Label();
            this.SuspendLayout();
            //
            // card
            //
            this.card.BackColor = Theme.Surface;
            this.card.BorderStyle = BorderStyle.FixedSingle;
            this.card.Location = new Point(24, 24);
            this.card.Size = new Size(560, 200);
            this.card.Controls.Add(this.lblBadge);
            this.card.Controls.Add(this.lblTitle);
            this.card.Controls.Add(this.lblMessage);
            //
            // lblBadge
            //
            this.lblBadge.AutoSize = true;
            this.lblBadge.BackColor = Theme.Accent;
            this.lblBadge.ForeColor = Color.White;
            this.lblBadge.Font = new Font("Segoe UI Semibold", 8f);
            this.lblBadge.Padding = new Padding(8, 4, 8, 4);
            this.lblBadge.Location = new Point(24, 24);
            this.lblBadge.Text = "PLANNED MODULE";
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = Theme.Heading;
            this.lblTitle.ForeColor = Theme.TextPrimary;
            this.lblTitle.Location = new Point(22, 60);
            this.lblTitle.Text = "Module";
            //
            // lblMessage
            //
            this.lblMessage.AutoSize = false;
            this.lblMessage.Font = Theme.Body;
            this.lblMessage.ForeColor = Theme.TextMuted;
            this.lblMessage.Location = new Point(24, 100);
            this.lblMessage.Size = new Size(510, 70);
            this.lblMessage.Text = "";
            //
            // PlaceholderControl
            //
            this.BackColor = Theme.Background;
            this.Controls.Add(this.card);
            this.ResumeLayout(false);
        }
    }
}
