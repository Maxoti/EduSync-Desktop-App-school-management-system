using ClassFeeReports;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.IsisMtt.X509;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.ComponentModel.Design.ObjectSelectorEditor;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace EduSync
{
    public partial class FeeManager : Form
    {
        private string connectionString = "Data Source=DESKTOP-TUS7DBQ;Initial Catalog=EduSync;Integrated Security=True;Encrypt=False";
        private FeeCalculationService feeCalculationService;
        private FeeReportGenerator reportGenerator;
        public FeeManager()
        {
            InitializeComponent();
            ButtonStyler.ApplyPrimaryStyle(btnAdd);
            ButtonStyler.ApplyPrimaryStyle(btnReset);
            ButtonStyler.ApplyPrimaryStyle(btnUpdate);
            ButtonStyler.ApplyDangerStyle(btnDelete);
            ButtonStyler.ApplyPrimaryStyle(btnMPESA);
            ButtonStyler.ApplyPrimaryStyle(btnPrint);
            ButtonStyler.ApplyPrimaryStyle(btnsearch);
        }

        private void LoadFeeRecords()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM FeeTbl ORDER BY FeeID ASC";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvFees.DataSource = dt;
                    AdjustDataGridView();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load fee records:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void MakeButtonRound(System.Windows.Forms.Button btn)
        {
            //
        }

        private void FeeManager_Load(object sender, EventArgs e)
        {
            // Initialize UI styling
            UIHelper.StyleGrid(dgvFees);
            txtTotalPaid.Font = new System.Drawing.Font("Verdana", 8, System.Drawing.FontStyle.Bold);
            txtTotalBalance.Font = new System.Drawing.Font("Verdana", 8, System.Drawing.FontStyle.Bold);
            StyleDataGridView();
            SetupDataGridView();

            // Load combo boxes
            LoadCombo(ClassCombo, "SELECT ClassName FROM ClassTable ORDER BY ClassName");
            LoadCombo(cmbPaymentMethod, "SELECT MethodName FROM PaymentMethodTable ORDER BY MethodName");
            LoadCombo(cmbTerm, "SELECT TermName FROM TermTable ORDER BY TermName");
            LoadCombo(cmbfee, "SELECT FeeTypeName FROM FeeTypeTable ORDER BY FeeTypeName");

            LoadFeeRecords();

            var feeService = new FeeCalculationService();
            reportGenerator = new FeeReportGenerator(feeService);

            // Make buttons round
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is System.Windows.Forms.Button btn)
                {
                    MakeButtonRound(btn);
                }
            }
        }

        private void LoadCombo(System.Windows.Forms.ComboBox comboBox, string query)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    comboBox.Items.Clear();

                    while (reader.Read())
                    {
                        comboBox.Items.Add(reader[0].ToString());
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading combo: {ex.Message}");
                }
            }
        }

        private void AdjustDataGridView()
        {
            if (dgvFees.Columns.Count > 0)
            {
                dgvFees.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
        }

        private void StyleDataGridView()
        {
            dgvFees.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10);
            dgvFees.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10, FontStyle.Bold);
            dgvFees.DefaultCellStyle.SelectionBackColor = Color.LightBlue;
            dgvFees.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(45, 156, 219);
            dgvFees.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvFees.EnableHeadersVisualStyles = false;
            dgvFees.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvFees.RowTemplate.Height = 28;
            dgvFees.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
            dgvFees.GridColor = Color.LightGray;
        }

        private void SetupDataGridView()
        {
            dgvFees.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvFees.MultiSelect = false;
            dgvFees.ReadOnly = true;
            dgvFees.AllowUserToAddRows = false;
            dgvFees.AllowUserToDeleteRows = false;
        }

        private void btnAdd_Click_1(object sender, EventArgs e)
        {
            try
            {
                // Validate required fields
                if (string.IsNullOrWhiteSpace(txtFee.Text) ||
                    string.IsNullOrWhiteSpace(txtStudentID.Text) ||
                    string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                    string.IsNullOrWhiteSpace(txtLastName.Text) ||
                    string.IsNullOrWhiteSpace(txtAmountPaid.Text) ||
                    string.IsNullOrWhiteSpace(txtYear.Text) ||
                    string.IsNullOrWhiteSpace(txtTotalfees.Text) ||
                    cmbfee.SelectedItem == null ||
                    ClassCombo.SelectedItem == null ||
                    cmbPaymentMethod.SelectedItem == null ||
                    cmbTerm.SelectedItem == null)
                {
                    MessageBox.Show("Please fill in all fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Parse and validate numeric values
                if (!decimal.TryParse(txtAmountPaid.Text.Trim(), out decimal amountPaid) ||
                    !decimal.TryParse(txtTotalfees.Text.Trim(), out decimal totalFee))
                {
                    MessageBox.Show("Please enter valid numeric values for Amount Paid and Total Fees.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Validate that amount paid is not negative
                if (amountPaid < 0 || totalFee < 0)
                {
                    MessageBox.Show("Amount Paid and Total Fees cannot be negative.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Compute balance
                decimal balance = totalFee - amountPaid;
                txtBalance.Text = balance.ToString("N2");

                // Get values
                string feeID = txtFee.Text.Trim();
                string studentID = txtStudentID.Text.Trim();
                string firstName = txtFirstName.Text.Trim();
                string lastName = txtLastName.Text.Trim();
                string feeType = cmbfee.SelectedItem.ToString();
                string className = ClassCombo.SelectedItem.ToString();
                string paymentMethod = cmbPaymentMethod.SelectedItem.ToString();
                string term = cmbTerm.SelectedItem.ToString();
                string year = txtYear.Text.Trim();
                DateTime datePaid = DateTimePicker.Value;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                    INSERT INTO FeeTbl 
                    (FeeID, StudentID, FirstName, LastName, FeeType, AmountPaid, Balance, Class, PaymentMethod, TotalFee, Term, Year, DatePaid)
                    VALUES 
                    (@FeeID, @StudentID, @FirstName, @LastName, @FeeType, @AmountPaid, @Balance, @Class, @PaymentMethod, @TotalFee, @Term, @Year, @DatePaid)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@FeeID", feeID);
                        cmd.Parameters.AddWithValue("@StudentID", studentID);
                        cmd.Parameters.AddWithValue("@FirstName", firstName);
                        cmd.Parameters.AddWithValue("@LastName", lastName);
                        cmd.Parameters.AddWithValue("@FeeType", feeType);
                        cmd.Parameters.AddWithValue("@AmountPaid", amountPaid);
                        cmd.Parameters.AddWithValue("@Balance", balance);
                        cmd.Parameters.AddWithValue("@Class", className);
                        cmd.Parameters.AddWithValue("@PaymentMethod", paymentMethod);
                        cmd.Parameters.AddWithValue("@TotalFee", totalFee);
                        cmd.Parameters.AddWithValue("@Term", term);
                        cmd.Parameters.AddWithValue("@Year", year);
                        cmd.Parameters.AddWithValue("@DatePaid", datePaid);

                        conn.Open();
                        cmd.ExecuteNonQuery();

                        MessageBox.Show($"Fee payment of KES {amountPaid:N2} recorded successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        btnReset_Click_1(sender, e);
                        LoadFeeRecords();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error submitting fee payment:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdate_Click_1(object sender, EventArgs e)
        {
            // Check if any row is selected
            if (dgvFees.SelectedRows.Count == 0 && dgvFees.CurrentRow == null)
            {
                MessageBox.Show("Please select a record to update.", "Update", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validate all required fields
            if (string.IsNullOrWhiteSpace(txtFee.Text) ||
                string.IsNullOrWhiteSpace(txtStudentID.Text) ||
                string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                string.IsNullOrWhiteSpace(txtLastName.Text) ||
                string.IsNullOrWhiteSpace(txtAmountPaid.Text) ||
                string.IsNullOrWhiteSpace(txtTotalfees.Text) ||
                string.IsNullOrWhiteSpace(txtYear.Text) ||
                cmbfee.SelectedItem == null ||
                ClassCombo.SelectedItem == null ||
                cmbPaymentMethod.SelectedItem == null ||
                cmbTerm.SelectedItem == null)
            {
                MessageBox.Show("Please fill in all fields before updating.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string feeID = txtFee.Text.Trim();
                string studentID = txtStudentID.Text.Trim();
                string firstName = txtFirstName.Text.Trim();
                string lastName = txtLastName.Text.Trim();
                string feeType = cmbfee.SelectedItem.ToString();
                string className = ClassCombo.SelectedItem.ToString();
                string paymentMethod = cmbPaymentMethod.SelectedItem.ToString();
                string term = cmbTerm.SelectedItem.ToString();
                string year = txtYear.Text.Trim();
                DateTime datePaid = DateTimePicker.Value;

                // Parse and validate numeric values
                if (!decimal.TryParse(txtAmountPaid.Text.Trim(), out decimal amountPaid) ||
                    !decimal.TryParse(txtTotalfees.Text.Trim(), out decimal totalFees))
                {
                    MessageBox.Show("Invalid numeric input for Amount Paid or Total Fees.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Calculate balance
                decimal balance = totalFees - amountPaid;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                    UPDATE FeeTbl 
                    SET 
                        StudentID = @StudentID,
                        FirstName = @FirstName,
                        LastName = @LastName,
                        FeeType = @FeeType,
                        AmountPaid = @AmountPaid,
                        Balance = @Balance,
                        Class = @Class,
                        PaymentMethod = @PaymentMethod,
                        TotalFee = @TotalFee,
                        Term = @Term,
                        Year = @Year,
                        DatePaid = @DatePaid
                    WHERE FeeID = @FeeID";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@FeeID", feeID);
                        cmd.Parameters.AddWithValue("@StudentID", studentID);
                        cmd.Parameters.AddWithValue("@FirstName", firstName);
                        cmd.Parameters.AddWithValue("@LastName", lastName);
                        cmd.Parameters.AddWithValue("@FeeType", feeType);
                        cmd.Parameters.AddWithValue("@AmountPaid", amountPaid);
                        cmd.Parameters.AddWithValue("@Balance", balance);
                        cmd.Parameters.AddWithValue("@Class", className);
                        cmd.Parameters.AddWithValue("@PaymentMethod", paymentMethod);
                        cmd.Parameters.AddWithValue("@TotalFee", totalFees);
                        cmd.Parameters.AddWithValue("@Term", term);
                        cmd.Parameters.AddWithValue("@Year", year);
                        cmd.Parameters.AddWithValue("@DatePaid", datePaid);

                        conn.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Record updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            txtBalance.Text = balance.ToString("N2");
                            LoadFeeRecords();
                        }
                        else
                        {
                            MessageBox.Show("No record was updated. Check Fee ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating record:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click_1(object sender, EventArgs e)
        {
            // Check both SelectedRows and CurrentRow
            if (dgvFees.SelectedRows.Count == 0 && dgvFees.CurrentRow == null)
            {
                MessageBox.Show("Please select a row to delete.", "Delete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Get the row to delete
            DataGridViewRow rowToDelete = dgvFees.SelectedRows.Count > 0
                ? dgvFees.SelectedRows[0]
                : dgvFees.CurrentRow;

            if (rowToDelete?.Cells["FeeID"]?.Value == null)
            {
                MessageBox.Show("Invalid row selected. Please try again.", "Delete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show("Are you sure you want to delete this fee record?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                try
                {
                    string feeID = rowToDelete.Cells["FeeID"].Value.ToString();

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        string deleteQuery = "DELETE FROM FeeTbl WHERE FeeID = @FeeID";

                        using (SqlCommand cmd = new SqlCommand(deleteQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@FeeID", feeID);

                            conn.Open();
                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Fee record deleted successfully.", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                btnReset_Click_1(null, null);
                                LoadFeeRecords();
                            }
                            else
                            {
                                MessageBox.Show("Deletion failed. Record not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error while deleting record:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnPrint_Click_1(object sender, EventArgs e)
        {
            // Check both SelectedRows and CurrentRow
            if (dgvFees.SelectedRows.Count == 0 && dgvFees.CurrentRow == null)
            {
                MessageBox.Show("Please select a row to print the receipt.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Get the row to print
                DataGridViewRow row = dgvFees.SelectedRows.Count > 0
                    ? dgvFees.SelectedRows[0]
                    : dgvFees.CurrentRow;

                // Validate row data
                if (row?.Cells["FeeID"]?.Value == null || row?.Cells["StudentID"]?.Value == null)
                {
                    MessageBox.Show("Invalid row selected. Please select a valid fee record.", "Print Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Extract data with null checking
                string feeID = row.Cells["FeeID"].Value?.ToString() ?? "";
                string studentID = row.Cells["StudentID"].Value?.ToString() ?? "";
                string firstName = row.Cells["FirstName"].Value?.ToString() ?? "";
                string lastName = row.Cells["LastName"].Value?.ToString() ?? "";
                string fullName = $"{firstName} {lastName}".Trim();
                string className = row.Cells["Class"].Value?.ToString() ?? "";
                string feeType = row.Cells["FeeType"].Value?.ToString() ?? "";
                string paymentMethod = row.Cells["PaymentMethod"].Value?.ToString() ?? "";
                string amountPaid = row.Cells["AmountPaid"].Value?.ToString() ?? "0.00";
                string balance = row.Cells["Balance"].Value?.ToString() ?? "0.00";
                string totalFee = row.Cells["TotalFee"].Value?.ToString() ?? "0.00";
                string term = row.Cells["Term"].Value?.ToString() ?? "";
                string year = row.Cells["Year"].Value?.ToString() ?? "";

                string datePaid = "";
                if (row.Cells["DatePaid"].Value != null && DateTime.TryParse(row.Cells["DatePaid"].Value.ToString(), out DateTime parsedDate))
                {
                    datePaid = parsedDate.ToShortDateString();
                }
                else
                {
                    datePaid = DateTime.Now.ToShortDateString();
                }

                // Generate PDF
                string fileName = $"FeeReceipt_{feeID}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
                string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string fullPath = Path.Combine(folderPath, fileName);

                Document doc = new Document();
                PdfWriter.GetInstance(doc, new FileStream(fullPath, FileMode.Create));
                doc.Open();

                // Handle logo with error checking
                string logoPath = Path.Combine(Application.StartupPath, "Resources", "logo.png");
                if (File.Exists(logoPath))
                {
                    try
                    {
                        iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(logoPath);
                        logo.ScaleToFit(120f, 120f);
                        logo.Alignment = Element.ALIGN_CENTER;
                        doc.Add(logo);
                    }
                    catch
                    {
                        doc.Add(new Paragraph("EDUSYNC LOGO\n"));
                    }
                }

                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
                var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);

                doc.Add(new Paragraph("MANDELA SCHOOL MANAGEMENT SYSTEM", titleFont));
                doc.Add(new Paragraph("FEE PAYMENT RECEIPT", titleFont));
                doc.Add(new Paragraph("----------------------------------------------------"));
                doc.Add(new Paragraph($"Receipt Date: {DateTime.Now.ToShortDateString()}", normalFont));
                doc.Add(new Paragraph("\n"));

                doc.Add(new Paragraph($"Fee ID: {feeID}", normalFont));
                doc.Add(new Paragraph($"Student ID: {studentID}", normalFont));
                doc.Add(new Paragraph($"Name: {fullName}", normalFont));
                doc.Add(new Paragraph($"Class: {className}", normalFont));
                doc.Add(new Paragraph($"Term: {term}, Year: {year}", normalFont));
                doc.Add(new Paragraph($"Fee Type: {feeType}", normalFont));
                doc.Add(new Paragraph($"Total Fee: KES {totalFee}", normalFont));
                doc.Add(new Paragraph($"Amount Paid: KES {amountPaid}", normalFont));
                doc.Add(new Paragraph($"Balance: KES {balance}", normalFont));
                doc.Add(new Paragraph($"Payment Method: {paymentMethod}", normalFont));
                doc.Add(new Paragraph($"Date Paid: {datePaid}", normalFont));

                doc.Add(new Paragraph("\n\nSignature: _______________________"));
                doc.Add(new Paragraph("Stamp: ___________________________"));

                doc.Close();

                MessageBox.Show($"Receipt exported to PDF successfully!\nSaved to: {fullPath}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Open the PDF
                Process.Start(new ProcessStartInfo
                {
                    FileName = fullPath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating receipt:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnReset_Click_1(object sender, EventArgs e)
        {
            txtFee.Text = "";
            txtStudentID.Text = "";
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtAmountPaid.Text = "";
            txtTotalfees.Text = "";
            txtBalance.Text = "";
            txtYear.Text = "";

            cmbfee.SelectedIndex = -1;
            ClassCombo.SelectedIndex = -1;
            cmbPaymentMethod.SelectedIndex = -1;
            cmbTerm.SelectedIndex = -1;

            DateTimePicker.Value = DateTime.Now;

            txtFee.Focus();
        }

        private void dgvFees_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvFees.Rows[e.RowIndex].Cells["FeeID"].Value != null)
            {
                DataGridViewRow row = dgvFees.Rows[e.RowIndex];

                txtFee.Text = row.Cells["FeeID"].Value?.ToString() ?? "";
                txtStudentID.Text = row.Cells["StudentID"].Value?.ToString() ?? "";
                txtFirstName.Text = row.Cells["FirstName"].Value?.ToString() ?? "";
                txtLastName.Text = row.Cells["LastName"].Value?.ToString() ?? "";
                txtAmountPaid.Text = row.Cells["AmountPaid"].Value?.ToString() ?? "";
                txtTotalfees.Text = row.Cells["TotalFee"].Value?.ToString() ?? "";
                txtBalance.Text = row.Cells["Balance"].Value?.ToString() ?? "";
                txtYear.Text = row.Cells["Year"].Value?.ToString() ?? "";

                // Set combo box selections
                string feeType = row.Cells["FeeType"].Value?.ToString();
                if (!string.IsNullOrEmpty(feeType) && cmbfee.Items.Contains(feeType))
                    cmbfee.SelectedItem = feeType;

                string className = row.Cells["Class"].Value?.ToString();
                if (!string.IsNullOrEmpty(className) && ClassCombo.Items.Contains(className))
                    ClassCombo.SelectedItem = className;

                string paymentMethod = row.Cells["PaymentMethod"].Value?.ToString();
                if (!string.IsNullOrEmpty(paymentMethod) && cmbPaymentMethod.Items.Contains(paymentMethod))
                    cmbPaymentMethod.SelectedItem = paymentMethod;

                string term = row.Cells["Term"].Value?.ToString();
                if (!string.IsNullOrEmpty(term) && cmbTerm.Items.Contains(term))
                    cmbTerm.SelectedItem = term;

                // Set date
                if (row.Cells["DatePaid"].Value != null && DateTime.TryParse(row.Cells["DatePaid"].Value.ToString(), out DateTime datePaid))
                {
                    DateTimePicker.Value = datePaid;
                }
            }
        }

        private void btnsearch_Click(object sender, EventArgs e)
        {
            try
            {
                string term = cmbTerm.Text.Trim();

                if (string.IsNullOrWhiteSpace(term))
                {
                    MessageBox.Show("Please enter a valid term.", "Input Error",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string query = @"
                SELECT FeeID, StudentID, FirstName, LastName, AmountPaid, Balance, Term, 
                       Class, FeeType, Year, DatePaid, PaymentMethod, TotalFee
                FROM FeeTbl
                WHERE Term LIKE @term
                ORDER BY LastName, FirstName;

                SELECT 
                    ISNULL(SUM(AmountPaid), 0) AS TotalPaid, 
                    ISNULL(SUM(Balance), 0) AS TotalBalance,
                    COUNT(*) AS TotalStudents,
                    COUNT(CASE WHEN Balance > 0 THEN 1 END) AS StudentsWithBalance
                FROM FeeTbl
                WHERE Term LIKE @term;
            ";

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@term", "%" + term + "%");

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        adapter.Fill(ds);

                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            // Populate DataGridView with individual records
                            dgvFees.DataSource = ds.Tables[0];

                            // Format the DataGridView
                            FormatFeeDataGridView();

                            // Populate textboxes with totals from the second query result
                            if (ds.Tables[1].Rows.Count > 0)
                            {
                                var totalsRow = ds.Tables[1].Rows[0];
                                txtTotalPaid.Text = Convert.ToDecimal(totalsRow["TotalPaid"]).ToString("N2");
                                txtTotalBalance.Text = Convert.ToDecimal(totalsRow["TotalBalance"]).ToString("N2");

                                // Color-code the balance
                                decimal totalBalance = Convert.ToDecimal(totalsRow["TotalBalance"]);
                                txtTotalBalance.ForeColor = totalBalance > 0 ? Color.Red : Color.Black;

                                // Show summary information
                                string summaryMessage = $"Term: {term}\n" +
                                                      $"Total Students: {totalsRow["TotalStudents"]}\n" +
                                                      $"Students with Outstanding Balance: {totalsRow["StudentsWithBalance"]}\n" +
                                                      $"Total Paid: KSH {Convert.ToDecimal(totalsRow["TotalPaid"]):N2}\n" +
                                                      $"Total Balance: KSH {totalBalance:N2}";

                                MessageBox.Show(summaryMessage, "Term Search Results",
                                              MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                        else
                        {
                            dgvFees.DataSource = null;
                            txtTotalPaid.Text = "0.00";
                            txtTotalBalance.Text = "0.00";
                            MessageBox.Show($"No fee records found for term: {term}",
                                          "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching fee records: {ex.Message}",
                                "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormatFeeDataGridView()
        {
            try
            {
                // Format AmountPaid column
                if (dgvFees.Columns.Contains("AmountPaid"))
                {
                    dgvFees.Columns["AmountPaid"].DefaultCellStyle.Format = "N2";
                    dgvFees.Columns["AmountPaid"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvFees.Columns["AmountPaid"].HeaderText = "Amount Paid";
                }

                // Format Balance column with color coding
                if (dgvFees.Columns.Contains("Balance"))
                {
                    dgvFees.Columns["Balance"].DefaultCellStyle.Format = "N2";
                    dgvFees.Columns["Balance"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                    // Color-code balances
                    foreach (DataGridViewRow row in dgvFees.Rows)
                    {
                        if (row.Cells["Balance"].Value != null)
                        {
                            decimal balance = Convert.ToDecimal(row.Cells["Balance"].Value);
                            if (balance > 0)
                                row.Cells["Balance"].Style.ForeColor = Color.Red;
                            else if (balance < 0)
                                row.Cells["Balance"].Style.ForeColor = Color.Green;
                            else
                                row.Cells["Balance"].Style.ForeColor = Color.Black;
                        }
                    }
                }

                // Format TotalFee column
                if (dgvFees.Columns.Contains("TotalFee"))
                {
                    dgvFees.Columns["TotalFee"].DefaultCellStyle.Format = "N2";
                    dgvFees.Columns["TotalFee"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvFees.Columns["TotalFee"].HeaderText = "Total Fee";
                }

                // Format DatePaid column
                if (dgvFees.Columns.Contains("DatePaid"))
                {
                    dgvFees.Columns["DatePaid"].DefaultCellStyle.Format = "dd/MM/yyyy";
                    dgvFees.Columns["DatePaid"].HeaderText = "Date Paid";
                }

                // Set column widths
                if (dgvFees.Columns.Contains("StudentID"))
                    dgvFees.Columns["StudentID"].Width = 100;
                if (dgvFees.Columns.Contains("FirstName"))
                    dgvFees.Columns["FirstName"].Width = 120;
                if (dgvFees.Columns.Contains("LastName"))
                    dgvFees.Columns["LastName"].Width = 120;
                if (dgvFees.Columns.Contains("Class"))
                    dgvFees.Columns["Class"].Width = 80;
                if (dgvFees.Columns.Contains("Term"))
                    dgvFees.Columns["Term"].Width = 80;

                // Auto-resize remaining columns
                dgvFees.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
            }
            catch (Exception ex)
            {
                // Log error but don't show message to user as this is formatting
                System.Diagnostics.Debug.WriteLine($"Error formatting DataGridView: {ex.Message}");
            }
        }

        private void SearchFeeRecords(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    MessageBox.Show("Please enter a search term.", "Input Error",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string query = @"SELECT FeeID, StudentID, FirstName, LastName, AmountPaid, Balance, 
                    Class, PaymentMethod, FeeType, Term, Year, DatePaid, TotalFee 
                    FROM FeeTbl
                    WHERE StudentID LIKE @search 
                    OR FirstName LIKE @search 
                    OR LastName LIKE @search 
                    OR CONCAT(FirstName, ' ', LastName) LIKE @search
                    OR Term LIKE @search
                    ORDER BY DatePaid DESC, LastName, FirstName";

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@search", "%" + searchTerm + "%");

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            dgvFees.DataSource = dt;
                            FormatFeeDataGridView();

                            // Calculate totals for search results
                            decimal totalPaid = 0, totalBalance = 0;
                            foreach (DataRow row in dt.Rows)
                            {
                                totalPaid += Convert.ToDecimal(row["AmountPaid"]);
                                totalBalance += Convert.ToDecimal(row["Balance"]);
                            }

                            txtTotalPaid.Text = totalPaid.ToString("N2");
                            txtTotalBalance.Text = totalBalance.ToString("N2");

                            MessageBox.Show($"Found {dt.Rows.Count} record(s) matching '{searchTerm}'",
                                          "Search Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            dgvFees.DataSource = null;
                            txtTotalPaid.Text = "0.00";
                            txtTotalBalance.Text = "0.00";
                            MessageBox.Show($"No records found matching '{searchTerm}'",
                                          "Search Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching fee records: {ex.Message}",
                               "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SearchFeeByTerm(string term)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(term))
                {
                    MessageBox.Show("Please enter a valid term.", "Input Error",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string query = @"
                SELECT FeeID, StudentID, FirstName, LastName, AmountPaid, Balance, Term, 
                       Class, FeeType, Year, DatePaid, PaymentMethod, TotalFee
                FROM FeeTbl
                WHERE Term LIKE @term
                ORDER BY LastName, FirstName;

                SELECT 
                    ISNULL(SUM(AmountPaid), 0) AS TotalPaid, 
                    ISNULL(SUM(Balance), 0) AS TotalBalance,
                    COUNT(*) AS TotalStudents,
                    COUNT(CASE WHEN Balance > 0 THEN 1 END) AS StudentsWithBalance
                FROM FeeTbl
                WHERE Term LIKE @term;
            ";

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@term", "%" + term + "%");

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        adapter.Fill(ds);

                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            dgvFees.DataSource = ds.Tables[0];
                            FormatFeeDataGridView();

                            if (ds.Tables[1].Rows.Count > 0)
                            {
                                var totalsRow = ds.Tables[1].Rows[0];
                                txtTotalPaid.Text = Convert.ToDecimal(totalsRow["TotalPaid"]).ToString("N2");
                                txtTotalBalance.Text = Convert.ToDecimal(totalsRow["TotalBalance"]).ToString("N2");

                                decimal totalBalance = Convert.ToDecimal(totalsRow["TotalBalance"]);
                                txtTotalBalance.ForeColor = totalBalance > 0 ? Color.Red : Color.Black;

                                string summaryMessage = $"Term: {term}\n" +
                                                      $"Total Students: {totalsRow["TotalStudents"]}\n" +
                                                      $"Students with Outstanding Balance: {totalsRow["StudentsWithBalance"]}\n" +
                                                      $"Total Paid: KSH {Convert.ToDecimal(totalsRow["TotalPaid"]):N2}\n" +
                                                      $"Total Balance: KSH {totalBalance:N2}";

                                MessageBox.Show(summaryMessage, "Term Search Results",
                                              MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                        else
                        {
                            dgvFees.DataSource = null;
                            txtTotalPaid.Text = "0.00";
                            txtTotalBalance.Text = "0.00";
                            MessageBox.Show($"No fee records found for term: {term}",
                                          "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching fee records: {ex.Message}",
                                "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // M-PESA Integration
        private async void btnMPESA_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate required fields
                if (string.IsNullOrWhiteSpace(txtStudentID.Text) ||
                    string.IsNullOrWhiteSpace(txtAmountPaid.Text))
                {
                    MessageBox.Show("Please enter Student ID and Amount to process M-Pesa payment.",
                                   "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Get phone number from user
                string phoneNumber = Microsoft.VisualBasic.Interaction.InputBox(
                    "Enter M-Pesa phone number (254XXXXXXXXX):",
                    "M-Pesa Payment",
                    "254");

                if (string.IsNullOrWhiteSpace(phoneNumber))
                {
                    MessageBox.Show("Phone number is required for M-Pesa payment.",
                                   "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!IsValidPhoneNumber(phoneNumber))
                {
                    MessageBox.Show("Please enter a valid phone number in format 254XXXXXXXXX",
                                   "Invalid Phone Number", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(txtAmountPaid.Text.Trim(), out decimal amount) || amount <= 0)
                {
                    MessageBox.Show("Please enter a valid amount.",
                                   "Invalid Amount", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Show confirmation
                DialogResult confirm = MessageBox.Show(
                    $"Initiate M-Pesa payment of KES {amount:N2} to {phoneNumber}?",
                    "Confirm M-Pesa Payment",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirm == DialogResult.Yes)
                {
                    // Disable button to prevent double-clicking
                    btnMPESA.Enabled = false;
                    btnMPESA.Text = "Processing...";

                    try
                    {
                        bool success = await ProcessMpesaPayment(phoneNumber, amount);

                        if (success)
                        {
                            MessageBox.Show("M-Pesa payment request sent successfully! Please complete the payment on your phone.",
                                          "M-Pesa Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Optionally set payment method to M-Pesa
                            if (cmbPaymentMethod.Items.Contains("M-Pesa"))
                                cmbPaymentMethod.SelectedItem = "M-Pesa";
                        }
                        else
                        {
                            MessageBox.Show("Failed to initiate M-Pesa payment. Please try again or use another payment method.",
                                          "M-Pesa Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    finally
                    {
                        btnMPESA.Enabled = true;
                        btnMPESA.Text = "M-PESA";
                    }
                }
            }
            catch (Exception ex)
            {
                btnMPESA.Enabled = true;
                btnMPESA.Text = "M-PESA";
                MessageBox.Show($"Error processing M-Pesa payment: {ex.Message}",
                               "M-Pesa Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task<bool> ProcessMpesaPayment(string phoneNumber, decimal amount)
        {
            try
            {
                // Get access token
                string accessToken = await GetAccessToken();
                if (string.IsNullOrEmpty(accessToken))
                {
                    return false;
                }

                // Initiate STK Push
                bool result = await InitiateStkPush(accessToken, phoneNumber, amount);
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"M-Pesa processing error: {ex.Message}");
                return false;
            }
        }

        private async Task<string> GetAccessToken()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // M-Pesa API credentials (replace with your actual credentials)
                    string consumerKey = "uAaaoAHSM17pYTDmtAGWRjTywrdLcAKdLutsnKtfrm6K9G3m";
                    string consumerSecret = "RXsS3wytRjAhRAkFiqmF2CyYqkU99295153Nxa7gLxCY10gQenrLtvmtRRsIywJa";

                    string credentials = Convert.ToBase64String(
                        Encoding.UTF8.GetBytes($"{consumerKey}:{consumerSecret}"));

                    client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);

                    string url = "https://sandbox.safaricom.co.ke/oauth/v1/generate?grant_type=client_credentials";

                    HttpResponseMessage response = await client.GetAsync(url);
                    string responseContent = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        dynamic tokenResponse = JsonConvert.DeserializeObject(responseContent);
                        return tokenResponse.access_token;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"Failed to get access token: {responseContent}");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting access token: {ex.Message}");
                return null;
            }
        }

        private async Task<bool> InitiateStkPush(string accessToken, string phoneNumber, decimal amount)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                    // M-Pesa API configuration (replace with your actual values)
                    string businessShortCode = "174379";
                    string passkey = "YOUR_PASSKEY";
                    string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                    string password = Convert.ToBase64String(
                        Encoding.UTF8.GetBytes($"{businessShortCode}{passkey}{timestamp}"));

                    var payload = new
                    {
                        BusinessShortCode = businessShortCode,
                        Password = password,
                        Timestamp = timestamp,
                        TransactionType = "CustomerPayBillOnline",
                        Amount = amount.ToString("F0"),
                        PartyA = phoneNumber,
                        PartyB = businessShortCode,
                        PhoneNumber = phoneNumber,
                        CallBackURL = "https://your-callback-url.com/callback",
                        AccountReference = $"EduSync-{txtStudentID.Text}",
                        TransactionDesc = $"Fee payment for student {txtStudentID.Text}"
                    };

                    string jsonPayload = JsonConvert.SerializeObject(payload);
                    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                    string url = "https://sandbox.safaricom.co.ke/mpesa/stkpush/v1/processrequest";

                    HttpResponseMessage response = await client.PostAsync(url, content);
                    string responseContent = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        dynamic stkResponse = JsonConvert.DeserializeObject(responseContent);
                        return stkResponse.ResponseCode == "0";
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"STK Push failed: {responseContent}");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initiating STK Push: {ex.Message}");
                return false;
            }
        }

        private bool IsValidPhoneNumber(string phoneNumber)
        {
            // Validate Kenyan phone number format (254XXXXXXXXX)
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return false;

            // Remove any spaces or special characters
            phoneNumber = Regex.Replace(phoneNumber, @"[^\d]", "");

            // Check if it starts with 254 and has correct length
            return phoneNumber.StartsWith("254") && phoneNumber.Length == 12;
        }

        // Export functionality
        private void ExportDataGridViewToCSV(DataGridView dgv, string fileName)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                // Add headers
                for (int col = 0; col < dgv.Columns.Count; col++)
                {
                    sb.Append(dgv.Columns[col].HeaderText);
                    if (col < dgv.Columns.Count - 1)
                        sb.Append(",");
                }
                sb.AppendLine();

                // Add data rows
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        for (int col = 0; col < dgv.Columns.Count; col++)
                        {
                            string cellValue = row.Cells[col].Value?.ToString() ?? "";
                            // Escape commas and quotes in CSV
                            if (cellValue.Contains(",") || cellValue.Contains("\""))
                            {
                                cellValue = "\"" + cellValue.Replace("\"", "\"\"") + "\"";
                            }
                            sb.Append(cellValue);
                            if (col < dgv.Columns.Count - 1)
                                sb.Append(",");
                        }
                        sb.AppendLine();
                    }
                }

                File.WriteAllText(fileName, sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception($"Error exporting to CSV: {ex.Message}");
            }
        }

        // Event handlers for combo box changes
        private void txtAmountPaid_TextChanged(object sender, EventArgs e)
        {
            CalculateBalance();
        }

        private void txtTotalfees_TextChanged(object sender, EventArgs e)
        {
            CalculateBalance();
        }

        private void CalculateBalance()
        {
            try
            {
                if (decimal.TryParse(txtTotalfees.Text.Trim(), out decimal totalFee) &&
                    decimal.TryParse(txtAmountPaid.Text.Trim(), out decimal amountPaid))
                {
                    decimal balance = totalFee - amountPaid;
                    txtBalance.Text = balance.ToString("N2");

                    // Color code the balance
                    if (balance > 0)
                        txtBalance.ForeColor = Color.Red;
                    else if (balance < 0)
                        txtBalance.ForeColor = Color.Green;
                    else
                        txtBalance.ForeColor = Color.Black;
                }
                else
                {
                    txtBalance.Text = "";
                }
            }
            catch
            {
                txtBalance.Text = "";
            }
        }

        // Additional utility methods
        private void RefreshData()
        {
            LoadFeeRecords();
            txtTotalPaid.Text = "0.00";
            txtTotalBalance.Text = "0.00";
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshData();
        }

        // Handle form closing
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                DialogResult result = MessageBox.Show("Are you sure you want to close the Fee Manager?",
                                                    "Confirm Close",
                                                    MessageBoxButtons.YesNo,
                                                    MessageBoxIcon.Question);

                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }

            base.OnFormClosing(e);
        }
        private DataTable GetClassFeeDataFixed(string input)
        {
        
            DataTable dt = new DataTable();
            string connectionString = "Data Source=DESKTOP-TUS7DBQ;Initial Catalog=EduSync;Integrated Security=True;Encrypt=False";

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    System.Diagnostics.Debug.WriteLine($"DEBUG: Original input: '{input}'");

                    input = input?.Trim() ?? "";
                    System.Diagnostics.Debug.WriteLine($"DEBUG: Trimmed input: '{input}'");

                    // Use LEFT JOIN to include students without fee records

                    string query = @"
SELECT
    s.StudentId,
    s.FirstName,
    s.LastName,
    s.Admission,
    f.DueDate,
    f.Status,
    f.Description,
    s.Class,
    s.ParentContact,
    s.ParentName,
    ISNULL(f.FeeId, 0) AS FeeId,
    ISNULL(f.FeeType, '') AS FeeType,
    ISNULL(f.TotalFee, 0) AS TotalFee,
    ISNULL(f.AmountPaid, 0) AS AmountPaid,
    ISNULL(f.Balance, 0) AS Balance,
    ISNULL(f.PaymentMethod, '') AS PaymentMethod,
    ISNULL(f.Term, '') AS Term,
    ISNULL(f.Year, '') AS Year,
    f.DatePaid
FROM StdTable s
LEFT JOIN FeeTbl f ON s.StudentId = f.StudentId
WHERE s.Class LIKE @ClassNameLike
ORDER BY s.Class, s.FirstName, s.LastName;
";


                    SqlCommand cmd = new SqlCommand(query, con);

                    // Always use LIKE with trimmed input
                    cmd.Parameters.AddWithValue("@ClassNameLike", $"%{input}%");

                    System.Diagnostics.Debug.WriteLine($"DEBUG: @ClassNameLike='{cmd.Parameters["@ClassNameLike"].Value}'");

                    con.Open();

                    // Optional: Debug all available classes
                    using (SqlCommand debugCmd = new SqlCommand("SELECT DISTINCT s.Class FROM StdTable s ORDER BY s.Class", con))
                    {
                        using (SqlDataReader reader = debugCmd.ExecuteReader())
                        {
                            System.Diagnostics.Debug.WriteLine("DEBUG: Available classes in StdTable:");
                            while (reader.Read())
                            {
                                System.Diagnostics.Debug.WriteLine($"DEBUG: - '{reader["Class"]}'");
                            }
                        }
                    }

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    System.Diagnostics.Debug.WriteLine($"DEBUG: Query returned {dt.Rows.Count} rows");

                    // Optional: show sample rows for debugging
                    foreach (DataRow row in dt.Rows)
                    {
                        System.Diagnostics.Debug.WriteLine($"DEBUG ROW: StudentId={row["StudentId"]}, Class={row["Class"]}, FeeId={row["FeeId"]}, AmountPaid={row["AmountPaid"]}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR: {ex.Message}");
                throw;
            }

            return dt;
        

        }
        
        
        

        

        private void btnGenerateClassReport_Click(object sender, EventArgs e)
        {


        
            try
            {
                System.Diagnostics.Debug.WriteLine("DEBUG: Starting class report generation...");

                // 1. Get the class name from ComboBox
                if (ClassCombo.SelectedItem == null)
                {
                    System.Diagnostics.Debug.WriteLine("DEBUG: No class selected in combo box");
                    MessageBox.Show("Please select a class first.", "Warning",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // DEBUG: Check ComboBox properties
                System.Diagnostics.Debug.WriteLine($"DEBUG: ClassCombo.SelectedItem: {ClassCombo.SelectedItem}");
                System.Diagnostics.Debug.WriteLine($"DEBUG: ClassCombo.SelectedValue: {ClassCombo.SelectedValue}");
                System.Diagnostics.Debug.WriteLine($"DEBUG: ClassCombo.Text: '{ClassCombo.Text}'");
                System.Diagnostics.Debug.WriteLine($"DEBUG: ClassCombo.SelectedIndex: {ClassCombo.SelectedIndex}");

                string selectedClassName = ClassCombo.SelectedValue != null
                    ? ClassCombo.SelectedValue.ToString()
                    : ClassCombo.Text;

                // Additional null/empty check
                if (string.IsNullOrWhiteSpace(selectedClassName))
                {
                    System.Diagnostics.Debug.WriteLine("DEBUG: Selected class name is null or empty");
                    MessageBox.Show("Please select a valid class.", "Warning",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                selectedClassName = selectedClassName.Trim();
                System.Diagnostics.Debug.WriteLine($"DEBUG: Final selected class name: '{selectedClassName}'");

                // 2. Get data from your existing database method
                System.Diagnostics.Debug.WriteLine("DEBUG: Calling GetClassFeeData...");
                DataTable classData = GetClassFeeDataFixed(selectedClassName);

                System.Diagnostics.Debug.WriteLine($"DEBUG: GetClassFeeData returned {classData.Rows.Count} rows");

                // DEBUG: Show column info
                if (classData.Columns.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine("DEBUG: DataTable columns:");
                    foreach (DataColumn col in classData.Columns)
                    {
                        System.Diagnostics.Debug.WriteLine($"DEBUG: - {col.ColumnName} ({col.DataType})");
                    }
                }

                // DEBUG: Show sample data if exists
                if (classData.Rows.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine("DEBUG: Sample data from first row:");
                    var firstRow = classData.Rows[0];
                    foreach (DataColumn col in classData.Columns)
                    {
                        var value = firstRow[col.ColumnName];
                        System.Diagnostics.Debug.WriteLine($"DEBUG: {col.ColumnName}: '{value}' (Type: {value?.GetType()})");
                    }
                }

                if (classData.Rows.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine($"DEBUG: No fee records found for '{selectedClassName}'");

                    // DEBUG: Let's check if the ComboBox has the right format
                    System.Diagnostics.Debug.WriteLine("DEBUG: Checking ComboBox data source...");
                    if (ClassCombo.DataSource != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"DEBUG: ComboBox DataSource type: {ClassCombo.DataSource.GetType()}");
                        if (ClassCombo.DataSource is DataTable dt)
                        {
                            System.Diagnostics.Debug.WriteLine($"DEBUG: DataTable has {dt.Rows.Count} rows");
                            System.Diagnostics.Debug.WriteLine($"DEBUG: DisplayMember: '{ClassCombo.DisplayMember}'");
                            System.Diagnostics.Debug.WriteLine($"DEBUG: ValueMember: '{ClassCombo.ValueMember}'");

                            // Show first few items in ComboBox data source
                            for (int i = 0; i < Math.Min(5, dt.Rows.Count); i++)
                            {
                                var row = dt.Rows[i];
                                System.Diagnostics.Debug.WriteLine($"DEBUG: ComboBox Row {i}:");
                                foreach (DataColumn col in dt.Columns)
                                {
                                    System.Diagnostics.Debug.WriteLine($"DEBUG:   {col.ColumnName}: '{row[col.ColumnName]}'");
                                }
                            }
                        }
                    }

                    MessageBox.Show($"No fee records found for {selectedClassName}", "Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                System.Diagnostics.Debug.WriteLine("DEBUG: Converting DataTable to class objects...");

                // 3. Convert DataTable to ClassFeeReports objects
                ClassFeeReports.Class classObject = ConvertDataTableToClass(classData, selectedClassName);
                System.Diagnostics.Debug.WriteLine($"DEBUG: Class object created for: {classObject?.ClassName ?? "NULL"}");

                List<ClassFeeReports.Fee> fees = ConvertDataTableToFees(classData);
                System.Diagnostics.Debug.WriteLine($"DEBUG: Converted {fees?.Count ?? 0} fee records");

                // 4. Generate report using the ClassFeeReports system
                System.Diagnostics.Debug.WriteLine("DEBUG: Generating class report...");
                ClassFeeReport report = reportGenerator.GenerateClassReport(classObject, fees);
                System.Diagnostics.Debug.WriteLine($"DEBUG: Report generated - Students: {report.TotalStudents}, Amount: {report.TotalClassAmount}");

                // 5. Generate text and CSV reports
                System.Diagnostics.Debug.WriteLine("DEBUG: Generating text and CSV reports...");
                string textReport = reportGenerator.GenerateTextReport(report);
                string csvReport = reportGenerator.GenerateCSVReport(report);

                System.Diagnostics.Debug.WriteLine($"DEBUG: Text report length: {textReport?.Length ?? 0} characters");
                System.Diagnostics.Debug.WriteLine($"DEBUG: CSV report length: {csvReport?.Length ?? 0} characters");

                // 6. Define file paths with timestamp
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

                // Create safe filename from class name (remove invalid characters)
                string safeClassName = string.Join("_", selectedClassName.Split(Path.GetInvalidFileNameChars()));

                string textFilePath = $"Reports/{safeClassName}_FeeReport_{timestamp}.txt";
                string csvFilePath = $"Reports/{safeClassName}_FeeReport_{timestamp}.csv";

                System.Diagnostics.Debug.WriteLine($"DEBUG: Text file path: {textFilePath}");
                System.Diagnostics.Debug.WriteLine($"DEBUG: CSV file path: {csvFilePath}");

                // 7. Ensure Reports directory exists
                string reportsDir = "Reports";
                if (!Directory.Exists(reportsDir))
                {
                    System.Diagnostics.Debug.WriteLine("DEBUG: Creating Reports directory...");
                    Directory.CreateDirectory(reportsDir);
                }

                // Save reports to files
                System.Diagnostics.Debug.WriteLine("DEBUG: Saving reports to files...");
                reportGenerator.SaveReportToFile(textReport, textFilePath);
                reportGenerator.SaveReportToFile(csvReport, csvFilePath);
                System.Diagnostics.Debug.WriteLine("DEBUG: Files saved successfully");

                // 8. Notify user
                string successMessage = $"Fee report for {selectedClassName} has been generated successfully!\n\n" +
                                      $"Text Report: {textFilePath}\n" +
                                      $"CSV Report: {csvFilePath}\n\n" +
                                      $"Total Students: {report.TotalStudents}\n" +
                                      $"KshTotal Amount: ${report.TotalClassAmount:N2}\n" +
                                      $"KshTotal Outstanding: ${report.TotalClassOutstanding:N2}";

                System.Diagnostics.Debug.WriteLine($"DEBUG: Success message: {successMessage}");

                MessageBox.Show(successMessage, "Report Generated",
                               MessageBoxButtons.OK, MessageBoxIcon.Information);

                // 9. Optional: Display report in a new window
                System.Diagnostics.Debug.WriteLine("DEBUG: Displaying report in text box...");
                DisplayReportInTextBox(textReport);

                System.Diagnostics.Debug.WriteLine("DEBUG: Class report generation completed successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR: Exception in btnGenerateClassReport_Click:");
                System.Diagnostics.Debug.WriteLine($"ERROR: Message: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"ERROR: Stack trace: {ex.StackTrace}");

                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"ERROR: Inner exception: {ex.InnerException.Message}");
                }

                MessageBox.Show("Error generating report: " + ex.Message + "\n\nDetails: " + ex.StackTrace,
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Helper method to check ComboBox setup (call this during form load to debug ComboBox issues)
        private void DebugComboBoxSetup()
        {
            System.Diagnostics.Debug.WriteLine("=== DEBUGGING COMBOBOX SETUP ===");
            System.Diagnostics.Debug.WriteLine($"ClassCombo.DataSource: {ClassCombo.DataSource?.GetType()}");
            System.Diagnostics.Debug.WriteLine($"ClassCombo.DisplayMember: '{ClassCombo.DisplayMember}'");
            System.Diagnostics.Debug.WriteLine($"ClassCombo.ValueMember: '{ClassCombo.ValueMember}'");
            System.Diagnostics.Debug.WriteLine($"ClassCombo.Items.Count: {ClassCombo.Items.Count}");

            if (ClassCombo.DataSource is DataTable dt)
            {
                System.Diagnostics.Debug.WriteLine($"DataTable rows: {dt.Rows.Count}");
                System.Diagnostics.Debug.WriteLine("Available columns:");
                foreach (DataColumn col in dt.Columns)
                {
                    System.Diagnostics.Debug.WriteLine($"  - {col.ColumnName}");
                }

                System.Diagnostics.Debug.WriteLine("Sample data:");
                for (int i = 0; i < Math.Min(3, dt.Rows.Count); i++)
                {
                    var row = dt.Rows[i];
                    foreach (DataColumn col in dt.Columns)
                    {
                        System.Diagnostics.Debug.WriteLine($"  Row {i} {col.ColumnName}: '{row[col.ColumnName]}'");
                    }
                }
            }
            System.Diagnostics.Debug.WriteLine("=== END COMBOBOX DEBUG ===");
        }
        

        

        // Convert DataTable to ClassFeeReports.Class object
        private ClassFeeReports.Class ConvertDataTableToClass(DataTable dt, string className)
        {
        
            var classObject = new ClassFeeReports.Class
            {
                ClassId = 1, // You can fetch from DB if needed
                ClassName = className.Replace("Class ", "Grade "), // Adjust naming
                Section = ExtractSectionFromClassName(className),
                AcademicYear = DateTime.Now.Year,
                Students = new List<ClassFeeReports.Student>()
            };

            // Use dictionary to avoid duplicate students
            var uniqueStudents = new Dictionary<int, ClassFeeReports.Student>();

            foreach (DataRow row in dt.Rows)
            {
                int studentId = Convert.ToInt32(row["StudentId"]);

                if (!uniqueStudents.ContainsKey(studentId))
                {
                    var student = new ClassFeeReports.Student
                    {
                        StudentId = studentId,
                        FirstName = row.Table.Columns.Contains("FirstName") && row["FirstName"] != DBNull.Value
                                    ? row["FirstName"].ToString()
                                    : string.Empty,
                        LastName = row.Table.Columns.Contains("LastName") && row["LastName"] != DBNull.Value
                                   ? row["LastName"].ToString()
                                   : string.Empty,
                        Email = string.Empty, // No Email column in your DB
                        EnrollmentDate = DateTime.MinValue // Default value since column is missing
                    };

                    uniqueStudents[studentId] = student;
                    classObject.Students.Add(student);
                }
            }

            return classObject;
        }





        // Convert DataTable to List of ClassFeeReports.Fee objects
        private List<ClassFeeReports.Fee> ConvertDataTableToFees(DataTable dt)
        {


           

                var fees = new List<ClassFeeReports.Fee>();

                foreach (DataRow row in dt.Rows)
                {
                    var fee = new ClassFeeReports.Fee
                    {
                        FeeId = row.Table.Rows.IndexOf(row) + 1000, // Generate unique ID
                        StudentId = row.Table.Columns.Contains("StudentId") && row["StudentId"] != DBNull.Value
                                    ? Convert.ToInt32(row["StudentId"])
                                    : 0,
                        ClassId = 1, // Adjust if you fetch from DB
                        FeeType = row.Table.Columns.Contains("FeeType") && row["FeeType"] != DBNull.Value
                                  ? ParseFeeType(row["FeeType"].ToString())
                                  : ClassFeeReports.FeeType.Miscellaneous,

                        Amount = row["TotalFee"] != DBNull.Value
                    ? Convert.ToDecimal(row["TotalFee"])
                    : 0m,
                        AmountPaid = row.Table.Columns.Contains("AmountPaid") && row["AmountPaid"] != DBNull.Value
                                     ? Convert.ToDecimal(row["AmountPaid"])
                                     : 0m,
                        DueDate = row.Table.Columns.Contains("DueDate") && row["DueDate"] != DBNull.Value
                                  ? Convert.ToDateTime(row["DueDate"])
                                  : (DateTime?)null, // nullable
                        Status = row.Table.Columns.Contains("Status") && row["Status"] != DBNull.Value
                                 ? ParsePaymentStatus(row["Status"].ToString())
                                 : ClassFeeReports.PaymentStatus.Pending,
                        Description = row.Table.Columns.Contains("Description") && row["Description"] != DBNull.Value
                                      ? row["Description"].ToString()
                                      : string.Empty,
                        PaymentDate = GetPaymentDate(row) // keep existing logic
                    };

                    fees.Add(fee);
                }

                return fees;


            
        }





        // Helper method to extract section from class name (e.g., "Class 8A" -> "A")
        private string ExtractSectionFromClassName(string className)
        {
            // Extract the last character if it's a letter
            if (!string.IsNullOrEmpty(className) && char.IsLetter(className.Last()))
            {
                return className.Last().ToString();
            }
            return "A"; // Default section
        }

        // Helper method to parse FeeType from string to ClassFeeReports.FeeType
        private ClassFeeReports.FeeType ParseFeeType(string feeTypeString)
        {
            // Try direct parsing first
            if (Enum.TryParse<ClassFeeReports.FeeType>(feeTypeString, true, out ClassFeeReports.FeeType result))
                return result;

            // Handle common variations and mappings
            switch (feeTypeString.ToLower().Trim())
            {
                case "tuition":
                case "school fee":
                case "academic fee":
                    return ClassFeeReports.FeeType.Tuition;
                case "lab":
                case "laboratory":
                case "lab fee":
                    return ClassFeeReports.FeeType.Laboratory;
                case "library":
                case "library fee":
                    return ClassFeeReports.FeeType.Library;
                case "sports":
                case "sports fee":
                case "athletics":
                    return ClassFeeReports.FeeType.Sports;
                case "transport":
                case "transportation":
                case "bus fee":
                    return ClassFeeReports.FeeType.Transport;
                case "misc":
                case "miscellaneous":
                case "other":
                default:
                    return ClassFeeReports.FeeType.Miscellaneous;
            }
        }

        // Helper method to parse PaymentStatus from string to ClassFeeReports.PaymentStatus
        private ClassFeeReports.PaymentStatus ParsePaymentStatus(string statusString)
        {
            // Try direct parsing first
            if (Enum.TryParse<ClassFeeReports.PaymentStatus>(statusString, true, out ClassFeeReports.PaymentStatus result))
                return result;

            // Handle common variations and mappings
            switch (statusString.ToLower().Trim())
            {
                case "paid":
                case "complete":
                case "completed":
                    return ClassFeeReports.PaymentStatus.Paid;
                case "pending":
                case "unpaid":
                case "due":
                    return ClassFeeReports.PaymentStatus.Pending;
                case "overdue":
                case "late":
                case "expired":
                    return ClassFeeReports.PaymentStatus.Overdue;
                case "partial":
                case "partiallypaid":
                case "partially paid":
                    return ClassFeeReports.PaymentStatus.PartiallyPaid;
                default:
                    return ClassFeeReports.PaymentStatus.Pending;
            }
        }

        // Helper method to get payment date from DataRow
        private DateTime? GetPaymentDate(DataRow row)
        {
            // If you have a PaymentDate column in your database
            if (row.Table.Columns.Contains("PaymentDate") && row["PaymentDate"] != DBNull.Value)
            {
                return Convert.ToDateTime(row["PaymentDate"]);
            }

            // Otherwise, if amount is paid, assume payment was recent
            if (Convert.ToDecimal(row["AmountPaid"]) > 0)
            {
                return DateTime.Now; // Or you could use a more appropriate date
            }

            return null;
        }

        // Display report in a new window
        private void DisplayReportInTextBox(string report)
        {
            var reportForm = new Form
            {
                Text = "Fee Report - " + DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
                Size = new Size(900, 700),
                StartPosition = FormStartPosition.CenterParent,
                MinimizeBox = true,
                MaximizeBox = true
            };

            var textBox = new System.Windows.Forms.TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Both,
                Dock = DockStyle.Fill,
                Font = new System.Drawing.Font("Consolas", 9),
                ReadOnly = true,
                Text = report,
                BackColor = Color.White,
                ForeColor = Color.Black
            };

            // Add a panel for buttons
            var buttonPanel = new Panel
            {
                Height = 50,
                Dock = DockStyle.Bottom,
                BackColor = SystemColors.Control
            };

            var btnClose = new System.Windows.Forms.Button
            {
                Text = "Close",
                Size = new Size(75, 30),
                Location = new Point(10, 10),
                DialogResult = DialogResult.OK
            };

            var btnPrint = new System.Windows.Forms.Button
            {
                Text = "Print",
                Size = new Size(75, 30),
                Location = new Point(95, 10)
            };

            // Print functionality (optional)
            btnPrint.Click += (s, e) => {
                try
                {
                    var printDialog = new PrintDialog();
                    if (printDialog.ShowDialog() == DialogResult.OK)
                    {
                        // Simple print implementation
                        MessageBox.Show("Print functionality would be implemented here.", "Print",
                                      MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Print error: " + ex.Message, "Error",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            buttonPanel.Controls.AddRange(new Control[] { btnClose, btnPrint });
            reportForm.Controls.AddRange(new Control[] { textBox, buttonPanel });

            reportForm.ShowDialog();
        }
        private DataTable GetSchoolFeeSummary()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-TUS7DBQ;Initial Catalog=EduSync;Integrated Security=True;Encrypt=False;"))
            {
                string query = @"
            SELECT 
                COUNT(DISTINCT StudentID) AS TotalStudents,
                SUM(AmountPaid + Balance) AS TotalBilled,
                SUM(AmountPaid) AS TotalPaid,
                SUM(Balance) AS TotalBalance
            FROM FeeTbl";

                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }
            return dt;
        }
        
private void ExportSchoolFeeSummaryToPDF(DataTable dt)
    {
        
            try
            {
                string filePath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    "SchoolFeeSummary.pdf"
                );

                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    Document doc = new Document(PageSize.A4, 50, 50, 50, 50);
                    PdfWriter writer = PdfWriter.GetInstance(doc, fs);

                    doc.Open();

                    // Title
                    Paragraph title = new Paragraph("Mandela School Fee Summary Report\n\n",
                        FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16));
                    title.Alignment = Element.ALIGN_CENTER;
                    doc.Add(title);

                    // Add some spacing
                    doc.Add(new Paragraph("\n"));

                    // Table
                    PdfPTable table = new PdfPTable(4);
                    table.WidthPercentage = 100;
                    table.SpacingBefore = 10f;
                    table.SpacingAfter = 10f;

                    // Headers
                    string[] headers = { "Total Students", "Total Billed (Ksh)", "Total Paid (Ksh)", "Balance (Ksh)" };
                    foreach (string h in headers)
                    {
                        PdfPCell cell = new PdfPCell(new Phrase(h,
                            FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12)));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.BackgroundColor = new BaseColor(220, 220, 220); // Light gray
                        cell.Padding = 8;
                        table.AddCell(cell);
                    }

                    // Data row
                    if (dt.Rows.Count > 0)
                    {
                        DataRow row = dt.Rows[0];

                        // Total Students
                        PdfPCell cell1 = new PdfPCell(new Phrase(row["TotalStudents"].ToString()));
                        cell1.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell1.Padding = 8;
                        table.AddCell(cell1);

                        // Total Billed
                        PdfPCell cell2 = new PdfPCell(new Phrase(
                            Convert.ToDecimal(row["TotalBilled"]).ToString("N2")));
                        cell2.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell2.Padding = 8;
                        table.AddCell(cell2);

                        // Total Paid
                        PdfPCell cell3 = new PdfPCell(new Phrase(
                            Convert.ToDecimal(row["TotalPaid"]).ToString("N2")));
                        cell3.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell3.Padding = 8;
                        table.AddCell(cell3);

                        // Balance
                        PdfPCell cell4 = new PdfPCell(new Phrase(
                            Convert.ToDecimal(row["TotalBalance"]).ToString("N2")));
                        cell4.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell4.Padding = 8;

                        // Color code balance (red if negative, green if positive)
                        decimal balance = Convert.ToDecimal(row["TotalBalance"]);
                        if (balance < 0)
                            cell4.BackgroundColor = new BaseColor(255, 200, 200); // Light red
                        else if (balance > 0)
                            cell4.BackgroundColor = new BaseColor(200, 255, 200); // Light green

                        table.AddCell(cell4);
                    }

                    doc.Add(table);

                    // Footer
                    Paragraph footer = new Paragraph(
                        "\nGenerated on: " + DateTime.Now.ToString("dd MMM yyyy HH:mm") +
                        "\nGenerated by: Mandela School Management System",
                        FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 10));
                    footer.Alignment = Element.ALIGN_CENTER;
                    doc.Add(footer);

                    doc.Close();
                }

                MessageBox.Show($"PDF report generated successfully!\nSaved to: {filePath}",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating PDF: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    

    private void btnSchoolSummary_Click(object sender, EventArgs e)
        {

        
            try
            {
                // Show loading cursor
                this.Cursor = Cursors.WaitCursor;

                // Get data
                DataTable dt = GetSchoolFeeSummary();

                // Check if we have data
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("No fee data found to generate report.",
                        "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Generate PDF
                ExportSchoolFeeSummaryToPDF(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Restore cursor
                this.Cursor = Cursors.Default;
            }
        }
    }
    
}