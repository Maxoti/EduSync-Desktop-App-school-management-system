using System;
using System.Windows.Forms;

namespace EduSync
{
    public partial class LicenseForm : Form
    {
        public LicenseForm()
        {
            InitializeComponent();
            InitializeLicenseUI();
        }

        // This will be called during startup
        public static bool ShowLicenseDialog(IWin32Window owner = null)
        {
            using (var form = new LicenseForm())
            {
                var result = form.ShowDialog(owner);
                // For now, treat OK as valid license
                return result == DialogResult.OK;
            }
        }

        // This will be called from the Help menu
        public static void ShowLicenseManager(IWin32Window owner = null)
        {
            using (var form = new LicenseForm())
            {
                form.ShowDialog(owner);
            }
        }

        private void InitializeLicenseUI()
        {
            this.Text = "EduSync License";
            this.Size = new System.Drawing.Size(400, 200);
            this.StartPosition = FormStartPosition.CenterParent;

            var label = new Label
            {
                Text = "Welcome to EduSync!\n\nThis application requires a valid license to proceed.",
                Dock = DockStyle.Fill,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Font = new System.Drawing.Font("Segoe UI", 10)
            };

            var okButton = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Dock = DockStyle.Bottom
            };

            this.Controls.Add(label);
            this.Controls.Add(okButton);

            this.AcceptButton = okButton;
        }

        private void LicenseForm_Load(object sender, EventArgs e)
        {
            // Leave empty or remove if unused
        }
    }
}
