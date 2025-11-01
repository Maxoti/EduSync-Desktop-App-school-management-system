using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EduSync
{
    public partial class Teacherbtn : Form
    {
        public Teacherbtn()
        {
            InitializeComponent();
            ButtonStyler.ApplyPrimaryStyle(btnsearch);
            ButtonStyler.ApplyPrimaryStyle(AddButton);
            ButtonStyler.ApplyPrimaryStyle(UpdateButton);
            ButtonStyler.ApplyDangerStyle(DeleteButton);
            ButtonStyler.ApplyPrimaryStyle(ResetButton);
            ButtonStyler.ApplyPrimaryStyle(btnTeacher);
            
        }


        private void Teacher_Load(object sender, EventArgs e)
        {
            UIHelper.StyleGrid(Teacherdgv);
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is Button btn)
                {
                    MakeButtonRound(btn);
                }
            }
            StyleDataGridView();

            string connectionString = "Data Source=DESKTOP-TUS7DBQ;Initial Catalog=EduSync;Integrated Security=True;Encrypt=False";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string selectQuery = "SELECT * FROM TeacherTable";
                    SqlDataAdapter adapter = new SqlDataAdapter(selectQuery, conn);
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    Teacherdgv.AutoGenerateColumns = true;

                    Teacherdgv.DataSource = table;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading teacher data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        private void label7_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnTeacher_Click(object sender, EventArgs e)
        {

        }


        private void MakeButtonRound(Button btn)
        {
        }





    private void AddButton_Click(object sender, EventArgs e)
        {
            string connectionString = "Data Source=DESKTOP-TUS7DBQ;Initial Catalog=EduSync;Integrated Security=True;Encrypt=False";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string insertQuery = "INSERT INTO [TeacherTable] (TeacherID, Name, TSCNO, Class,LearningArea) " +
                                         "VALUES (@TeacherID, @Name, @TSCNO, @Class,@LearningArea)";

                    using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                    {
                        if (ClassComboBox.SelectedIndex == -1)
                        {
                            MessageBox.Show("Please select a class.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        string SelectedAreas = string.Join(",", checkedListBox1.CheckedItems.Cast<string>());


                        cmd.Parameters.Add("@TeacherID", SqlDbType.Int).Value = Convert.ToInt32(txtTeacherID.Text);
                        cmd.Parameters.Add("@Name", SqlDbType.VarChar).Value = txtName.Text.Trim();
                        cmd.Parameters.Add("@TSCNO", SqlDbType.VarChar).Value = txtTSCNO.Text.Trim();
                        cmd.Parameters.Add("@Class", SqlDbType.VarChar).Value = ClassComboBox.SelectedItem?.ToString();
                        cmd.Parameters.Add("@LearningArea", SqlDbType.VarChar).Value = SelectedAreas;

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Teacher inserted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Insert failed.", "Failure", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            string connectionString = "Data Source=DESKTOP-TUS7DBQ;Initial Catalog=EduSync;Integrated Security=True;Encrypt=False";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string updateQuery = "UPDATE TeacherTable SET Name = @Name, TSCNO = @TSCNO, Class = @Class WHERE TeacherID = @TeacherID";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                    {
                        if (ClassComboBox.SelectedIndex == -1)
                        {
                            MessageBox.Show("Please select a class.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        cmd.Parameters.Add("@TeacherID", SqlDbType.Int).Value = Convert.ToInt32(txtTeacherID.Text);
                        cmd.Parameters.Add("@Name", SqlDbType.VarChar).Value = txtName.Text.Trim();
                        cmd.Parameters.Add("@TSCNO", SqlDbType.VarChar).Value = txtTSCNO.Text.Trim();
                        cmd.Parameters.Add("@Class", SqlDbType.VarChar).Value = ClassComboBox.SelectedItem?.ToString();

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Teacher record updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Update failed. Teacher not found.", "Failure", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            string connectionString = "Data Source=DESKTOP-TUS7DBQ;Initial Catalog=EduSync;Integrated Security=True;Encrypt=False";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string deleteQuery = "DELETE FROM TeacherTable WHERE TeacherID = @TeacherID";

                    using (SqlCommand cmd = new SqlCommand(deleteQuery, conn))
                    {
                        cmd.Parameters.Add("@TeacherID", SqlDbType.Int).Value = Convert.ToInt32(txtTeacherID.Text);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Teacher deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Delete failed. Teacher not found.", "Failure", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            txtTeacherID.Clear();
            txtName.Clear();
            txtTSCNO.Clear();
            ClassComboBox.SelectedIndex = -1;
            checkedListBox1.Items.Clear();
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            Dashboard dash = new Dashboard();
            dash.Show();
            this.Close();
        }

        private void btnTeacher_Click_1(object sender, EventArgs e)
        {
            string connectionString = "Data Source=DESKTOP-TUS7DBQ;Initial Catalog=EduSync;Integrated Security=True;Encrypt=False";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string selectQuery = @"SELECT TeacherID,Name,TscNo,Class,LearningArea FROM TeacherTable";


                    SqlDataAdapter adapter = new SqlDataAdapter(selectQuery, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    Teacherdgv.AutoGenerateColumns = true;
                    Teacherdgv.ColumnHeadersVisible = true;
                    Teacherdgv.DataSource = dt;

                    Teacherdgv.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading teacher: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void StyleDataGridView()
        {
            Teacherdgv.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            Teacherdgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            Teacherdgv.DefaultCellStyle.SelectionBackColor = Color.LightBlue;

            Teacherdgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(45, 156, 219);
            Teacherdgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            Teacherdgv.EnableHeadersVisualStyles = false;
            Teacherdgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            Teacherdgv.RowTemplate.Height = 28;
            Teacherdgv.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
            Teacherdgv.GridColor = Color.LightGray;
        }


        private void btnsearch_Click(object sender, EventArgs e)
        {
        
            string connectionString = "Data Source=DESKTOP-TUS7DBQ;Initial Catalog=EduSync;Integrated Security=True;Encrypt=False";

            if (string.IsNullOrWhiteSpace(txtsearch.Text))
            {
                MessageBox.Show("Please enter a teacher's name to search.");
                return;
            }

            string query = "SELECT TeacherID, Name, TSCNo, Class FROM TeacherTable WHERE Name LIKE @Name";

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@Name", "%" + txtsearch.Text.Trim() + "%"));

            // Optional filters
            if (!string.IsNullOrWhiteSpace(txtTeacherID.Text))
            {
                query += " AND TeacherID = @TeacherID";
                parameters.Add(new SqlParameter("@TeacherID", txtTeacherID.Text.Trim()));
            }

            if (!string.IsNullOrWhiteSpace(txtTSCNO.Text))
            {
                query += " AND TSCNo = @TSCNo";
                parameters.Add(new SqlParameter("@TSCNo", txtTSCNO.Text.Trim()));
            }

            if (ClassComboBox.SelectedItem != null)
            {
                query += " AND Class = @Class";
                parameters.Add(new SqlParameter("@Class", ClassComboBox.SelectedItem.ToString()));
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(query, conn);
                    foreach (var p in parameters)
                    {
                        cmd.Parameters.Add(p);
                    }

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    Teacherdgv.DataSource = dt;

                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show("No matching teacher found.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error occurred: " + ex.Message);
                }
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Teacherdgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = Teacherdgv.Rows[e.RowIndex];

                txtTeacherID.Text = row.Cells["TeacherID"].Value?.ToString();
                txtName.Text = row.Cells["Name"].Value?.ToString();
                txtTSCNO.Text = row.Cells["TSCNO"].Value?.ToString();
                ClassComboBox.Text = row.Cells["Class"].Value?.ToString();
            }

        }
    }
}
    

    

