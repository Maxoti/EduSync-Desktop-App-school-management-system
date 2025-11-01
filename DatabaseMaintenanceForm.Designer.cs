namespace EduSync
{
    partial class DatabaseMaintenanceForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnBackupDatabase = new System.Windows.Forms.Button();
            this.BtnRestore = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnBackupDatabase
            // 
            this.btnBackupDatabase.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBackupDatabase.Location = new System.Drawing.Point(36, 56);
            this.btnBackupDatabase.Name = "btnBackupDatabase";
            this.btnBackupDatabase.Size = new System.Drawing.Size(215, 57);
            this.btnBackupDatabase.TabIndex = 0;
            this.btnBackupDatabase.Text = "BackupDatabase";
            this.btnBackupDatabase.UseVisualStyleBackColor = true;
            this.btnBackupDatabase.Click += new System.EventHandler(this.btnBackupDatabase_Click);
            // 
            // BtnRestore
            // 
            this.BtnRestore.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnRestore.Location = new System.Drawing.Point(280, 56);
            this.BtnRestore.Name = "BtnRestore";
            this.BtnRestore.Size = new System.Drawing.Size(187, 57);
            this.BtnRestore.TabIndex = 1;
            this.BtnRestore.Text = "Restore Database";
            this.BtnRestore.UseVisualStyleBackColor = true;
            this.BtnRestore.Click += new System.EventHandler(this.BtnRestore_Click);
            // 
            // DatabaseMaintenanceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(519, 203);
            this.Controls.Add(this.BtnRestore);
            this.Controls.Add(this.btnBackupDatabase);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "DatabaseMaintenanceForm";
            this.Text = "DatabaseMaintenanceForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnBackupDatabase;
        private System.Windows.Forms.Button BtnRestore;
    }
}