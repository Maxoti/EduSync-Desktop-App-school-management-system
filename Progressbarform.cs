using System;
using System.Windows.Forms;

namespace EduSync
{
    public partial class Progressbarform : Form
    {
        // Track actual progress (0-100)
        private int _currentProgress = 0;

        public Progressbarform()
        {
            InitializeComponent();
        }

        private void Progressbarform_Load(object sender, EventArgs e)
        {
            // Initialize UI
            progressBar1.Value = 0;
            lblPercentage.Text = "0%";

            // Start the progress simulation
            timer1.Interval = 50; // Update every 50ms
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Simulate progress (replace with actual work progress)
            _currentProgress += 2;

            // Update UI
            UpdateProgressUI(_currentProgress);

            // Check if completed
            if (_currentProgress >= 100)
            {
                timer1.Stop();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void UpdateProgressUI(int progress)
        {
            // Ensure progress stays within bounds
            progress = Math.Min(100, Math.Max(0, progress));

            // Update progress bar
            progressBar1.Value = progress;

            // Update percentage label
            lblPercentage.Text = $"{progress}%";

            // Force immediate UI update
            progressBar1.Refresh();
            lblPercentage.Refresh();
            Application.DoEvents();
        }

        // Optional: Allow external progress updates
        public void SetProgress(int progress)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<int>(SetProgress), progress);
                return;
            }

            UpdateProgressUI(progress);

            // Close if completed
            if (progress >= 100)
            {
                timer1.Stop();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}