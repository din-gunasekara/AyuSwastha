using System.Drawing;

namespace AyuSwastha.Core
{
    /// <summary>Central palette and fonts so every form shares one look.</summary>
    public static class Theme
    {
        public static readonly Color Primary = ColorTranslator.FromHtml("#1B5E3A");   // Ayurveda green
        public static readonly Color PrimaryLight = ColorTranslator.FromHtml("#4E9F70"); // Light green
        public static readonly Color PrimaryDark = ColorTranslator.FromHtml("#134429");
        public static readonly Color Accent = ColorTranslator.FromHtml("#C8A24B");    // herbal gold
        public static readonly Color Surface = ColorTranslator.FromHtml("#FFFFFF");
        public static readonly Color Background = ColorTranslator.FromHtml("#F5FAF6"); // Very light green background
        public static readonly Color SidebarBg = ColorTranslator.FromHtml("#E8F3EB"); // Light green
        public static readonly Color TextPrimary = ColorTranslator.FromHtml("#1F2A24");
        public static readonly Color TextMuted = ColorTranslator.FromHtml("#6B7A70");
        public static readonly Color Border = ColorTranslator.FromHtml("#D8E0DA");
        public static readonly Color Danger = ColorTranslator.FromHtml("#B23A48");

        public static readonly Font Heading = new Font("Segoe UI Semibold", 15f, FontStyle.Regular);
        public static readonly Font Subheading = new Font("Segoe UI Semibold", 11f, FontStyle.Regular);
        public static readonly Font Body = new Font("Segoe UI", 9.75f, FontStyle.Regular);
    }
}
