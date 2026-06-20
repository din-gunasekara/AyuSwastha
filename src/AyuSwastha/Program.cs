using System;
using System.Windows.Forms;
using AyuSwastha.Core;
using AyuSwastha.Data;
using AyuSwastha.Forms;

namespace AyuSwastha
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Catch anything that escapes so the app shows a friendly message
            // instead of crashing to the WinForms unhandled-exception dialog.
            Application.ThreadException += (s, e) => ShowFatal(e.Exception);
            AppDomain.CurrentDomain.UnhandledException += (s, e) => ShowFatal(e.ExceptionObject as Exception);

            try
            {
                Database.Initialize();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not initialize the database:\n\n" + ex.Message,
                    "AyuSwastha", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (var login = new LoginForm())
            {
                if (login.ShowDialog() != DialogResult.OK)
                    return; // user cancelled the login

                Application.Run(new MainForm());
            }
        }

        private static void ShowFatal(Exception ex)
        {
            string message = ex is AppException ? ex.Message
                : "An unexpected error occurred:\n\n" + (ex?.Message ?? "Unknown error");
            MessageBox.Show(message, "AyuSwastha", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
