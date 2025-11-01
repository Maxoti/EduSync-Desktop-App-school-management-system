using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
namespace EduSync
{
    internal static class Program
    {
        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();
        /// <summary>
        /// The main entry point for the EduSync application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Enable per-monitor DPI awareness (modern method)
            if(Environment.OSVersion.Version.Major >= 6)
            {
                SetProcessDPIAware();
            }

            // Enable visual styles for modern UI
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                // 1. Check license validity
                if (!LicenseManager.IsLicenseValid() && !LicenseForm.ShowLicenseDialog())
                {
                    MessageBox.Show("Application requires a valid license to run.",
                        "License Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // Stop if license is invalid
                }

                // 2. Show welcome message
                MessageBox.Show("Welcome to EduSync!.",
                    "Welcome", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // 3. Show splash screen (progress bar form)
                using (var splash = new Progressbarform())
                {
                    splash.ShowDialog(); // Wait for splash to finish
                }

                // 4. Show login form
                using (var loginForm = new log())
                {
                    if (loginForm.ShowDialog() == DialogResult.OK)
                    {
                        // 5. If login was successful, run the main dashboard
                        Application.Run(new Dashboard());
                    }
                    else
                    {
                        // Login failed or cancelled — exit
                        MessageBox.Show("Login was cancelled. Application will now exit.",
                            "Login Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                // 6. General startup error handling
                MessageBox.Show($"An error occurred starting the application:\n\n{ex.Message}",
                    "Startup Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
