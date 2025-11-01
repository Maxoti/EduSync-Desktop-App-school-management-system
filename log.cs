using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace EduSync
{
    public partial class log : Form
    {
        public log()
        {
            InitializeComponent();
            ButtonStyler.ApplyPrimaryStyle(btnLogin);
            ButtonStyler.ApplyPrimaryStyle(ResetButton);
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            txtUsername.Clear();
            txtPassword.Clear();
        }

        private void label4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        public static string LoggedInUsername;
        public static string LoggedInRole;

        private void SetPlaceholder(System.Windows.Forms.TextBox txt, string placeholder, bool isPassword = false)
        {
            txt.Text = placeholder;
            txt.ForeColor = Color.Gray;

            txt.GotFocus += (s, e) =>
            {
                if (txt.Text == placeholder)
                {
                    txt.Text = "";
                    txt.ForeColor = Color.Black;
                    if (isPassword) txt.PasswordChar = '*'; // show stars only when typing
                }
            };

            txt.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txt.Text))
                {
                    txt.Text = placeholder;
                    txt.ForeColor = Color.Gray;
                    if (isPassword) txt.PasswordChar = '\0'; // remove stars when placeholder visible
                }
            };
        }














        private void ResetButton_Click(object sender, EventArgs e)
        {
            txtUsername.Clear();
            txtPassword.Clear();
        }

        private void btnGenerateHash_Click(object sender, EventArgs e)
        {

            string hashedAdmin = PasswordHasher.HashPassword("Admin");
            MessageBox.Show(hashedAdmin, "Hashed Password for Admin");
        }

        private void log_Load(object sender, EventArgs e)
        {
            SetPlaceholder(txtUsername, "Username");
            SetPlaceholder(txtPassword, "Password", true);
        }

      






        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnGenHash_Click(object sender, EventArgs e)
        {

            string teacherHash = PasswordHasher.HashPassword("teacher123");
            string accountantHash = PasswordHasher.HashPassword("accountant123");

            MessageBox.Show($"Teacher Hash:\n{teacherHash}\n\nAccountant Hash:\n{accountantHash}", "Generated Hashes");
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
        
            string connectionString = "Data Source=DESKTOP-TUS7DBQ;Initial Catalog=EduSync;Integrated Security=True;Encrypt=False";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "SELECT Password, Role FROM UserTbl WHERE Username = @username";
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.Add("@username", SqlDbType.VarChar).Value = txtUsername.Text.Trim();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string storedHashedPassword = reader["Password"].ToString();
                                string enteredPassword = txtPassword.Text.Trim();

                                if (!PasswordHasher.VerifyPassword(enteredPassword, storedHashedPassword))
                                {
                                    MessageBox.Show("Incorrect password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }

                                string userRole = reader["Role"].ToString();

                                // ✅ Check license
                               

                                // ✅ All checks passed
                                LoggedInUsername = txtUsername.Text.Trim();
                                LoggedInRole = userRole;
                                this.DialogResult = DialogResult.OK;
                                this.Close();
                            }
                            else
                            {
                                MessageBox.Show("User not found.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        



        private bool IsLicenseValid()
        {
            // Add your real license logic here
            return true; // Placeholder
        }

        private void linkAdmin_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            AdminSignupcs signup = new AdminSignupcs();
            signup.ShowDialog();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private bool isUsernamePlaceholderActive = true;
        private const string USERNAME_PLACEHOLDER = "Enter your username";
        private Color originalUsernameBackColor;
        private BorderStyle originalUsernameBorderStyle;

        private void txtUsername_Enter(object sender, EventArgs e)
        {
            // Clear placeholder text when user enters the textbox
            if (isUsernamePlaceholderActive)
            {
                txtUsername.Text = "";
                txtUsername.ForeColor = SystemColors.WindowText; // Change to normal text color
                txtUsername.Font = new Font(txtUsername.Font, FontStyle.Regular); // Remove italic style
                isUsernamePlaceholderActive = false;
            }

            // Visual feedback that the field is focused
            txtUsername.BackColor = Color.FromArgb(240, 248, 255); // Light blue background
            txtUsername.BorderStyle = BorderStyle.Fixed3D;

        }
       

        private void txtUsername_Leave(object sender, EventArgs e)
        {
            // Restore placeholder text if field is empty
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                txtUsername.Text = USERNAME_PLACEHOLDER;
                txtUsername.ForeColor = SystemColors.GrayText; // Gray color for placeholder
                txtUsername.Font = new Font(txtUsername.Font, FontStyle.Italic); // Italic style for placeholder
                isUsernamePlaceholderActive = true;
            }
            else
            {
                isUsernamePlaceholderActive = false;
            }

            // Restore normal appearance
            txtUsername.BackColor = originalUsernameBackColor;
            txtUsername.BorderStyle = originalUsernameBorderStyle;
        }

        private void txtPassword_Enter(object sender, EventArgs e)
        {
            // Clear placeholder text when user enters the textbox
            if (isUsernamePlaceholderActive)
            {
                txtUsername.Text = "";
                txtUsername.ForeColor = SystemColors.WindowText; // Change to normal text color
                txtUsername.Font = new Font(txtUsername.Font, FontStyle.Regular); // Remove italic style
                isUsernamePlaceholderActive = false;
            }

            // Visual feedback that the field is focused
            txtUsername.BackColor = Color.FromArgb(240, 248, 255); // Light blue background
            txtUsername.BorderStyle = BorderStyle.Fixed3D;


        }

        private void txtPassword_Leave(object sender, EventArgs e)
        {
            // Restore placeholder text if field is empty
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                txtUsername.Text = USERNAME_PLACEHOLDER;
                txtUsername.ForeColor = SystemColors.GrayText; // Gray color for placeholder
                txtUsername.Font = new Font(txtUsername.Font, FontStyle.Italic); // Italic style for placeholder
                isUsernamePlaceholderActive = true;
            }
            else
            {
                isUsernamePlaceholderActive = false;
            }

            // Restore normal appearance
            txtUsername.BackColor = originalUsernameBackColor;
            txtUsername.BorderStyle = originalUsernameBorderStyle;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBox1.Checked)
            {
               txtPassword.UseSystemPasswordChar = false;

            }
            else
            {
                txtPassword.UseSystemPasswordChar = true;
            }
        }
    }

}



