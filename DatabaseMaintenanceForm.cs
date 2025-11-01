using System;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;

namespace EduSync
{
    public partial class DatabaseMaintenanceForm : Form
    {
        string connectionString = "Data Source=DESKTOP-TUS7DBQ;Initial Catalog=EduSync;Integrated Security=True;Encrypt=False";

        public DatabaseMaintenanceForm()
        {
            InitializeComponent();
        }

        // 🔹 Event for Backup Button
        private void btnBackupDatabase_Click(object sender, EventArgs e)
        {
            try
            {
                // Ensure backup folder exists
                string backupFolder = @"C:\EduSyncBackups";
                if (!Directory.Exists(backupFolder))
                {
                    Directory.CreateDirectory(backupFolder);
                }

                // Build backup path
                string backupPath = $@"{backupFolder}\EduSync_Backup_{DateTime.Now:yyyyMMdd_HHmmss}.bak";

                // Run backup
                BackupDatabase(backupPath);

                MessageBox.Show($"✅ Backup completed successfully!\nSaved at: {backupPath}",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Error during backup: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 🔹 Event for Restore Button
        private void btnRestoreDatabase_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.InitialDirectory = @"C:\EduSyncBackups"; // default restore folder
                    ofd.Filter = "Backup Files (*.bak)|*.bak";

                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        string backupPath = ofd.FileName;
                        RestoreDatabase(backupPath);

                        MessageBox.Show("✅ Database restored successfully!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Error during restore: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 🔹 Actual backup logic
        private void BackupDatabase(string backupPath)
        {
            string dbName = "EduSync"; // must match your database name
            string query = $@"
                BACKUP DATABASE [{dbName}] 
                TO DISK = '{backupPath}' 
                WITH FORMAT, INIT, 
                NAME = 'Full Backup of {dbName}';";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // 🔹 Actual restore logic
        private void RestoreDatabase(string backupPath)
        {
            string dbName = "EduSync"; // must match your database name
            string query = $@"
                ALTER DATABASE [{dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                RESTORE DATABASE [{dbName}] 
                FROM DISK = '{backupPath}' WITH REPLACE;
                ALTER DATABASE [{dbName}] SET MULTI_USER;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void BtnRestore_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.InitialDirectory = @"C:\EduSyncBackups"; // default restore folder
                    ofd.Filter = "Backup Files (*.bak)|*.bak";

                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        string backupPath = ofd.FileName;
                        RestoreDatabase(backupPath);

                        MessageBox.Show("✅ Database restored successfully!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Error during restore: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
    
}
