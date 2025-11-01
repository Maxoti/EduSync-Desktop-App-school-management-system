using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace EduSync
{
    public partial class AdminSignupcs : Form
    {
        public AdminSignupcs()
        {
            InitializeComponent();
            InitializeRoleComboBox();
            ButtonStyler.ApplyPrimaryStyle(btnRegister);
            ButtonStyler.ApplyPrimaryStyle(btnBack);
        }

        private void InitializeRoleComboBox()
        {
            // Configure the ComboBox
            CmbRole.DropDownStyle = ComboBoxStyle.DropDownList; // No typing allowed
            CmbRole.Items.Clear();

            // Add roles
            string[] roles = { "Teacher", "Admin", "Accountant" };
            CmbRole.Items.AddRange(roles);

            // Default selection
            CmbRole.SelectedIndex = 0;
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            // Professional registration code with role selection and access control
           
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
           
        }

        private void CmbRole_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Optional: handle role change events
        }

        private void btnRegister_Click_1(object sender, EventArgs e)
        {
            // Professional registration code with role selection and access control
            string username = txtusername.Text.Trim();
            string password = txtpassword.Text;
            string confirmPassword = txtConfirmpassword.Text;
            string selectedRole = CmbRole.SelectedItem?.ToString();

            // Validation
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Passwords do not match.");
                return;
            }

            if (string.IsNullOrEmpty(selectedRole))
            {
                MessageBox.Show("Please select a role.");
                return;
            }

            string hashedPassword = PasswordHasher.HashPassword(password);
            string connectionString = "Data Source=DESKTOP-TUS7DBQ;Initial Catalog=EduSync;Integrated Security=True;Encrypt=False";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Check if username already exists
                    string checkUserQuery = "SELECT COUNT(*) FROM UserTbl WHERE Username = @username";
                    using (SqlCommand checkUserCmd = new SqlCommand(checkUserQuery, conn))
                    {
                        checkUserCmd.Parameters.AddWithValue("@username", username);
                        int userCount = (int)checkUserCmd.ExecuteScalar();

                        if (userCount > 0)
                        {
                            MessageBox.Show("Username already exists. Please choose a different username.");
                            return;
                        }
                    }

                    // Restrict Admin role creation (only one admin allowed)
                    if (selectedRole == "Admin")
                    {
                        string checkAdminQuery = "SELECT COUNT(*) FROM UserTbl WHERE Role = 'Admin'";
                        using (SqlCommand checkAdminCmd = new SqlCommand(checkAdminQuery, conn))
                        {
                            int adminCount = (int)checkAdminCmd.ExecuteScalar();

                            if (adminCount > 0)
                            {
                                MessageBox.Show("An admin account already exists. Please select a different role.");
                                return;
                            }
                        }
                    }

                    // Register user with selected role
                    string insertQuery = "INSERT INTO UserTbl (Username, Password, Role) VALUES (@username, @password, @role)";
                    using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", hashedPassword);
                        cmd.Parameters.AddWithValue("@role", selectedRole);
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show($"{selectedRole} account registered successfully!");
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnBack_Click_1(object sender, EventArgs e)
        {
            log login = new log();
            login.Show();
            this.Close();
        }
    }
}
