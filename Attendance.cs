using iTextSharp.text.pdf;
using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using Excel = Microsoft.Office.Interop.Excel;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.IO;
using Org.BouncyCastle.Asn1.Cmp;
using System.Drawing.Drawing2D;
using System.Drawing;

namespace EduSync
{
    public partial class txtAttend : Form
    {
        public void ExportAttendanceToExcel(DataGridView dgv)
        {
            try
            {
                Excel.Application excelApp = new Excel.Application();
                Excel.Workbook workbook = excelApp.Workbooks.Add();
                Excel.Worksheet worksheet = (Excel.Worksheet)workbook.Sheets[1];

                excelApp.Visible = false;
                worksheet.Name = "Attendance Records";

                // Add column headers
                for (int col = 0; col < dgv.Columns.Count; col++)
                {
                    worksheet.Cells[1, col + 1] = dgv.Columns[col].HeaderText;
                }

                // Add row data
                for (int row = 0; row < dgv.Rows.Count; row++)
                {
                    for (int col = 0; col < dgv.Columns.Count; col++)
                    {
                        if (dgv.Rows[row].Cells[col].Value != null)
                        {
                            worksheet.Cells[row + 2, col + 1] = dgv.Rows[row].Cells[col].Value.ToString();
                        }
                    }
                }

                // Autofit columns
                worksheet.Columns.AutoFit();

                // Save file
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "Excel Workbook|*.xlsx";
                saveDialog.Title = "Save Attendance as Excel";
                saveDialog.FileName = "Attendance_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    workbook.SaveAs(saveDialog.FileName);
                    MessageBox.Show("Export successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // Cleanup
                workbook.Close(false);
                excelApp.Quit();

                ReleaseObject(worksheet);
                ReleaseObject(workbook);
                ReleaseObject(excelApp);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ReleaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch
            {
                obj = null;
            }
            finally
            {
                GC.Collect();
            }
        }

        public txtAttend()
        {
            InitializeComponent();
            ButtonStyler.ApplyPrimaryStyle(btnSubmit);
            ButtonStyler.ApplyPrimaryStyle(btnReset);
            ButtonStyler.ApplyPrimaryStyle(btnUpdate);
            ButtonStyler.ApplyDangerStyle(btnDelete);
            ButtonStyler.ApplyPrimaryStyle(btnExport);
            ButtonStyler.ApplyPrimaryStyle(btnExel);
            ButtonStyler.ApplyPrimaryStyle(btnSearch);
            ButtonStyler.ApplyPrimaryStyle(btnRefresh);
            ButtonStyler.ApplyPrimaryStyle(btnFilter);
           

        }

        private void RstButton_Click(object sender, EventArgs e)
        {

        }

        private void ResetButton_Click(object sender, EventArgs e)
        {


        }



        private void label7_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Back_Click(object sender, EventArgs e)
        {

        }

        private void guna2RadioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void RdoAbsent_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btnEdit_Click(object sender, EventArgs e)
        {



            cmbReason.Enabled = true;
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            // Basic input validation

        }





        private void btnDelete_Click(object sender, EventArgs e)
        {


        }

        private void btnLoad_Click(object sender, EventArgs e)
        {

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {



        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {


        }

        private void btnExport_Click(object sender, EventArgs e)
        {




        }












        private void btnUpdate_Click(object sender, EventArgs e)
        {


        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Dashboard dash = new Dashboard();
            dash.Show();
            this.Close();
        }

        private void btnSubmit_Click_1(object sender, EventArgs e)
        {
            if (!int.TryParse(txtAttendID.Text, out int attendanceID) ||
               !int.TryParse(txtStudentID.Text, out int studentID))
            {
                MessageBox.Show("Please enter valid numbers for Attendance ID and Student ID.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string studentName = txtName.Text.Trim();
            if (string.IsNullOrWhiteSpace(studentName))
            {
                MessageBox.Show("Please enter the Student Name.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string term = cmbTerm.SelectedItem?.ToString();
            string parentContact = txtContact.Text.Trim();
            string reason = cmbReason.SelectedItem?.ToString() ?? "N/A";
            string studentClass = cmbClass.SelectedItem?.ToString();
            DateTime attendanceDate = dtpDate.Value;

            // Nullable status handling
            string status = cmbstatus.SelectedItem != null ? cmbstatus.SelectedItem.ToString() : null;

            // Additional validation
            if (string.IsNullOrWhiteSpace(term) || string.IsNullOrWhiteSpace(studentClass))
            {
                MessageBox.Show("Please select both Term and Class.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string connectionString = "Data Source=DESKTOP-TUS7DBQ;Initial Catalog=EduSync;Integrated Security=True;Encrypt=False";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string query = @"INSERT INTO AttendTbl 
            (AttendanceID, Class, Term, StudentID, StudentName, Reason, Date, ParentContact, Status) 
             VALUES 
            (@AttendanceID, @Class, @Term, @StudentID, @StudentName, @Reason, @Date, @ParentContact, @Status)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@AttendanceID", attendanceID);
                        cmd.Parameters.AddWithValue("@Class", studentClass);
                        cmd.Parameters.AddWithValue("@Term", term);
                        cmd.Parameters.AddWithValue("@StudentID", studentID);
                        cmd.Parameters.AddWithValue("@StudentName", studentName);
                        cmd.Parameters.AddWithValue("@Reason", reason);
                        cmd.Parameters.AddWithValue("@Date", attendanceDate);
                        cmd.Parameters.AddWithValue("@ParentContact", parentContact);

                        if (string.IsNullOrWhiteSpace(status))
                            cmd.Parameters.AddWithValue("@Status", DBNull.Value);
                        else
                            cmd.Parameters.AddWithValue("@Status", status);

                        int result = cmd.ExecuteNonQuery();

                        MessageBox.Show(result > 0 ? "Attendance recorded successfully!" : "Failed to save attendance.",
                                        "Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnDelete_Click_1(object sender, EventArgs e)
        {
            string connectionString = "Data Source=DESKTOP-TUS7DBQ;Initial Catalog=EduSync;Integrated Security=True;Encrypt=False";

            if (!int.TryParse(txtAttendID.Text, out int attendanceID))
            {
                MessageBox.Show("Please enter a valid Attendance ID.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string deleteQuery = "DELETE FROM AttendTbl WHERE AttendanceID = @AttendanceID";

                    using (SqlCommand cmd = new SqlCommand(deleteQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@AttendanceID", attendanceID);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Attendance record deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("No record found with that Attendance ID.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnUpdate_Click_1(object sender, EventArgs e)
        {
            if (!int.TryParse(txtAttendID.Text, out int attendanceID) ||
               !int.TryParse(txtStudentID.Text, out int studentID))
            {
                MessageBox.Show("Please enter valid Attendance ID and Student ID.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string term = cmbTerm.SelectedItem?.ToString();
            string studentClass = cmbClass.SelectedItem?.ToString();
            string reason = cmbReason.SelectedItem?.ToString() ?? "N/A";
            DateTime date = dtpDate.Value;
            string parentContact = txtContact.Text.Trim();
            string status = cmbstatus.SelectedItem?.ToString();

            if (string.IsNullOrWhiteSpace(term) || string.IsNullOrWhiteSpace(studentClass) || string.IsNullOrWhiteSpace(status))
            {
                MessageBox.Show("Please fill in all required fields.", "Missing Info", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string connectionString = "Data Source=DESKTOP-TUS7DBQ;Initial Catalog=EduSync;Integrated Security=True;Encrypt=False";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "UPDATE AttendTbl SET Class = @Class, Term = @Term, StudentID = @StudentID, Reason = @Reason, Date = @Date, ParentContact = @ParentContact, Status = @Status WHERE AttendanceID = @AttendanceID";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Class", studentClass);
                        cmd.Parameters.AddWithValue("@Term", term);
                        cmd.Parameters.AddWithValue("@StudentID", studentID);
                        cmd.Parameters.AddWithValue("@Reason", reason);
                        cmd.Parameters.AddWithValue("@Date", date);
                        cmd.Parameters.AddWithValue("@ParentContact", parentContact);
                        cmd.Parameters.AddWithValue("@Status", status);
                        cmd.Parameters.AddWithValue("@AttendanceID", attendanceID);

                        int result = cmd.ExecuteNonQuery();

                        MessageBox.Show(result > 0 ? "Attendance updated successfully!" : "No record updated.", "Update Status", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            // Clear TextBoxes
            txtAttendID.Clear();
            txtStudentID.Clear();
            txtContact.Clear();

            // Reset ComboBoxes
            cmbTerm.SelectedIndex = -1;
            cmbClass.SelectedIndex = -1;
            cmbReason.SelectedIndex = -1;
            cmbstatus.SelectedIndex = -1;

            // Reset DateTimePicker
            dtpDate.Value = DateTime.Today;

            // Optionally reset status and reason field state
            cmbReason.Enabled = true;

            cmbstatus.SelectedIndex = -1;


            // Optional: Reset focus
            txtAttendID.Focus();
        }

        private void btnLoad_Click_1(object sender, EventArgs e)
        {
            string connectionString = "Data Source=DESKTOP-TUS7DBQ;Initial Catalog=EduSync;Integrated Security=True;Encrypt=False";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string selectQuery = @"SELECT AttendanceID, StudentID, Class, Term, Date, Reason, ParentContact,
                                   CASE 
                                       WHEN ISNULL(Reason, '') = '' THEN 'Present' 
                                       ELSE 'Absent' 
                                   END AS Status
                                   FROM AttendTbl";
                    SqlDataAdapter adapter = new SqlDataAdapter(selectQuery, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvAttendance.AutoGenerateColumns = true;
                    dgvAttendance.ColumnHeadersVisible = true;
                    dgvAttendance.DataSource = dt;

                    dgvAttendance.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading attendance: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnRefresh_Click_1(object sender, EventArgs e)
        {
            string connectionString = "Data Source=DESKTOP-TUS7DBQ;Initial Catalog=EduSync;Integrated Security=True;Encrypt=False";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM AttendTbl";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dgvAttendance.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnSearch_Click_1(object sender, EventArgs e)
        {
            string connectionString = "Data Source=DESKTOP-TUS7DBQ;Initial Catalog=EduSync;Integrated Security=True;Encrypt=False";

            if (!int.TryParse(txtStudentID.Text.Trim(), out int studentID))
            {
                MessageBox.Show("Please enter a valid Student ID.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM AttendTbl WHERE StudentID = @StudentID";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    adapter.SelectCommand.Parameters.AddWithValue("@StudentID", studentID);

                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvAttendance.DataSource = dt;

                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show("No records found for the entered Student ID.", "Search Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDlg = new FolderBrowserDialog())
            {
                if (folderDlg.ShowDialog() == DialogResult.OK)
                {
                    string saveFolderPath = folderDlg.SelectedPath;

                    foreach (DataGridViewRow studentRow in dgvAttendance.Rows)
                    {
                        if (!studentRow.IsNewRow)
                        {
                            string studentName = studentRow.Cells["StudentName"].Value?.ToString();

                            // Customize the filename for each student
                            string fileName = $"{studentName}_Attendance_Term1.pdf";
                            string fullPath = Path.Combine(saveFolderPath, fileName);

                            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
                            {
                                Document doc = new Document(PageSize.A4.Rotate(), 25, 25, 30, 30);
                                PdfWriter writer = PdfWriter.GetInstance(doc, stream);
                                doc.Open();

                                // === Header Title ===
                                Paragraph title = new Paragraph("EduSync - Individual Attendance Report",
                                    FontFactory.GetFont("Arial", 16, iTextSharp.text.Font.BOLD, new BaseColor(30, 50, 100)));
                                title.Alignment = Element.ALIGN_CENTER;
                                title.SpacingAfter = 15f;
                                doc.Add(title);

                                // === Student Info Box ===
                                PdfPTable studentInfoTable = new PdfPTable(1);
                                studentInfoTable.WidthPercentage = 100;

                                string studentInfoText = $"Name: {studentName}\nClass: 6B\nTerm: Term 2\nDate: {DateTime.Now:dd MMM yyyy}";
                                PdfPCell infoCell = new PdfPCell(new Phrase(studentInfoText, FontFactory.GetFont("Arial", 11)));
                                infoCell.BackgroundColor = new BaseColor(230, 240, 255); // soft blue
                                infoCell.Padding = 8f;

                                infoCell.Border = iTextSharp.text.Rectangle.NO_BORDER;

                                studentInfoTable.AddCell(infoCell);
                                doc.Add(studentInfoTable);

                                // === Attendance Summary Title ===
                                Paragraph summaryTitle = new Paragraph("Attendance Summary",
                                    FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.BOLD));
                                summaryTitle.SpacingBefore = 15f;
                                summaryTitle.SpacingAfter = 10f;
                                doc.Add(summaryTitle);

                                // === Attendance Table ===
                                PdfPTable table = new PdfPTable(dgvAttendance.Columns.Count);
                                table.WidthPercentage = 100;
                                table.SpacingBefore = 5f;

                                // Headers with background color
                                foreach (DataGridViewColumn col in dgvAttendance.Columns)
                                {
                                    PdfPCell headerCell = new PdfPCell(new Phrase(col.HeaderText, FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)));
                                    headerCell.BackgroundColor = new BaseColor(200, 230, 250); // soft light blue
                                    headerCell.Padding = 5f;
                                    table.AddCell(headerCell);
                                }

                                // Student Row
                                foreach (DataGridViewCell cell in studentRow.Cells)
                                {
                                    string cellText = cell.Value?.ToString() ?? "";
                                    PdfPCell cellItem = new PdfPCell(new Phrase(cellText, FontFactory.GetFont("Arial", 10)));
                                    cellItem.Padding = 4f;
                                    table.AddCell(cellItem);
                                }

                                doc.Add(table);

                                // === Footer ===
                                doc.Add(new Paragraph("\n\nParent Signature: ______________________", FontFactory.GetFont("Arial", 11)));
                                doc.Add(new Paragraph("\nGenerated by EduSync", FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.ITALIC, BaseColor.GRAY)));

                                doc.Close();
                                writer.Close();
                                stream.Close();
                            }
                        }
                    }
                }
            }
        }

        private void btnExel_Click(object sender, EventArgs e)
        {
            ExportAttendanceToExcel(dgvAttendance);
        }


private void MakeButtonRound(System.Windows.Forms. Button btn)
    {
        
    }
        

    private void txtAttend_Load(object sender, EventArgs e)
        {
            UIHelper.StyleGrid(dgvAttendance);
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is System.Windows.Forms. Button btn)
                {
                    MakeButtonRound(btn);
                }
            }
            StyleDataGridView();

            LoadCombo(cmbClass, "SELECT ClassName FROM ClassTable ORDER BY ClassName");
            LoadCombo(cmbTerm, "SELECT TermName FROM TermTable ORDER BY TermName");
            LoadCombo(cmbstatus, "SELECT StatusName FROM StatusTable ORDER BY StatusName");
            LoadCombo(cmbReason, "SELECT ReasonName FROM ReasonTable ORDER BY ReasonName");
        }
        private void StyleDataGridView()
        {
            dgvAttendance.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10);
            dgvAttendance.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10, FontStyle.Bold);
            dgvAttendance.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(45, 156, 219);
            dgvAttendance.DefaultCellStyle.SelectionBackColor = Color.LightBlue;

            dgvAttendance.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvAttendance.EnableHeadersVisualStyles = false;
            dgvAttendance.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvAttendance.RowTemplate.Height = 28;
            dgvAttendance.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
            dgvAttendance.GridColor = Color.LightGray;
        }


        private void LoadCombo(System.Windows.Forms.ComboBox combo, string query)
        {
            string connStr = "Data Source=DESKTOP-TUS7DBQ;Initial Catalog=EduSync;Integrated Security=True;Encrypt=False";

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    combo.Items.Clear();

                    while (reader.Read())
                    {
                        combo.Items.Add(reader[0].ToString());
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading combo: " + ex.Message);
                }
            }
        }

       

        private void button1_Click(object sender, EventArgs e)
        {

        
            string connectionString = "Data Source=DESKTOP-TUS7DBQ;Initial Catalog=EduSync;Integrated Security=True;Encrypt=False";

            // Validate if user entered something
            if (string.IsNullOrWhiteSpace(txtsearch.Text))
            {
                MessageBox.Show("Please enter a student name to search.");
                return;
            }

            // Query filters only by name
            string query = "SELECT * FROM AttendTbl WHERE StudentName LIKE @StudentName";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@StudentName", "%" + txtsearch.Text.Trim() + "%");

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dgvAttendance.DataSource = dt;

                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show("No matching attendance records found.");
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

        private void dgvAttendance_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvAttendance.Rows[e.RowIndex];
                txtAttendID.Text = row.Cells["AttendanceID"].Value?.ToString();
                txtStudentID.Text = row.Cells["StudentID"].Value?.ToString();
                txtName.Text = row.Cells["StudentName"].Value?.ToString();
                dtpDate.Text = row.Cells["Date"].Value?.ToString();
                cmbClass.Text = row.Cells["Class"].Value?.ToString();
                txtContact.Text = row.Cells["ParentContact"].Value?.ToString();
                cmbstatus.Text = row.Cells["status"].Value?.ToString();
                cmbReason.Text = row.Cells["Reason"].Value?.ToString();
                cmbTerm.Text = row.Cells["Term"].Value.ToString();
            }

        }

        private void dgvAttendance_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}



    
            
        

    





       


