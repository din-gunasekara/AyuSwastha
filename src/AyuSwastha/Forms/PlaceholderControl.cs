using System.Windows.Forms;

namespace AyuSwastha.Forms
{
    /// <summary>Simple stub view for modules that are on the roadmap (see TASK.md).</summary>
    public partial class PlaceholderControl : UserControl
    {
        public PlaceholderControl() : this("Module", "")
        {
        }

        public PlaceholderControl(string title, string message)
        {
            InitializeComponent();
            lblTitle.Text = title;
            lblMessage.Text = message;
        }
    }
}
