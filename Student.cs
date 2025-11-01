// Student.cs - Updated without Term/Year columns
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net.Http;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EduSync
{
    public partial class Student : Form
    {
        string connectionString = "Data Source=DESKTOP-TUS7DBQ;Initial Catalog=EduSync;Integrated Security=True;Encrypt=False";

        public Student()
        {
            InitializeComponent();
            ButtonStyler.ApplyPrimaryStyle(AddButton);
            ButtonStyler.ApplyPrimaryStyle(UpdateButton);
            ButtonStyler.ApplyPrimaryStyle(Loadbtn);
            ButtonStyler.ApplyDangerStyle(DeleteButton);
            ButtonStyler.ApplyPrimaryStyle(Resetbutton);
            ButtonStyler.ApplyPrimaryStyle(btnsms);
            ButtonStyler.ApplyPrimaryStyle(btnsearch);

            btnsearch.Click += btnsearch_Click;
        }

        private void Student_Load(object sender, EventArgs e)
        {
            LoadStudents();
            StyleDataGridView();
            LoadClassComboBox();
            RoundAllButtons();
            LoadStudentData();
            ClearFields();
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection("Data Source=DESKTOP-TUS7DBQ;Initial Catalog=EduSync;Integrated Security=True;Encrypt=False");
        }

        private void LoadStudents()
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                // ✅ Clean query without Term/Year columns
                string query = log.LoggedInRole == "Teacher"
                    ? "SELECT StudentID, FirstName, LastName, Admission, Class, ParentContact FROM StdTable WHERE Class IN (SELECT Class FROM TeacherTable WHERE Username = @username)"
                    : "SELECT StudentID, FirstName, LastName, Admission, Class, ParentContact FROM StdTable";

                SqlCommand cmd = new SqlCommand(query, conn);
                if (log.LoggedInRole == "Teacher")
                    cmd.Parameters.AddWithValue("@username", log.LoggedInUsername);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                Studentdgv.DataSource = dt;
            }
        }

        private void StyleDataGridView()
        {
            Studentdgv.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            Studentdgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            Studentdgv.DefaultCellStyle.SelectionBackColor = Color.LightBlue;

            Studentdgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(45, 156, 219);
            Studentdgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            Studentdgv.EnableHeadersVisualStyles = false;
            Studentdgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            Studentdgv.RowTemplate.Height = 28;
            Studentdgv.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
            Studentdgv.GridColor = Color.LightGray;

            // ✅ Hide Term/Year columns if they accidentally appear
            if (Studentdgv.Columns.Contains("Term"))
                Studentdgv.Columns["Term"].Visible = false;
            if (Studentdgv.Columns.Contains("Year"))
                Studentdgv.Columns["Year"].Visible = false;
        }

        private void LoadClassComboBox()
        {
            ClassComboBox.Items.Clear();
            ClassComboBox.Items.AddRange(new string[]
            {
                "PLAYGROUP", "PP1", "PP2",
                "GRADE 1", "GRADE 2", "GRADE 3",
                "GRADE 4", "GRADE 5", "GRADE 6",
                "GRADE 7", "GRADE 8", "GRADE 9"
            });
            ClassComboBox.SelectedIndex = -1;
        }

        private void RoundAllButtons()
        {
            foreach (Control ctrl in this.Controls)
                if (ctrl is Button btn) MakeButtonRound(btn);
        }

        private void MakeButtonRound(Button btn)
        {
            // Button rounding implementation
        }

        private void AddButton_Click_1(object sender, EventArgs e)
        {
            // ✅ Validate inputs
            if (string.IsNullOrWhiteSpace(txtStudentID.Text) ||
                string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                string.IsNullOrWhiteSpace(txtLastName.Text) ||
                string.IsNullOrWhiteSpace(txtAdmission.Text) ||
                string.IsNullOrWhiteSpace(txtParentContact.Text) ||
                ClassComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("Please fill in all student details before adding.", "Missing Info", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();

                    // ✅ Check if StudentID already exists
                    SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM StdTable WHERE StudentID = @StudentID", conn);
                    checkCmd.Parameters.AddWithValue("@StudentID", int.Parse(txtStudentID.Text));
                    int count = (int)checkCmd.ExecuteScalar();

                    if (count > 0)
                    {
                        MessageBox.Show("A student with this ID already exists.", "Duplicate", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // ✅ Insert without Term/Year columns
                    SqlCommand cmd = new SqlCommand(@"
                INSERT INTO StdTable 
                (StudentID, FirstName, LastName, Admission, ParentContact, Class) 
                VALUES 
                (@StudentID, @FirstName, @LastName, @Admission, @ParentContact, @Class)", conn);

                    cmd.Parameters.AddWithValue("@StudentID", int.Parse(txtStudentID.Text));
                    cmd.Parameters.AddWithValue("@FirstName", txtFirstName.Text.Trim());
                    cmd.Parameters.AddWithValue("@LastName", txtLastName.Text.Trim());
                    cmd.Parameters.AddWithValue("@Admission", txtAdmission.Text.Trim());
                    cmd.Parameters.AddWithValue("@ParentContact", txtParentContact.Text.Trim());
                    cmd.Parameters.AddWithValue("@Class", ClassComboBox.SelectedItem.ToString());

                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        MessageBox.Show("✅ Student added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadStudents(); // Refresh the grid
                        Resetbutton.PerformClick(); // Clear inputs
                    }
                    else
                    {
                        MessageBox.Show("❌ Failed to insert student.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error occurred:\n" + ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                // ✅ Update without Term/Year columns
                SqlCommand cmd = new SqlCommand(@"
                UPDATE StdTable SET 
                FirstName = @FirstName, 
                LastName = @LastName, 
                Admission = @Admission, 
                ParentContact = @ParentContact, 
                Class = @Class 
                WHERE StudentID = @StudentID", conn);

                cmd.Parameters.AddWithValue("@StudentID", int.Parse(txtStudentID.Text));
                cmd.Parameters.AddWithValue("@FirstName", txtFirstName.Text.Trim());
                cmd.Parameters.AddWithValue("@LastName", txtLastName.Text.Trim());
                cmd.Parameters.AddWithValue("@Admission", txtAdmission.Text.Trim());
                cmd.Parameters.AddWithValue("@ParentContact", txtParentContact.Text.Trim());
                cmd.Parameters.AddWithValue("@Class", ClassComboBox.SelectedItem);

                int rows = cmd.ExecuteNonQuery();
                MessageBox.Show(rows > 0 ? "✅ Student updated successfully!" : "❌ Update failed.");
                if (rows > 0) LoadStudents(); // Refresh grid
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete this student?", "Confirm Delete",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("DELETE FROM StdTable WHERE StudentID = @StudentID", conn);
                    cmd.Parameters.AddWithValue("@StudentID", int.Parse(txtStudentID.Text));

                    int rows = cmd.ExecuteNonQuery();
                    MessageBox.Show(rows > 0 ? "✅ Student deleted successfully!" : "❌ Delete failed.");
                    if (rows > 0)
                    {
                        LoadStudents(); // Refresh grid
                        Resetbutton.PerformClick(); // Clear inputs
                    }
                }
            }
        }

        private void Loadbtn_Click(object sender, EventArgs e)
        {
            LoadStudents();
        }

        private void Resetbutton_Click(object sender, EventArgs e)
        {
            txtStudentID.Clear();
            txtFirstName.Clear();
            txtLastName.Clear();
            txtAdmission.Clear();
            txtParentContact.Clear();
            ClassComboBox.SelectedIndex = -1;
        }

        private void btnsearch_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(@"
                SELECT StudentID, FirstName, LastName, Admission, Class, ParentContact 
                FROM StdTable 
                WHERE FirstName LIKE @search OR LastName LIKE @search OR StudentID LIKE @search", conn);
                cmd.Parameters.AddWithValue("@search", "%" + txtsearch.Text + "%");
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                Studentdgv.DataSource = dt;
            }
        }

        private void Studentdgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = Studentdgv.Rows[e.RowIndex];

                txtStudentID.Text = row.Cells["StudentID"].Value?.ToString();
                txtFirstName.Text = row.Cells["FirstName"].Value?.ToString();
                txtLastName.Text = row.Cells["LastName"].Value?.ToString();
                txtAdmission.Text = row.Cells["Admission"].Value?.ToString();
                ClassComboBox.Text = row.Cells["Class"].Value?.ToString();
                txtParentContact.Text = row.Cells["ParentContact"].Value?.ToString();
            }
        }

        private async void btnsms_Click(object sender, EventArgs e)
        {
            try
            {
                string phone = txtphone.Text.Trim();
                if (string.IsNullOrEmpty(phone))
                {
                    MessageBox.Show("Please enter a phone number.");
                    return;
                }

                if (!phone.StartsWith("254") || phone.Length != 12)
                {
                    MessageBox.Show("Phone number must be in format 2547XXXXXXXX");
                    return;
                }

                var apiKey = "402fa7ae";
                var apiSecret = "uh0JjdMz411uYpMw";
                var from = "EduSync";
                var text = "Please clear fee balance for your child.";

                var url = $"https://rest.nexmo.com/sms/json?" +
                          $"api_key={apiKey}&api_secret={apiSecret}&to={phone}&from={from}&text={Uri.EscapeDataString(text)}";

                using (HttpClient client = new HttpClient())
                {
                    btnsms.Enabled = false;
                    btnsms.Text = "Sending...";

                    HttpResponseMessage response = await client.GetAsync(url);
                    string result = await response.Content.ReadAsStringAsync();

                    MessageBox.Show("SMS sent successfully!\n" + result);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                btnsms.Enabled = true;
                btnsms.Text = "Send SMS";
            }
        }

        private void Backbutton_Click(object sender, EventArgs e)
        {
            Dashboard dash = new Dashboard();
            dash.Show();
            this.Hide();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void DeleteButton_Click_1(object sender, EventArgs e)
        {
        
            if (MessageBox.Show("Are you sure you want to delete this student?", "Confirm Delete",
                 MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection conn = GetConnection())
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand("DELETE FROM StdTable WHERE StudentID = @StudentID", conn);
                        cmd.Parameters.AddWithValue("@StudentID", int.Parse(txtStudentID.Text));

                        int rows = cmd.ExecuteNonQuery();
                        MessageBox.Show(rows > 0 ? "✅ Student deleted successfully!" : "❌ Delete failed.");

                        if (rows > 0)
                        {
                            LoadStudents();           // Refresh grid
                            Resetbutton.PerformClick(); // Clear inputs
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting student: {ex.Message}", "Database Error",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        

        private void UpdateButton_Click_1(object sender, EventArgs e)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                // ✅ Update without Term/Year columns
                SqlCommand cmd = new SqlCommand(@"
                UPDATE StdTable SET 
                FirstName = @FirstName, 
                LastName = @LastName, 
                Admission = @Admission, 
                ParentContact = @ParentContact, 
                Class = @Class 
                WHERE StudentID = @StudentID", conn);

                cmd.Parameters.AddWithValue("@StudentID", int.Parse(txtStudentID.Text));
                cmd.Parameters.AddWithValue("@FirstName", txtFirstName.Text.Trim());
                cmd.Parameters.AddWithValue("@LastName", txtLastName.Text.Trim());
                cmd.Parameters.AddWithValue("@Admission", txtAdmission.Text.Trim());
                cmd.Parameters.AddWithValue("@ParentContact", txtParentContact.Text.Trim());
                cmd.Parameters.AddWithValue("@Class", ClassComboBox.SelectedItem);

                int rows = cmd.ExecuteNonQuery();
                MessageBox.Show(rows > 0 ? "✅ Student updated successfully!" : "❌ Update failed.");
                if (rows > 0) LoadStudents(); // Refresh grid
            }
        }
private void ClearFields()
        {
            txtStudentID.Clear();
            txtFirstName.Clear();
            txtLastName.Clear();
            txtAdmission.Clear();
            txtParentContact.Clear();
            ClassComboBox.SelectedIndex = -1;

            // Clear any selections in DataGridView
            Studentdgv.ClearSelection();
        }

        // Helper method to load/refresh student data in DataGridView
        private void LoadStudentData()
        {

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string selectQuery = "SELECT StudentID, FirstName, LastName, Class, Admission, ParentContact FROM StdTable ORDER BY StudentID";

                    using (SqlDataAdapter adapter = new SqlDataAdapter(selectQuery, connection))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        Studentdgv.DataSource = dataTable;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading student data: {ex.Message}", "Database Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Optional: Add this to handle row selection in DataGridView for easy editing
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = Studentdgv.Rows[e.RowIndex];

                // Populate fields with selected student data
                txtStudentID.Text = row.Cells["StudentID"].Value?.ToString() ?? "";
                txtFirstName.Text = row.Cells["FirstName"].Value?.ToString() ?? "";
                txtLastName.Text = row.Cells["LastName"].Value?.ToString() ?? "";
                ClassComboBox.Text = row.Cells["Class"].Value?.ToString() ?? "";
                txtAdmission.Text = row.Cells["Admission"].Value?.ToString() ?? "";
                txtParentContact.Text = row.Cells["ParentContact"].Value?.ToString() ?? "";
            }
        }

    }
    
}