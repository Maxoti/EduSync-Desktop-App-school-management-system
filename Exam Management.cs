using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace EduSync
{
    public partial class Exam_Management : Form
    {
        private List<StudentComboItem> allStudentItems = new List<StudentComboItem>();

        #region Fields and Properties

        // Database connection string - update this according to your database
        private string connectionString = "Data Source=DESKTOP-TUS7DBQ;Initial Catalog=EduSync;Integrated Security=True;Encrypt=False";

        // Print components
        private PrintDocument printDocument;
        private PrintPreviewDialog printPreviewDialog;

        #endregion

        #region Constructor and Initialization

        public Exam_Management()
        {
            InitializeComponent();
            InitializePrintComponents();
            InitializeForm();
            SetupEventHandlers();
            examDGV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvMarks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvRanking.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;


            cmbExamType.Items.AddRange(new string[] { "Opener", "MidTerm", "EndTerm" });
            cmbExamType.SelectedIndex = 0;

          
            LoadStudentsData();  // 👈 use this, not LoadAllFirstNames()

            FirstNameComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        }
        private DataTable GetAllStudents(string classFilter = null)
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString)) // replace with your connection string
                {
                    conn.Open();

                    string query = @"
                SELECT 
                    s.StudentId,
                    s.FirstName + ' ' + s.LastName AS FullName,
                    s.Class,
                    s.ParentName,
                    s.ParentContact,
                    s.Admission
                FROM StdTable s
                WHERE (@ClassFilter IS NULL OR s.Class LIKE @ClassFilter)
                ORDER BY s.Class, s.FirstName, s.LastName
            ";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (string.IsNullOrEmpty(classFilter))
                            cmd.Parameters.AddWithValue("@ClassFilter", DBNull.Value);
                        else
                            cmd.Parameters.AddWithValue("@ClassFilter", classFilter + "%");

                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching students: " + ex.Message);
            }

            return dt;
        }


        private void InitializePrintComponents()
        {
            ButtonStyler.ApplyPrimaryStyle(btnclear);
            ButtonStyler.ApplyPrimaryStyle(btnSearch);
            ButtonStyler.ApplyPrimaryStyle(printbtn);
            ButtonStyler.ApplyPrimaryStyle(btnLoadMarks);
            ButtonStyler.ApplyPrimaryStyle(btnSave);
            ButtonStyler.ApplyPrimaryStyle(btnGen);
            ButtonStyler.ApplyPrimaryStyle(btnRanking);
            ButtonStyler.ApplyPrimaryStyle(btnToExcel);

            printDocument = new PrintDocument();
            printDocument.PrintPage += PrintDocument_PrintPage;

            printPreviewDialog = new PrintPreviewDialog();
            printPreviewDialog.Document = printDocument;

            // Ensure proper cleanup when form is disposed
            this.FormClosed += (s, e) => CleanupPrintComponents();
        }

        private void InitializeForm()
        {
            // Initialize combo boxes with data
            InitializeComboBoxes();

            // Set up DataGridView columns for marks entry
            SetupMarksDataGridView();
        }

        private void SetupEventHandlers()
        {
            dgvMarks.CellValueChanged += DgvMarks_CellValueChanged;
            dgvMarks.EditingControlShowing += DgvMarks_EditingControlShowing;

            // Add event handler for class selection change
            cmbClass.SelectedIndexChanged += CmbClass_SelectedIndexChanged;
        }

        private void InitializeComboBoxes()
        {
            // Initialize Class combo boxes with school levels
            string[] classes = {
                "PRE-PRIMARY", "LOWER PRIMARY", "UPPER PRIMARY", "JUNIOR SECONDARY",
                "PLAYGROUP", "PP1", "PP2", "GRADE 1", "GRADE 2", "GRADE 3",
                "GRADE 4", "GRADE 5", "GRADE 6", "GRADE 7", "GRADE 8", "GRADE 9"
            };

            cmbClass.Items.AddRange(classes);
            comboClass.Items.AddRange(classes);

            // Initialize Term combo boxes
            string[] terms = { "TERM 1", "TERM 2", "TERM 3" };
            cmbTerm.Items.AddRange(terms);
            comboTerm.Items.AddRange(terms);

            // Set current year
            txtYear.Text = DateTime.Now.Year.ToString();
            textYear.Text = DateTime.Now.Year.ToString();
        }

        private void SetupMarksDataGridView()
        {
            // Clear existing columns
            dgvMarks.Columns.Clear();

            // Add columns for marks entry
            dgvMarks.Columns.Add("LearningArea", "Learning Area");
            dgvMarks.Columns.Add("MarksObtained", "Marks Obtained");
            dgvMarks.Columns.Add("TotalMarks", "Total Marks");

            // Add Performance Level ComboBox column (renamed from Grade)
            DataGridViewComboBoxColumn performanceLevelColumn = new DataGridViewComboBoxColumn();
            performanceLevelColumn.Name = "PerformanceLevel";
            performanceLevelColumn.HeaderText = "Performance Level";
            performanceLevelColumn.DataPropertyName = "PerformanceLevel";

            // Add CBC rubric performance levels
            performanceLevelColumn.Items.AddRange(new string[] {
                "",        // Empty option
                "EE1",     // Exceeds Expectations Level 1
                "EE2",     // Exceeds Expectations Level 2  
                "ME1",     // Meets Expectations Level 1
                "ME2",     // Meets Expectations Level 2
                "AE1",     // Approaches Expectations Level 1
                "AE2",     // Approaches Expectations Level 2
                "BE1",     // Below Expectations Level 1
                "BE2"      // Below Expectations Level 2
            });

            performanceLevelColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton;
            performanceLevelColumn.FlatStyle = FlatStyle.Flat;
            dgvMarks.Columns.Add(performanceLevelColumn);

            // Set column widths
            dgvMarks.Columns["LearningArea"].Width = 250;
            dgvMarks.Columns["MarksObtained"].Width = 120;
            dgvMarks.Columns["TotalMarks"].Width = 120;
            dgvMarks.Columns["PerformanceLevel"].Width = 120;

            // Make LearningArea read-only, others editable
            dgvMarks.Columns["LearningArea"].ReadOnly = true;

            // Configure DataGridView appearance
            dgvMarks.AllowUserToAddRows = false;
            dgvMarks.AllowUserToDeleteRows = false;
            dgvMarks.SelectionMode = DataGridViewSelectionMode.CellSelect;
        }

        #endregion

        #region Learning Areas Management

        private void CmbClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbClass.SelectedItem != null)
            {
                string selectedClass = cmbClass.SelectedItem.ToString();
                PopulateLearningAreasFromDatabase(selectedClass);
            }
        }

        private void PopulateLearningAreasFromDatabase(string studentClass)
        {
            try
            {
                // Clear existing rows
                dgvMarks.Rows.Clear();

                // Map class to level
                string level = MapClassToLevel(studentClass);

                // Fetch learning areas from database
                List<string> learningAreas = GetLearningAreasFromDatabase(level);

                // Add learning areas to the grid
                foreach (string area in learningAreas)
                {
                    dgvMarks.Rows.Add(area, "", "100", "");
                }

                // Add total row
                AddTotalRow();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading learning areas: " + ex.Message, "Database Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string MapClassToLevel(string studentClass)
        {
            // Map class names to database level names
            switch (studentClass.ToUpper())
            {
                case "PLAYGROUP":
                case "PP1":
                case "PP2":
                    return "ECDE";

                case "GRADE 1":
                case "GRADE 2":
                case "GRADE 3":
                case "GRADE 4":
                case "GRADE 5":
                case "GRADE 6":
                    return "UPPER PRIMARY";

                case "GRADE 7":
                case "GRADE 8":
                case "GRADE 9":
                    return "JUNIOR SECONDARY";

                default:
                    // Check if it's already a level name
                    if (studentClass.ToUpper().Contains("ECDE") || studentClass.ToUpper().Contains("PRE"))
                        return "ECDE";
                    else if (studentClass.ToUpper().Contains("PRIMARY"))
                        return "UPPER PRIMARY";
                    else if (studentClass.ToUpper().Contains("SECONDARY"))
                        return "JUNIOR SECONDARY";
                    else
                        return "UPPER PRIMARY"; // Default fallback
            }
        }

        private List<string> GetLearningAreasFromDatabase(string level)
        {
            List<string> learningAreas = new List<string>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = "SELECT AreaName FROM LearningAreaTbl WHERE Level = @Level ORDER BY AreaName";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Level", level);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                learningAreas.Add(reader["AreaName"].ToString());
                            }
                        }
                    }
                }

                // If no learning areas found in database, use fallback
                if (learningAreas.Count == 0)
                {
                    learningAreas = GetFallbackLearningAreas(level);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching learning areas from database: " + ex.Message,
                              "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                // Use fallback learning areas
                learningAreas = GetFallbackLearningAreas(level);
            }

            return learningAreas;
        }

        private List<string> GetFallbackLearningAreas(string level)
        {
            // Fallback learning areas if database fetch fails
            switch (level.ToUpper())
            {
                case "ECDE":
                    return new List<string>
                    {
                        "Language and Literacy",
    "Mathematics",
    "Physical and Psychomotor Development",
    "Social and Emotional Development",
    "Creative Arts",
    "Environment/Science",
    "Health and Nutrition"
                    };

                case "UPPER PRIMARY":
                    return new List<string>
                    {
                        "English Language",
                        "Kiswahili/Mother Tongue",
                        "Mathematics",
                        "Science and Technology",
                        "Social Studies",
                        "Religious Education",
                        "Creative Arts",

                    };

                case "JUNIOR SECONDARY":
                    return new List<string>
                    {
                        "English",
                        "Kiswahili",
                        "Mathematics",
                        "Integrated Science",
                        "Social Studies",
                        "Agriculture & Nutrition",
                        "Religious Education",
                        "Creative Arts",
                        "Pre-Technical Studies",
                    };

                default:
                    return new List<string>
                    {
                        "Mathematical Activities",
                        "Language Activities",
                        "Environmental Activities",
                        "Psychomotor & Creative Activities",
                        "Religious Education Activities"
                    };
            }
        }

        private void AddTotalRow()
        {
            // Add a separator row
            int totalRowIndex = dgvMarks.Rows.Add("TOTAL", "", "", "");

            // Style the total row
            DataGridViewRow totalRow = dgvMarks.Rows[totalRowIndex];
            totalRow.DefaultCellStyle.BackColor = Color.LightBlue;
            totalRow.DefaultCellStyle.Font = new Font(dgvMarks.Font, FontStyle.Bold);

            // Make total row cells read-only except for performance level
            totalRow.Cells["LearningArea"].ReadOnly = true;
            totalRow.Cells["MarksObtained"].ReadOnly = true;
            totalRow.Cells["TotalMarks"].ReadOnly = true;
            totalRow.Cells["PerformanceLevel"].ReadOnly = true;

            // Calculate totals
            UpdateTotalRow();
        }

        private void UpdateTotalRow()
        {
            int totalMarks = 0;
            int totalPossible = 0;
            int validSubjects = 0;

            // Find the total row (last row)
            DataGridViewRow totalRow = null;
            foreach (DataGridViewRow row in dgvMarks.Rows)
            {
                if (row.Cells["LearningArea"].Value?.ToString() == "TOTAL")
                {
                    totalRow = row;
                    break;
                }
            }

            if (totalRow == null) return;

            // Calculate totals from all non-total rows
            foreach (DataGridViewRow row in dgvMarks.Rows)
            {
                if (row.Cells["LearningArea"].Value?.ToString() != "TOTAL" && !row.IsNewRow)
                {
                    string marksObtainedStr = row.Cells["MarksObtained"].Value?.ToString();
                    string totalMarksStr = row.Cells["TotalMarks"].Value?.ToString();

                    if (int.TryParse(marksObtainedStr, out int marksObtained) &&
                        int.TryParse(totalMarksStr, out int totalMarksForSubject))
                    {
                        totalMarks += marksObtained;
                        totalPossible += totalMarksForSubject;
                        validSubjects++;
                    }
                }
            }

            // Update total row
            totalRow.Cells["MarksObtained"].Value = totalMarks.ToString();
            totalRow.Cells["TotalMarks"].Value = totalPossible.ToString();

            // Calculate overall performance level
            if (totalPossible > 0)
            {
                double percentage = (double)totalMarks / totalPossible * 100;
                totalRow.Cells["PerformanceLevel"].Value = GetCbcRubric((int)Math.Round(percentage));
            }
        }

        #endregion

        #region Event Handlers

        private void Exam_Management_Load(object sender, EventArgs e)
        {

            DataTable students = GetAllStudents(); // Fetch all students
            examDGV.DataSource = students;

            // Optional: hide columns you don’t want to show
            examDGV.Columns["StudentId"].Visible = false;
            examDGV.Columns["Admission"].Visible = false;

            // Optional: rename headers
            examDGV.Columns["FullName"].HeaderText = "Student Name";
            examDGV.Columns["ParentName"].HeaderText = "Parent/Guardian";
            examDGV.Columns["ParentContact"].HeaderText = "Contact";
        }



        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtStudentID.Text))
                {
                    MessageBox.Show("Please enter a Student ID to search.", "Search Error",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                SearchStudentInDatabase(txtStudentID.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during search: " + ex.Message, "Search Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnclear_Click(object sender, EventArgs e)
        {
            try
            {
                ClearFormFields();
                MessageBox.Show("Form cleared successfully.", "Clear",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error clearing form: " + ex.Message, "Clear Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void printbtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateStudentData())
                {
                    printPreviewDialog.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error occurred while printing: " + ex.Message, "Print Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateStudentData() && ValidateMarksData())
                {
                    SaveMarksToDatabase();
                    MessageBox.Show("Marks saved successfully!", "Save Successful",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving data: " + ex.Message, "Save Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLoadMarks_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateStudentData())
                {
                    LoadMarksFromDatabase();
                    MessageBox.Show("Marks loaded successfully!", "Load Successful",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading marks: " + ex.Message, "Load Marks Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGen_Click(object sender, EventArgs e)
        {

            try
            {
                // ✅ Step 1: Get form data
                string studentId = txtStudentID.Text.Trim();
                string term = cmbTerm.Text.Trim();
                string year = txtYear.Text.Trim();
                string className = cmbClass.Text.Trim();

                // ✅ Validation
                if (string.IsNullOrEmpty(studentId) || string.IsNullOrEmpty(term) ||
                    string.IsNullOrEmpty(year) || string.IsNullOrEmpty(className))
                {
                    MessageBox.Show("Please fill in all required fields: Student ID, Class, Term, and Year.");
                    return;
                }

                // ✅ Get student name
                string studentName = (FirstNameComboBox.Text.Trim() + " " + txtLastName.Text.Trim()).Trim();
                if (string.IsNullOrEmpty(studentName.Replace(" ", "")))
                {
                    MessageBox.Show("Please search for student first to get the student name.");
                    return;
                }

                // ✅ Step 2: Load data from DB
                string connectionString = "Data Source=DESKTOP-TUS7DBQ;Initial Catalog=EduSync;Integrated Security=True;Encrypt=False";
                DataTable dt = new DataTable();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string sql = @"
SELECT 
    @StudentID as StudentID,
    LearningArea,
    MAX(CASE WHEN ExamType = 'Opener' THEN MarksObtained END) AS Marks,
    100 AS TotalMarks
FROM StudentMarks
WHERE StudentID = @StudentID AND Term = @Term AND Year = @Year AND Class = @Class
GROUP BY LearningArea";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@StudentID", studentId);
                        cmd.Parameters.AddWithValue("@Term", term);
                        cmd.Parameters.AddWithValue("@Year", year);
                        cmd.Parameters.AddWithValue("@Class", className);

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);
                    }
                }

                // ✅ Calculate and add TOTAL row (NEW CODE)
                if (dt.Rows.Count > 0)
                {
                    int totalMarks = 0;
                    int totalPossibleMarks = dt.Rows.Count * 100;

                    // Sum all marks
                    foreach (DataRow row in dt.Rows)
                    {
                        if (row["Marks"] != DBNull.Value)
                        {
                            totalMarks += Convert.ToInt32(row["Marks"]);
                        }
                    }

                    // Add TOTAL row
                    DataRow totalRow = dt.NewRow();
                    totalRow["StudentID"] = studentId;
                    totalRow["LearningArea"] = "TOTAL";
                    totalRow["Marks"] = totalMarks;
                    totalRow["TotalMarks"] = totalPossibleMarks;
                    dt.Rows.Add(totalRow);
                }

                // ✅ Check data presence
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show($"No exam data found for Student ID: {studentId}, Class: {className}, Term: {term}, Year: {year}");
                    return;
                }

                // ✅ Set table name to match RDLC dataset
                dt.TableName = "MarksDataSet";

                // ✅ Step 3: Setup ReportViewer
                rptGen.LocalReport.DataSources.Clear();
                ReportDataSource rds = new ReportDataSource("MarksDataSet", dt);
                rptGen.LocalReport.DataSources.Add(rds);

                // ✅ Step 4: Set RDLC Path
                string reportPath = Path.Combine(Application.StartupPath, "Reports", "StudentReport.rdlc");
                if (!File.Exists(reportPath))
                {
                    // Try fallback path
                    reportPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "StudentReport.rdlc");
                    if (!File.Exists(reportPath))
                    {
                        MessageBox.Show($"Report file not found at: {reportPath}");
                        return;
                    }
                }

                rptGen.LocalReport.ReportPath = reportPath;

                // ✅ Step 5: Set Report Parameters
                ReportParameter[] reportParams = new ReportParameter[]
                {
        new ReportParameter("StudentName", studentName),
        new ReportParameter("Class", className),
        new ReportParameter("Term", term),
        new ReportParameter("Year", year)
                };
                rptGen.LocalReport.SetParameters(reportParams);

                // ✅ Step 6: Refresh Report
                rptGen.LocalReport.Refresh();
                rptGen.RefreshReport();

                MessageBox.Show("Report generated successfully");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Error generating report:\n{ex.Message}\n\nStack Trace:\n{ex.StackTrace}");
            }
        }

        // ✅ Helper method to add totals row to DataTable
        private void AddTotalsToDataTable(DataTable dt)
        {
            if (dt.Rows.Count == 0) return;

            int totalMarks = 0;
            int totalPossible = 0;

            // Calculate totals
            foreach (DataRow row in dt.Rows)
            {
                if (int.TryParse(row["Marks"].ToString(), out int marks))
                {
                    totalMarks += marks;
                }
                if (int.TryParse(row["TotalMarks"].ToString(), out int possible))
                {
                    totalPossible += possible;
                }
            }

            // Calculate overall percentage and performance level
            double overallPercentage = totalPossible > 0 ? (double)totalMarks / totalPossible * 100 : 0;
            string overallPerformanceLevel = GetCbcRubric((int)Math.Round(overallPercentage));

            // Add totals row
            DataRow totalRow = dt.NewRow();
            totalRow["RowNumber"] = dt.Rows.Count + 1;
            totalRow["LearningArea"] = "TOTAL";
            totalRow["Marks"] = totalMarks;
            totalRow["TotalMarks"] = totalPossible;
            totalRow["PerformanceLevel"] = overallPerformanceLevel;
            totalRow["Percentage"] = $"{overallPercentage:F1}%";

            dt.Rows.Add(totalRow);
        }

        // ✅ Alternative cleaner SQL query if you want to simplify further
        private DataTable GetCleanStudentMarks(string studentId, string term, string year, string className)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                // ✅ Enhanced query with percentage calculation
                string sql = @"
                SELECT 
                    LearningArea as Subject,
                    MarksObtained as Score,
                    100 as MaxScore,
                    PerformanceLevel as Grade,
                    CAST(ROUND((MarksObtained * 100.0 / 100), 2) AS DECIMAL(5,2)) as Percentage
                FROM StudentMarks
                WHERE StudentID = @StudentID 
                    AND Term = @Term 
                    AND Year = @Year 
                    AND Class = @Class
                    AND ExamType = 'Opener'
                ORDER BY LearningArea";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@StudentID", studentId);
                    cmd.Parameters.AddWithValue("@Term", term);
                    cmd.Parameters.AddWithValue("@Year", year);
                    cmd.Parameters.AddWithValue("@Class", className);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }

            // ✅ Add totals calculation
            if (dt.Rows.Count > 0)
            {
                AddTotalCalculations(dt);
            }

            return dt;
        }
        private void AddTotalCalculations(DataTable dt)
        {
            // Calculate totals
            int totalScore = dt.AsEnumerable().Sum(row => Convert.ToInt32(row["Score"]));
            int totalMaxScore = dt.Rows.Count * 100; // Since each subject has 100 max
            decimal overallPercentage = totalMaxScore > 0 ? Math.Round((decimal)(totalScore * 100.0 / totalMaxScore), 2) : 0;
            string overallGrade = GetGradeFromPercentage(overallPercentage);

            // Add total columns to DataTable
            dt.Columns.Add("TotalScore", typeof(int));
            dt.Columns.Add("TotalMaxScore", typeof(int));
            dt.Columns.Add("OverallPercentage", typeof(decimal));
            dt.Columns.Add("OverallGrade", typeof(string));
            dt.Columns.Add("SubjectCount", typeof(int));

            // Populate total values for each row
            foreach (DataRow row in dt.Rows)
            {
                row["TotalScore"] = totalScore;
                row["TotalMaxScore"] = totalMaxScore;
                row["OverallPercentage"] = overallPercentage;
                row["OverallGrade"] = overallGrade;
                row["SubjectCount"] = dt.Rows.Count;
            }
        }
        private string GetGradeFromPercentage(decimal percentage)
        {
            if (percentage >= 90) return "A+";
            if (percentage >= 80) return "A";
            if (percentage >= 70) return "B+";
            if (percentage >= 60) return "B";
            if (percentage >= 50) return "C+";
            if (percentage >= 40) return "C";
            if (percentage >= 30) return "D";
            return "F";
        }











        private void btnToExcel_Click(object sender, EventArgs e)
        {
            try
            {
                ExportRankingToExcel();
                MessageBox.Show("Data exported to Excel successfully!", "Export Successful",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exporting to Excel: " + ex.Message, "Export Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRanking_Click(object sender, EventArgs e)
        {
            try
            {
                GenerateRanking();
                MessageBox.Show("Ranking generated successfully!", "Ranking Generation",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating ranking: " + ex.Message, "Ranking Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Data Grid Events

        private void DgvMarks_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvMarks.Rows[e.RowIndex];

                // Skip total row calculations
                if (row.Cells["LearningArea"].Value?.ToString() == "TOTAL")
                    return;

                // Auto-calculate performance level when marks are entered
                if (dgvMarks.Columns[e.ColumnIndex].Name == "MarksObtained")
                {
                    string value = row.Cells["MarksObtained"].Value?.ToString();

                    if (int.TryParse(value, out int marks))
                    {
                        // Auto-suggest CBC performance level based on marks
                        string suggestedLevel = GetCbcRubric(marks);
                        row.Cells["PerformanceLevel"].Value = suggestedLevel;
                    }
                    else
                    {
                        row.Cells["PerformanceLevel"].Value = "";
                    }

                    // Update total row
                    UpdateTotalRow();
                }
            }
        }

        private void DgvMarks_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            System.Windows.Forms.TextBox tb = e.Control as System.Windows.Forms.TextBox;
            if (tb != null)
            {
                tb.TextChanged -= TextBox_TextChanged;
                tb.TextChanged += TextBox_TextChanged;
            }
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            dgvMarks.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        #endregion

        #region Validation Methods

        private bool ValidateStudentData()
        {
            if (string.IsNullOrWhiteSpace(txtStudentID.Text))
            {
                MessageBox.Show("Please enter Student ID.", "Validation Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtStudentID.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(FirstNameComboBox.Text))
            {
                MessageBox.Show("Please enter First Name.", "Validation Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FirstNameComboBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                MessageBox.Show("Please enter Last Name.", "Validation Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtLastName.Focus();
                return false;
            }

            if (cmbClass.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a Class.", "Validation Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbClass.Focus();
                return false;
            }

            if (cmbTerm.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a Term.", "Validation Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbTerm.Focus();
                return false;
            }

            return true;
        }

        private bool ValidateMarksData()
        {
            foreach (DataGridViewRow row in dgvMarks.Rows)
            {
                if (!row.IsNewRow && row.Cells["LearningArea"].Value?.ToString() != "TOTAL")
                {
                    string marksObtained = row.Cells["MarksObtained"].Value?.ToString();
                    if (!string.IsNullOrWhiteSpace(marksObtained))
                    {
                        if (!int.TryParse(marksObtained, out int marks) || marks < 0 || marks > 100)
                        {
                            MessageBox.Show($"Please enter valid marks (0-100) for {row.Cells["LearningArea"].Value}.",
                                          "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                    }
                }
            }
            return ValidatePerformanceLevels();
        }

        private bool ValidateReportParameters()
        {
            if (string.IsNullOrWhiteSpace(txtStudent.Text))
            {
                MessageBox.Show("Please enter Student ID for report generation.", "Validation Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private bool ValidatePerformanceLevels()
        {
            var validLevels = new[] { "", "EE1", "EE2", "ME1", "ME2", "AE1", "AE2", "BE1", "BE2" };

            foreach (DataGridViewRow row in dgvMarks.Rows)
            {
                if (row.IsNewRow || row.Cells["LearningArea"].Value?.ToString() == "TOTAL")
                    continue;

                string level = row.Cells["PerformanceLevel"].Value?.ToString();

                if (!string.IsNullOrEmpty(level) && !validLevels.Contains(level))
                {
                    MessageBox.Show($"Invalid performance level '{level}' found. Please select a valid CBC rubric level.",
                                  "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region Database Operations

        private void SearchStudentInDatabase(string studentId)
        {

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string sql = @"SELECT StudentID, FirstName, LastName, Class 
                           FROM StdTable 
                           WHERE StudentID = @StudentID";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@StudentID", studentId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // ✅ Populate student information
                                txtStudentID.Text = reader["StudentID"]?.ToString() ?? "";
                                txtLastName.Text = reader["LastName"]?.ToString() ?? "";

                                string firstName = reader["FirstName"]?.ToString() ?? "";
                                string studentClass = reader["Class"]?.ToString() ?? "";

                                // ✅ Try to select the student's first name from combo
                                var match = FirstNameComboBox.Items.Cast<StudentComboItem>()
                                                .FirstOrDefault(item => item.FirstName == firstName);

                                if (match != null)
                                {
                                    FirstNameComboBox.SelectedItem = match;
                                }
                                else
                                {
                                    // fallback: set text directly if not in list
                                    FirstNameComboBox.Text = firstName;
                                }

                                // ✅ Set the class in combo box
                                cmbClass.Text = studentClass;

                                // ✅ Automatically load learning areas
                                if (!string.IsNullOrEmpty(studentClass))
                                {
                                    PopulateLearningAreasFromDatabase(studentClass);
                                }

                                MessageBox.Show(
                                    $"Student found: {firstName} {txtLastName.Text}\nClass: {studentClass}",
                                    "Search Successful",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information
                                );
                            }
                            else
                            {
                                MessageBox.Show("Student not found in the database.", "Search Result",
                                              MessageBoxButtons.OK, MessageBoxIcon.Information);

                                // Clear form if student not found
                                ClearStudentInfo();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database error during student search: " + ex.Message, "Database Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void ClearStudentInfo()
        {
            FirstNameComboBox.SelectedIndex = -1;
            txtLastName.Clear();
            cmbClass.SelectedIndex = -1;
            dgvMarks.Rows.Clear();
        }

        private void SaveMarksToDatabase()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Delete existing marks for this student, term, and year
                        string deleteSql = "DELETE FROM StudentMarks WHERE StudentID = @StudentID AND Term = @Term AND Year = @Year";
                        using (SqlCommand deleteCmd = new SqlCommand(deleteSql, conn, transaction))
                        {
                            deleteCmd.Parameters.AddWithValue("@StudentID", txtStudentID.Text);
                            deleteCmd.Parameters.AddWithValue("@Term", cmbTerm.Text);
                            deleteCmd.Parameters.AddWithValue("@Year", txtYear.Text);
                            deleteCmd.ExecuteNonQuery();
                        }

                        // Insert new marks
                        foreach (DataGridViewRow row in dgvMarks.Rows)
                        {
                            if (!row.IsNewRow &&
                                row.Cells["LearningArea"].Value?.ToString() != "TOTAL" &&
                                !string.IsNullOrWhiteSpace(row.Cells["MarksObtained"].Value?.ToString()))
                            {
                                // ✅ FIXED: Column order now matches parameter order
                                string insertSql = @"INSERT INTO StudentMarks 
                                           (StudentID, LearningArea, MarksObtained, TotalMarks, ExamType, Term, Year, Class, PerformanceLevel) 
                                           VALUES (@StudentID, @LearningArea, @MarksObtained, @TotalMarks, @ExamType, @Term, @Year, @Class, @PerformanceLevel)";

                                using (SqlCommand insertCmd = new SqlCommand(insertSql, conn, transaction))
                                {
                                    // ✅ Parameters now match the column order exactly
                                    insertCmd.Parameters.AddWithValue("@StudentID", txtStudentID.Text);
                                    insertCmd.Parameters.AddWithValue("@LearningArea", row.Cells["LearningArea"].Value);

                                    // ✅ Convert to int to ensure proper data type
                                    int marksObtained = 0;
                                    if (int.TryParse(row.Cells["MarksObtained"].Value?.ToString(), out marksObtained))
                                    {
                                        insertCmd.Parameters.AddWithValue("@MarksObtained", marksObtained);
                                    }
                                    else
                                    {
                                        insertCmd.Parameters.AddWithValue("@MarksObtained", 0);
                                    }

                                    // ✅ Convert to int for TotalMarks
                                    int totalMarks = 0;
                                    if (int.TryParse(row.Cells["TotalMarks"].Value?.ToString(), out totalMarks))
                                    {
                                        insertCmd.Parameters.AddWithValue("@TotalMarks", totalMarks);
                                    }
                                    else
                                    {
                                        insertCmd.Parameters.AddWithValue("@TotalMarks", 100); // Default to 100
                                    }

                                    insertCmd.Parameters.AddWithValue("@ExamType", cmbExamType.Text ?? "");
                                    insertCmd.Parameters.AddWithValue("@Term", cmbTerm.Text);
                                    insertCmd.Parameters.AddWithValue("@Year", txtYear.Text);
                                    insertCmd.Parameters.AddWithValue("@Class", cmbClass.Text);
                                    insertCmd.Parameters.AddWithValue("@PerformanceLevel", row.Cells["PerformanceLevel"].Value ?? "");

                                    insertCmd.ExecuteNonQuery();
                                }
                            }
                        }

                        transaction.Commit();
                        MessageBox.Show("Marks saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"Error saving marks: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        throw;
                    }
                }
            }
        }


        private void LoadMarksFromDatabase()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = @"SELECT LearningArea, MarksObtained, TotalMarks, PerformanceLevel 
                                 FROM StudentMarks 
                                 WHERE StudentID = @StudentID AND Term = @Term AND Year = @Year AND Class = @Class";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@StudentID", txtStudentID.Text);
                        cmd.Parameters.AddWithValue("@Term", cmbTerm.Text);
                        cmd.Parameters.AddWithValue("@Year", txtYear.Text);
                        cmd.Parameters.AddWithValue("@Class", cmbClass.Text);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            // Clear existing marks but keep learning areas structure
                            foreach (DataGridViewRow row in dgvMarks.Rows)
                            {
                                if (!row.IsNewRow && row.Cells["LearningArea"].Value?.ToString() != "TOTAL")
                                {
                                    row.Cells["MarksObtained"].Value = "";
                                    row.Cells["PerformanceLevel"].Value = "";
                                }
                            }

                            // Load marks from database
                            while (reader.Read())
                            {
                                string learningArea = reader["LearningArea"].ToString();

                                // Find the corresponding row in the grid
                                foreach (DataGridViewRow row in dgvMarks.Rows)
                                {
                                    if (row.Cells["LearningArea"].Value?.ToString() == learningArea)
                                    {
                                        row.Cells["MarksObtained"].Value = reader["MarksObtained"];
                                        row.Cells["TotalMarks"].Value = reader["TotalMarks"];
                                        row.Cells["PerformanceLevel"].Value = reader["PerformanceLevel"];
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                // Update total row after loading
                UpdateTotalRow();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading marks from database: " + ex.Message, "Load Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Helper Methods

        private void ClearFormFields()
        {
            txtStudentID.Clear();
            FirstNameComboBox.SelectedIndex = -1;
            txtLastName.Clear();
            cmbClass.SelectedIndex = -1;
            cmbTerm.SelectedIndex = -1;
            txtYear.Text = DateTime.Now.Year.ToString();

            // Clear marks grid
            dgvMarks.Rows.Clear();

            // Clear report generation fields
            txtStudent.Clear();
            comboClass.SelectedIndex = -1;
            comboTerm.SelectedIndex = -1;
            textYear.Text = DateTime.Now.Year.ToString();

            // Clear ranking grid
            dgvRanking.Rows.Clear();
        }

        private string GetCbcRubric(int marks)
        {
            if (marks >= 90) return "EE1";      // 90-100
            else if (marks >= 80) return "EE2"; // 80-89
            else if (marks >= 70) return "ME1"; // 70-79
            else if (marks >= 60) return "ME2"; // 60-69
            else if (marks >= 50) return "AE1"; // 50-59
            else if (marks >= 40) return "AE2"; // 40-49
            else if (marks >= 30) return "BE1"; // 30-39
            else return "BE2";                  // 0-29
        }

        private string GetPerformanceLevelDescription(string level)
        {
            switch (level)
            {
                case "EE1": return "Exceeds Expectations - Outstanding Performance";
                case "EE2": return "Exceeds Expectations - Excellent Performance";
                case "ME1": return "Meets Expectations - Very Good Performance";
                case "ME2": return "Meets Expectations - Good Performance";
                case "AE1": return "Approaches Expectations - Satisfactory Performance";
                case "AE2": return "Approaches Expectations - Fair Performance";
                case "BE1": return "Below Expectations - Needs Improvement";
                case "BE2": return "Below Expectations - Significant Support Needed";
                default: return "Not Assessed";
            }
        }

        #endregion

        #region Report Generation

        private void GenerateReport()
        {
            // Placeholder for report generation using ReportViewer
            MessageBox.Show("Report generation functionality to be implemented with ReportViewer.",
                          "Report Generation", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void GenerateRanking()
        {
            // Clear existing data
            dgvRanking.Rows.Clear();

            // Setup ranking grid columns
            dgvRanking.Columns.Clear();
            dgvRanking.Columns.Add("Rank", "Rank");
            dgvRanking.Columns.Add("StudentID", "Student ID");
            dgvRanking.Columns.Add("Name", "Student Name");
            dgvRanking.Columns.Add("Class", "Class");
            dgvRanking.Columns.Add("TotalMarks", "Total Marks");
            dgvRanking.Columns.Add("Average", "Average %");
            dgvRanking.Columns.Add("OverallLevel", "Overall Performance Level");

            // Add sample ranking data
            dgvRanking.Rows.Add("1", "001", "John Doe", "GRADE 5", "850", "85.0", "EE2");
            dgvRanking.Rows.Add("2", "002", "Jane Smith", "GRADE 5", "840", "84.0", "EE2");
            dgvRanking.Rows.Add("3", "003", "Mike Johnson", "GRADE 5", "750", "75.0", "ME1");
        }

        private void ExportRankingToExcel()
        {

            if (dgvRanking.Rows.Count == 0)
            {
                throw new Exception("No data available to export.");
            }

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
            saveDialog.FilterIndex = 1;
            saveDialog.FileName = "Student_Ranking_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";

            if (saveDialog.ShowDialog() != DialogResult.OK)
            {
                return; // User cancelled
            }

            StringBuilder sb = new StringBuilder();

            // Add title
            sb.AppendLine("STUDENT RANKING REPORT");
            sb.AppendLine("Generated on: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            sb.AppendLine();

            // Add headers
            string[] headers = new string[dgvRanking.Columns.Count];
            for (int i = 0; i < dgvRanking.Columns.Count; i++)
            {
                headers[i] = dgvRanking.Columns[i].HeaderText;
            }
            sb.AppendLine(string.Join(",", headers));

            // Add data
            foreach (DataGridViewRow row in dgvRanking.Rows)
            {
                if (!row.IsNewRow)
                {
                    string[] values = new string[dgvRanking.Columns.Count];
                    for (int i = 0; i < dgvRanking.Columns.Count; i++)
                    {
                        values[i] = $"\"{row.Cells[i].Value?.ToString() ?? ""}\"";
                    }
                    sb.AppendLine(string.Join(",", values));
                }
            }

            File.WriteAllText(saveDialog.FileName, sb.ToString());

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx";
            saveFileDialog.FileName = $"Student_Ranking_{DateTime.Now:yyyyMMdd}.xlsx";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Export logic would go here using EPPlus or similar library
                MessageBox.Show($"Ranking data would be exported to: {saveFileDialog.FileName}");
            }
        }

        #endregion

        #region Print Functionality

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Font titleFont = new Font("Arial", 16, FontStyle.Bold);
            Font headerFont = new Font("Arial", 12, FontStyle.Bold);
            Font normalFont = new Font("Arial", 10);
            Font smallFont = new Font("Arial", 9);
            Font boldFont = new Font("Arial", 10, FontStyle.Bold);
            Brush blackBrush = Brushes.Black;
            Brush blueBrush = Brushes.Blue;
            int yPosition = 50;
            int xPosition = 50;
            int pageWidth = e.PageBounds.Width - 100;

            // Print title
            string title = "EXAM MANAGEMENT 5.0";
            SizeF titleSize = g.MeasureString(title, titleFont);
            g.DrawString(title, titleFont, blueBrush,
                        (pageWidth - titleSize.Width) / 2 + 50, yPosition);
            yPosition += 50;

            // Print student information header
            g.DrawString("STUDENT INFORMATION", headerFont, blackBrush, xPosition, yPosition);
            yPosition += 30;

            // Print student details
            g.DrawString($"Student ID: {txtStudentID.Text}", normalFont, blackBrush, xPosition, yPosition);
            yPosition += 20;

            // ✅ Handle cmbFirstName (ComboBox) correctly
            string firstName;
            if (FirstNameComboBox.SelectedItem is StudentComboItem selectedItem)
            {
                firstName = selectedItem.FirstName;
            }
            else
            {
                firstName = FirstNameComboBox.Text; // fallback to text
            }

            g.DrawString($"Name: {firstName} {txtLastName.Text}", normalFont, blackBrush, xPosition, yPosition);
            yPosition += 20;

            g.DrawString($"Class: {cmbClass.Text}", normalFont, blackBrush, xPosition, yPosition);
            g.DrawString($"Term: {cmbTerm.Text}", normalFont, blueBrush, xPosition + 200, yPosition);
            yPosition += 20;
            g.DrawString($"Year: {txtYear.Text}", normalFont, blackBrush, xPosition, yPosition);
            Pen thinPen = Pens.Black;
            // Exam type
            string examType = cmbExamType.Text;
            g.DrawString($"Exam Type: {examType}", normalFont, blackBrush, xPosition + 200, yPosition);
            yPosition += 40;

            // Print marks table header
            g.DrawString("ACADEMIC PERFORMANCE", headerFont, blackBrush, xPosition, yPosition);
            yPosition += 30;

            int col1Width = 220;  // Learning Area
            int col2Width = 100;  // Marks Obtained
            int col3Width = 100;  // Total Marks
            int col4Width = 130;  // Performance Level

            // Draw header background
            Rectangle headerRect = new Rectangle(xPosition, yPosition, col1Width + col2Width + col3Width + col4Width, 25);
            g.FillRectangle(Brushes.LightGray, headerRect);
            g.DrawRectangle(Pens.Black, headerRect);

            g.DrawString("Learning Area", normalFont, blackBrush, xPosition + 5, yPosition + 5);
            g.DrawString("Marks", normalFont, blackBrush, xPosition + col1Width + 5, yPosition + 5);
            g.DrawString("Total", normalFont, blackBrush, xPosition + col1Width + col2Width + 5, yPosition + 5);
            g.DrawString("Performance Level", normalFont, blackBrush, xPosition + col1Width + col2Width + col3Width + 5, yPosition + 5);
            yPosition += 25;

            // Print marks data
            foreach (DataGridViewRow row in dgvMarks.Rows)
            {
                if (row.IsNewRow) continue;

                string learningArea = row.Cells["LearningArea"]?.Value?.ToString() ?? "";
                string marksObtained = row.Cells["MarksObtained"]?.Value?.ToString() ?? "";
                string totalMarks = row.Cells["TotalMarks"]?.Value?.ToString() ?? "";
                string performanceLevel = row.Cells["PerformanceLevel"]?.Value?.ToString() ?? "";

                bool isTotalRow = learningArea == "TOTAL";
                Font rowFont = isTotalRow ? boldFont : smallFont;
                Brush textBrush = isTotalRow ? Brushes.DarkBlue : blackBrush;

                if (!isTotalRow && row.Index % 2 == 0)
                {
                    Rectangle rowRect = new Rectangle(xPosition, yPosition, col1Width + col2Width + col3Width + col4Width, 20);
                    g.FillRectangle(Brushes.WhiteSmoke, rowRect);
                }
                else if (isTotalRow)
                {
                    Rectangle rowRect = new Rectangle(xPosition, yPosition, col1Width + col2Width + col3Width + col4Width, 20);
                    g.FillRectangle(Brushes.LightBlue, rowRect);
                }

                // Draw cell borders
                g.DrawRectangle(Pens.Black, xPosition, yPosition, col1Width, 20);
                g.DrawRectangle(Pens.Black, xPosition + col1Width, yPosition, col2Width, 20);
                g.DrawRectangle(Pens.Black, xPosition + col1Width + col2Width, yPosition, col3Width, 20);
                g.DrawRectangle(Pens.Black, xPosition + col1Width + col2Width + col3Width, yPosition, col4Width, 20);

                // Draw cell content
                g.DrawString(learningArea, rowFont, textBrush, xPosition + 5, yPosition + 3);
                g.DrawString(marksObtained, rowFont, textBrush, xPosition + col1Width + 5, yPosition + 3);
                g.DrawString(totalMarks, rowFont, textBrush, xPosition + col1Width + col2Width + 5, yPosition + 3);
                g.DrawString(performanceLevel, rowFont, textBrush, xPosition + col1Width + col2Width + col3Width + 5, yPosition + 3);

                yPosition += 20;

                if (yPosition > e.PageBounds.Height - 200)
                {
                    e.HasMorePages = true;
                    return;
                }
            }



            // TERMLY COMMENT SECTION
            yPosition += 40;

            // Termly comment title
            g.DrawString("TERMLY COMMENT", boldFont, blackBrush, xPosition, yPosition);
            yPosition += 30;

            // Closing and opening dates
            g.DrawString("Closing Date :...............................", smallFont, blackBrush, xPosition, yPosition);
            g.DrawString("Opening Date:...............................", smallFont, blackBrush, xPosition + 300, yPosition);
            yPosition += 25;

            // Class teacher and dean of studies
            g.DrawString("Class Teacher signature ...............................", smallFont, blackBrush, xPosition, yPosition);
            g.DrawString("Dean of studies signature...............................", smallFont, blackBrush, xPosition + 300, yPosition);
            yPosition += 25;

            // Head teacher
            g.DrawString("Head Teacher signature ...............................", smallFont, blackBrush, xPosition, yPosition);

            // School stamp (right-aligned)
            g.DrawString("School Stamp", smallFont, blackBrush, xPosition + 500, yPosition + 10);
            yPosition += 50;

            // Draw rectangle border for grading key
            Rectangle gradingBox = new Rectangle(xPosition, yPosition, 460, 150);
            g.DrawRectangle(thinPen, gradingBox);

            // Title inside grading key box
            g.DrawString("Key to CBC Performance Levels", new Font("Arial", 9, FontStyle.Bold), blackBrush, xPosition + 5, yPosition + 5);

            // Performance level descriptions
            yPosition += 25;
            g.DrawString("EE1 (90-100): Exceeds Expectations - Outstanding", smallFont, blackBrush, xPosition + 10, yPosition);
            yPosition += 15;
            g.DrawString("EE2 (80-89): Exceeds Expectations - Excellent", smallFont, blackBrush, xPosition + 10, yPosition);
            yPosition += 15;
            g.DrawString("ME1 (70-79): Meets Expectations - Very Good", smallFont, blackBrush, xPosition + 10, yPosition);
            yPosition += 15;
            g.DrawString("ME2 (60-69): Meets Expectations - Good", smallFont, blackBrush, xPosition + 10, yPosition);
            yPosition += 15;
            g.DrawString("AE1 (50-59): Approaches Expectations - Satisfactory", smallFont, blackBrush, xPosition + 10, yPosition);
            yPosition += 15;
            g.DrawString("AE2 (40-49): Approaches Expectations - Fair", smallFont, blackBrush, xPosition + 10, yPosition);
            yPosition += 15;
            g.DrawString("BE1 (30-39): Below Expectations - Needs Improvement", smallFont, blackBrush, xPosition + 10, yPosition);
            yPosition += 15;
            g.DrawString("BE2 (0-29): Below Expectations - Significant Support Needed", smallFont, blackBrush, xPosition + 10, yPosition);

            // Print footer
            yPosition += 32;
            string printDate = $"Printed on: {DateTime.Now:dd/MM/yyyy HH:mm}";
            g.DrawString(printDate, smallFont, blackBrush, xPosition, yPosition);

            // Dispose fonts
            titleFont.Dispose();
            headerFont.Dispose();
            normalFont.Dispose();
            smallFont.Dispose();
            boldFont.Dispose();
        }

        private void CleanupPrintComponents()
        {
            printDocument?.Dispose();
            printPreviewDialog?.Dispose();
        }

        #endregion

        #region Cleanup and Disposal



        #endregion

        private void examDGV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
        
            if (e.RowIndex < 0) return;

            DataGridViewRow row = examDGV.Rows[e.RowIndex];
            string fullName = "";

            // Try different possible column names
            if (examDGV.Columns.Contains("StudentName"))
                fullName = row.Cells["StudentName"].Value?.ToString() ?? "";
            else if (examDGV.Columns.Contains("FullName"))
                fullName = row.Cells["FullName"].Value?.ToString() ?? "";
            else if (examDGV.Columns.Contains("Name"))
                fullName = row.Cells["Name"].Value?.ToString() ?? "";

            // Split into words
            var nameParts = fullName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // First name
            FirstNameComboBox.Text = nameParts.Length > 0 ? nameParts[0] : "";

            // Last name
            txtLastName.Text = nameParts.Length > 1 ? string.Join(" ", nameParts.Skip(1)) : "";
        }

        









        private void PopulateFirstNameComboBox()
        {
            try
            {
                // Clear existing items
                FirstNameComboBox.Items.Clear();
                FirstNameComboBox.Text = "";

                // Get the selected class from the Class ComboBox
                string selectedClass = cmbClass.SelectedItem?.ToString();

                if (string.IsNullOrEmpty(selectedClass))
                {
                    FirstNameComboBox.Enabled = false;
                    return;
                }

                FirstNameComboBox.Enabled = true;

                // Database connection and query
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"SELECT DISTINCT FirstName, LastName, StudentID 
                           FROM StdTable 
                           WHERE Class = @Class 
                           ORDER BY FirstName";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Class", selectedClass);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Create a custom item that stores both display text and student info
                                string firstName = reader["FirstName"].ToString();
                                string lastName = reader["LastName"].ToString();
                                string studentId = reader["StudentID"].ToString();

                                // Add item with FirstName as display, but store full info
                                var studentItem = new StudentComboItem
                                {
                                    FirstName = firstName,
                                    LastName = lastName,
                                    StudentID = studentId,
                                    DisplayText = firstName
                                };

                                FirstNameComboBox.Items.Add(studentItem);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading student names: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    
// Custom class to hold student information in ComboBox items
public class StudentComboItem
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string StudentID { get; set; }
        public string Class { get; set; }
        public string DisplayText { get; set; }

        public override string ToString()
        {
            return DisplayText;
        }
}

// Event handler for Class ComboBox selection change
private void classComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateFirstNameComboBox();

            // Clear other dependent fields
            txtLastName.Text = "";
            txtStudentID.Text = "";
        }

        // Event handler for FirstName ComboBox selection change
        private void firstNameComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (FirstNameComboBox.SelectedItem is StudentComboItem selectedStudent)
            {
                // Auto-populate LastName and StudentID based on selection
                txtLastName.Text = selectedStudent.LastName;
                txtStudentID.Text = selectedStudent.StudentID;
            }
        }

        // Alternative implementation with filtering/search capability
        private void SetupFirstNameComboBoxWithFiltering()
        {
            // Enable auto-complete for better user experience
            FirstNameComboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            FirstNameComboBox.AutoCompleteSource = AutoCompleteSource.ListItems;

            // Handle text change for real-time filtering
            FirstNameComboBox.TextChanged += FirstNameComboBox_TextChanged;
        }

        private void FirstNameComboBox_TextChanged(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            string searchText = cb.Text.ToLower();

            if (searchText.Length < 2) return; // Start filtering after 2 characters

            // Filter items based on search text
            var filteredItems = allStudentItems.Where(item =>
                item.FirstName.ToLower().Contains(searchText)).ToList();

            // Update dropdown items
            cb.Items.Clear();
            foreach (var item in filteredItems)
            {
                cb.Items.Add(item);
            }

            if (filteredItems.Count > 0)
            {
                cb.DroppedDown = true;
            }
        }

        // Method to initialize the form
        private void InitializeStudentForm()
        {
            // Set up the ComboBox properties
            FirstNameComboBox.DropDownStyle = ComboBoxStyle.DropDown; // Allows typing
            FirstNameComboBox.Sorted = true; // Auto-sort items alphabetically

            SetupFirstNameComboBoxWithFiltering();

            // Load initial data when form loads
            LoadClassData();
        }

        // Load class data into Class ComboBox
        private void LoadClassData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT DISTINCT Class FROM StdTable ORDER BY Class";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cmbClass.Items.Add(reader["Class"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading classes: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
      
        private void comboBoxFirstName_SelectedIndexChanged(object sender, EventArgs e)
        {
        

            if (FirstNameComboBox.SelectedIndex < 0 || FirstNameComboBox.SelectedItem == null)
            {
                txtLastName.Text = string.Empty;
                if (cmbClass.Items.Count > 0)
                    cmbClass.SelectedIndex = 0;
                return;
            }

            if (FirstNameComboBox.SelectedItem is StudentComboItem selectedStudent)
            {
                // Auto-fill last name
                txtLastName.Text = selectedStudent.LastName;

                // Auto-fill class
                if (cmbClass.Items.Contains(selectedStudent.Class))
                {
                    cmbClass.SelectedItem = selectedStudent.Class;
                }
                else
                {
                    if (cmbClass.Items.Count > 0)
                        cmbClass.SelectedIndex = 0;
                }


            }
        }

        private void FillOtherFields(string firstName)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = "SELECT LastName, Class FROM StdTable WHERE FirstName=@firstName";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@firstName", firstName);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtLastName.Text = reader["LastName"].ToString();
                            cmbClass.Text = reader["Class"].ToString();
                        }
                    }
                }
            }
        }
        private void LoadStudentsData()
        {
        
            try
            {
                allStudentItems.Clear();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                SELECT 
                    s.StudentID, 
                    s.FirstName, 
                    s.LastName, 
                    S.Class
                FROM StdTable s
                LEFT JOIN FeeTbl f ON s.StudentID = f.StudentID
                ORDER BY s.FirstName, s.LastName";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            allStudentItems.Add(new StudentComboItem
                            {
                                StudentID = reader["StudentID"].ToString(),
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                Class = reader["Class"] == DBNull.Value ? "" : reader["Class"].ToString(),
                                DisplayText = reader["FirstName"].ToString()
                            });
                        }
                    }
                }

                // Bind to ComboBox
                FirstNameComboBox.DataSource = null;
                FirstNameComboBox.DataSource = allStudentItems;
                FirstNameComboBox.DisplayMember = "DisplayText";
                FirstNameComboBox.ValueMember = "StudentID";

                FirstNameComboBox.SelectedIndex = -1;
                FirstNameComboBox.Text = "-- Select Student --";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading student data: {ex.Message}");
            }
        }

        

        

        private void FirstNameComboBox_Resize(object sender, EventArgs e)
        {
        
            if (FirstNameComboBox.SelectedIndex < 0 || FirstNameComboBox.SelectedItem == null)
            {
                txtLastName.Text = string.Empty;
                if (cmbClass.Items.Count > 0)
                    cmbClass.SelectedIndex = 0;
                return;
            }

            if (FirstNameComboBox.SelectedItem is StudentComboItem selectedStudent)
            {
                // Auto-fill last name
                txtLastName.Text = selectedStudent.LastName;

                // Auto-fill class
                if (cmbClass.Items.Contains(selectedStudent.Class))
                {
                    cmbClass.SelectedItem = selectedStudent.Class;
                }
                else
                {
                    if (cmbClass.Items.Count > 0)
                        cmbClass.SelectedIndex = 0;
                }

              
            }
        }

    }
}




