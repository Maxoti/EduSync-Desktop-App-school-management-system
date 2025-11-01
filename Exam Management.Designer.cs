namespace EduSync
{
    partial class Exam_Management
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabMarksEntry = new System.Windows.Forms.TabPage();
            this.grpEnterMarks = new System.Windows.Forms.GroupBox();
            this.FirstNameComboBox = new System.Windows.Forms.ComboBox();
            this.examDGV = new System.Windows.Forms.DataGridView();
            this.label12 = new System.Windows.Forms.Label();
            this.cmbExamType = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnclear = new System.Windows.Forms.Button();
            this.txtYear = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cmbTerm = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbClass = new System.Windows.Forms.ComboBox();
            this.txtLastName = new System.Windows.Forms.TextBox();
            this.txtStudentID = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tabExamRecords = new System.Windows.Forms.TabPage();
            this.printbtn = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnLoadMarks = new System.Windows.Forms.Button();
            this.dgvMarks = new System.Windows.Forms.DataGridView();
            this.tabReportGen = new System.Windows.Forms.TabPage();
            this.textYear = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.comboTerm = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.comboClass = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txtStudent = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.btnGen = new System.Windows.Forms.Button();
            this.rptGen = new Microsoft.Reporting.WinForms.ReportViewer();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.btnToExcel = new System.Windows.Forms.Button();
            this.btnRanking = new System.Windows.Forms.Button();
            this.dgvRanking = new System.Windows.Forms.DataGridView();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabMarksEntry.SuspendLayout();
            this.grpEnterMarks.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.examDGV)).BeginInit();
            this.tabExamRecords.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMarks)).BeginInit();
            this.tabReportGen.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRanking)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Teal;
            this.panel1.Controls.Add(this.pictureBox2);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.ForeColor = System.Drawing.Color.DodgerBlue;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1276, 80);
            this.panel1.TabIndex = 17;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new System.Drawing.Point(1294, 19);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(74, 50);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 39;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(3, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(86, 74);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 38;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Stencil", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label1.Location = new System.Drawing.Point(326, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(369, 35);
            this.label1.TabIndex = 2;
            this.label1.Text = "EXAM MANAGEMENT 5.0";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabMarksEntry);
            this.tabControl1.Controls.Add(this.tabExamRecords);
            this.tabControl1.Controls.Add(this.tabReportGen);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Font = new System.Drawing.Font("Century", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.Location = new System.Drawing.Point(32, 100);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1087, 529);
            this.tabControl1.TabIndex = 26;
            // 
            // tabMarksEntry
            // 
            this.tabMarksEntry.Controls.Add(this.grpEnterMarks);
            this.tabMarksEntry.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabMarksEntry.Location = new System.Drawing.Point(4, 30);
            this.tabMarksEntry.Name = "tabMarksEntry";
            this.tabMarksEntry.Padding = new System.Windows.Forms.Padding(3);
            this.tabMarksEntry.Size = new System.Drawing.Size(1079, 495);
            this.tabMarksEntry.TabIndex = 0;
            this.tabMarksEntry.Text = "Student Info";
            this.tabMarksEntry.UseVisualStyleBackColor = true;
            // 
            // grpEnterMarks
            // 
            this.grpEnterMarks.BackColor = System.Drawing.Color.MintCream;
            this.grpEnterMarks.Controls.Add(this.FirstNameComboBox);
            this.grpEnterMarks.Controls.Add(this.examDGV);
            this.grpEnterMarks.Controls.Add(this.label12);
            this.grpEnterMarks.Controls.Add(this.cmbExamType);
            this.grpEnterMarks.Controls.Add(this.label7);
            this.grpEnterMarks.Controls.Add(this.btnSearch);
            this.grpEnterMarks.Controls.Add(this.btnclear);
            this.grpEnterMarks.Controls.Add(this.txtYear);
            this.grpEnterMarks.Controls.Add(this.label6);
            this.grpEnterMarks.Controls.Add(this.cmbTerm);
            this.grpEnterMarks.Controls.Add(this.label5);
            this.grpEnterMarks.Controls.Add(this.cmbClass);
            this.grpEnterMarks.Controls.Add(this.txtLastName);
            this.grpEnterMarks.Controls.Add(this.txtStudentID);
            this.grpEnterMarks.Controls.Add(this.label4);
            this.grpEnterMarks.Controls.Add(this.label3);
            this.grpEnterMarks.Controls.Add(this.label2);
            this.grpEnterMarks.Location = new System.Drawing.Point(19, 23);
            this.grpEnterMarks.Name = "grpEnterMarks";
            this.grpEnterMarks.Size = new System.Drawing.Size(987, 466);
            this.grpEnterMarks.TabIndex = 0;
            this.grpEnterMarks.TabStop = false;
            this.grpEnterMarks.Text = "Student Info";
            // 
            // FirstNameComboBox
            // 
            this.FirstNameComboBox.FormattingEnabled = true;
            this.FirstNameComboBox.Location = new System.Drawing.Point(130, 73);
            this.FirstNameComboBox.Name = "FirstNameComboBox";
            this.FirstNameComboBox.Size = new System.Drawing.Size(126, 33);
            this.FirstNameComboBox.TabIndex = 18;
            this.FirstNameComboBox.SelectedIndexChanged += new System.EventHandler(this.comboBoxFirstName_SelectedIndexChanged);
            this.FirstNameComboBox.Resize += new System.EventHandler(this.FirstNameComboBox_Resize);
            // 
            // examDGV
            // 
            this.examDGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.examDGV.Location = new System.Drawing.Point(14, 197);
            this.examDGV.Name = "examDGV";
            this.examDGV.RowHeadersWidth = 51;
            this.examDGV.RowTemplate.Height = 24;
            this.examDGV.Size = new System.Drawing.Size(950, 248);
            this.examDGV.TabIndex = 17;
            this.examDGV.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.examDGV_CellClick);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Century", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(853, 43);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(108, 21);
            this.label12.TabIndex = 16;
            this.label12.Text = "Exam Type";
            // 
            // cmbExamType
            // 
            this.cmbExamType.FormattingEnabled = true;
            this.cmbExamType.Location = new System.Drawing.Point(857, 77);
            this.cmbExamType.Name = "cmbExamType";
            this.cmbExamType.Size = new System.Drawing.Size(107, 33);
            this.cmbExamType.TabIndex = 15;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Century", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(149, 43);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(105, 21);
            this.label7.TabIndex = 13;
            this.label7.Text = "FirstName";
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btnSearch.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnSearch.Location = new System.Drawing.Point(14, 141);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(112, 37);
            this.btnSearch.TabIndex = 11;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnclear
            // 
            this.btnclear.BackColor = System.Drawing.Color.Cyan;
            this.btnclear.Location = new System.Drawing.Point(226, 141);
            this.btnclear.Name = "btnclear";
            this.btnclear.Size = new System.Drawing.Size(82, 37);
            this.btnclear.TabIndex = 10;
            this.btnclear.Text = "Clear";
            this.btnclear.UseVisualStyleBackColor = false;
            this.btnclear.Click += new System.EventHandler(this.btnclear_Click);
            // 
            // txtYear
            // 
            this.txtYear.Location = new System.Drawing.Point(727, 73);
            this.txtYear.Name = "txtYear";
            this.txtYear.Size = new System.Drawing.Size(71, 32);
            this.txtYear.TabIndex = 9;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Century", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(746, 43);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(52, 21);
            this.label6.TabIndex = 8;
            this.label6.Text = "Year";
            // 
            // cmbTerm
            // 
            this.cmbTerm.FormattingEnabled = true;
            this.cmbTerm.Location = new System.Drawing.Point(582, 72);
            this.cmbTerm.Name = "cmbTerm";
            this.cmbTerm.Size = new System.Drawing.Size(103, 33);
            this.cmbTerm.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Century", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(592, 43);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(57, 21);
            this.label5.TabIndex = 6;
            this.label5.Text = "Term";
            // 
            // cmbClass
            // 
            this.cmbClass.FormattingEnabled = true;
            this.cmbClass.Location = new System.Drawing.Point(442, 74);
            this.cmbClass.Name = "cmbClass";
            this.cmbClass.Size = new System.Drawing.Size(115, 33);
            this.cmbClass.TabIndex = 5;
            // 
            // txtLastName
            // 
            this.txtLastName.Location = new System.Drawing.Point(277, 75);
            this.txtLastName.Name = "txtLastName";
            this.txtLastName.Size = new System.Drawing.Size(130, 32);
            this.txtLastName.TabIndex = 4;
            // 
            // txtStudentID
            // 
            this.txtStudentID.Location = new System.Drawing.Point(14, 73);
            this.txtStudentID.Name = "txtStudentID";
            this.txtStudentID.Size = new System.Drawing.Size(38, 32);
            this.txtStudentID.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Century", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(461, 43);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(57, 21);
            this.label4.TabIndex = 2;
            this.label4.Text = "Class";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Century", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(297, 43);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 21);
            this.label3.TabIndex = 1;
            this.label3.Text = "LastName";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Century", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(15, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(103, 21);
            this.label2.TabIndex = 0;
            this.label2.Text = "StudentID";
            // 
            // tabExamRecords
            // 
            this.tabExamRecords.BackColor = System.Drawing.Color.MintCream;
            this.tabExamRecords.Controls.Add(this.printbtn);
            this.tabExamRecords.Controls.Add(this.btnSave);
            this.tabExamRecords.Controls.Add(this.btnLoadMarks);
            this.tabExamRecords.Controls.Add(this.dgvMarks);
            this.tabExamRecords.Location = new System.Drawing.Point(4, 30);
            this.tabExamRecords.Name = "tabExamRecords";
            this.tabExamRecords.Padding = new System.Windows.Forms.Padding(3);
            this.tabExamRecords.Size = new System.Drawing.Size(1079, 495);
            this.tabExamRecords.TabIndex = 1;
            this.tabExamRecords.Text = "Marks Entry";
            // 
            // printbtn
            // 
            this.printbtn.BackColor = System.Drawing.Color.Lime;
            this.printbtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.printbtn.Font = new System.Drawing.Font("Verdana", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.printbtn.Location = new System.Drawing.Point(75, 449);
            this.printbtn.Name = "printbtn";
            this.printbtn.Size = new System.Drawing.Size(156, 35);
            this.printbtn.TabIndex = 3;
            this.printbtn.Text = "Print marks";
            this.printbtn.UseVisualStyleBackColor = false;
            this.printbtn.Click += new System.EventHandler(this.printbtn_Click);
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.Aqua;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Verdana", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Location = new System.Drawing.Point(695, 449);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(155, 35);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "save Marks";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnLoadMarks
            // 
            this.btnLoadMarks.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btnLoadMarks.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoadMarks.Font = new System.Drawing.Font("Verdana", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLoadMarks.Location = new System.Drawing.Point(379, 449);
            this.btnLoadMarks.Name = "btnLoadMarks";
            this.btnLoadMarks.Size = new System.Drawing.Size(173, 35);
            this.btnLoadMarks.TabIndex = 1;
            this.btnLoadMarks.Text = "Load Marks";
            this.btnLoadMarks.UseVisualStyleBackColor = false;
            this.btnLoadMarks.Click += new System.EventHandler(this.btnLoadMarks_Click);
            // 
            // dgvMarks
            // 
            this.dgvMarks.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMarks.Location = new System.Drawing.Point(105, 23);
            this.dgvMarks.Name = "dgvMarks";
            this.dgvMarks.RowHeadersWidth = 51;
            this.dgvMarks.RowTemplate.Height = 24;
            this.dgvMarks.Size = new System.Drawing.Size(893, 399);
            this.dgvMarks.TabIndex = 0;
            this.dgvMarks.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvMarks_CellValueChanged);
            this.dgvMarks.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.DgvMarks_EditingControlShowing);
            // 
            // tabReportGen
            // 
            this.tabReportGen.BackColor = System.Drawing.Color.MintCream;
            this.tabReportGen.Controls.Add(this.textYear);
            this.tabReportGen.Controls.Add(this.label9);
            this.tabReportGen.Controls.Add(this.comboTerm);
            this.tabReportGen.Controls.Add(this.label10);
            this.tabReportGen.Controls.Add(this.comboClass);
            this.tabReportGen.Controls.Add(this.label11);
            this.tabReportGen.Controls.Add(this.txtStudent);
            this.tabReportGen.Controls.Add(this.label8);
            this.tabReportGen.Controls.Add(this.btnGen);
            this.tabReportGen.Controls.Add(this.rptGen);
            this.tabReportGen.Location = new System.Drawing.Point(4, 30);
            this.tabReportGen.Name = "tabReportGen";
            this.tabReportGen.Padding = new System.Windows.Forms.Padding(3);
            this.tabReportGen.Size = new System.Drawing.Size(1079, 495);
            this.tabReportGen.TabIndex = 2;
            this.tabReportGen.Text = "Report Generation";
            // 
            // textYear
            // 
            this.textYear.Location = new System.Drawing.Point(739, 17);
            this.textYear.Name = "textYear";
            this.textYear.Size = new System.Drawing.Size(123, 28);
            this.textYear.TabIndex = 15;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(666, 24);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(52, 21);
            this.label9.TabIndex = 14;
            this.label9.Text = "Year";
            // 
            // comboTerm
            // 
            this.comboTerm.FormattingEnabled = true;
            this.comboTerm.Location = new System.Drawing.Point(507, 16);
            this.comboTerm.Name = "comboTerm";
            this.comboTerm.Size = new System.Drawing.Size(106, 29);
            this.comboTerm.TabIndex = 13;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(435, 24);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(57, 21);
            this.label10.TabIndex = 12;
            this.label10.Text = "Term";
            // 
            // comboClass
            // 
            this.comboClass.FormattingEnabled = true;
            this.comboClass.Items.AddRange(new object[] {
            "PLAYGROUP",
            "PP1",
            "PP2",
            "GRADE 1",
            "GRADE 2",
            "GRADE 3",
            "GRADE 4",
            "GRADE 5",
            "GRADE 6",
            "GRADE 7",
            "GRADE 8",
            "GRADE 9"});
            this.comboClass.Location = new System.Drawing.Point(261, 16);
            this.comboClass.Name = "comboClass";
            this.comboClass.Size = new System.Drawing.Size(168, 29);
            this.comboClass.TabIndex = 11;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(198, 20);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(57, 21);
            this.label11.TabIndex = 10;
            this.label11.Text = "Class";
            // 
            // txtStudent
            // 
            this.txtStudent.Location = new System.Drawing.Point(115, 17);
            this.txtStudent.Name = "txtStudent";
            this.txtStudent.Size = new System.Drawing.Size(52, 28);
            this.txtStudent.TabIndex = 5;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 18);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(103, 21);
            this.label8.TabIndex = 4;
            this.label8.Text = "StudentID";
            // 
            // btnGen
            // 
            this.btnGen.BackColor = System.Drawing.Color.SpringGreen;
            this.btnGen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGen.Font = new System.Drawing.Font("Verdana", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGen.Location = new System.Drawing.Point(375, 64);
            this.btnGen.Name = "btnGen";
            this.btnGen.Size = new System.Drawing.Size(205, 47);
            this.btnGen.TabIndex = 1;
            this.btnGen.Text = "Generate Report";
            this.btnGen.UseVisualStyleBackColor = false;
            this.btnGen.Click += new System.EventHandler(this.btnGen_Click);
            // 
            // rptGen
            // 
            this.rptGen.Location = new System.Drawing.Point(56, 136);
            this.rptGen.Name = "rptGen";
            this.rptGen.ServerReport.BearerToken = null;
            this.rptGen.Size = new System.Drawing.Size(993, 336);
            this.rptGen.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.MintCream;
            this.tabPage1.Controls.Add(this.btnToExcel);
            this.tabPage1.Controls.Add(this.btnRanking);
            this.tabPage1.Controls.Add(this.dgvRanking);
            this.tabPage1.Location = new System.Drawing.Point(4, 30);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1079, 495);
            this.tabPage1.TabIndex = 3;
            this.tabPage1.Text = "Ranking";
            // 
            // btnToExcel
            // 
            this.btnToExcel.BackColor = System.Drawing.Color.Black;
            this.btnToExcel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToExcel.Font = new System.Drawing.Font("Verdana", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnToExcel.ForeColor = System.Drawing.Color.White;
            this.btnToExcel.Location = new System.Drawing.Point(591, 27);
            this.btnToExcel.Name = "btnToExcel";
            this.btnToExcel.Size = new System.Drawing.Size(205, 34);
            this.btnToExcel.TabIndex = 3;
            this.btnToExcel.Text = "Export To Excel";
            this.btnToExcel.UseVisualStyleBackColor = false;
            this.btnToExcel.Click += new System.EventHandler(this.btnToExcel_Click);
            // 
            // btnRanking
            // 
            this.btnRanking.BackColor = System.Drawing.Color.Black;
            this.btnRanking.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRanking.Font = new System.Drawing.Font("Verdana", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRanking.ForeColor = System.Drawing.Color.White;
            this.btnRanking.Location = new System.Drawing.Point(362, 27);
            this.btnRanking.Name = "btnRanking";
            this.btnRanking.Size = new System.Drawing.Size(205, 34);
            this.btnRanking.TabIndex = 2;
            this.btnRanking.Text = "Generate Ranking";
            this.btnRanking.UseVisualStyleBackColor = false;
            this.btnRanking.Click += new System.EventHandler(this.btnRanking_Click);
            // 
            // dgvRanking
            // 
            this.dgvRanking.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRanking.Location = new System.Drawing.Point(20, 85);
            this.dgvRanking.Name = "dgvRanking";
            this.dgvRanking.RowHeadersWidth = 51;
            this.dgvRanking.RowTemplate.Height = 24;
            this.dgvRanking.Size = new System.Drawing.Size(1043, 336);
            this.dgvRanking.TabIndex = 0;
            // 
            // Exam_Management
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.MintCream;
            this.ClientSize = new System.Drawing.Size(1276, 656);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Exam_Management";
            this.Text = "Exam_Management";
            this.Load += new System.EventHandler(this.Exam_Management_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabMarksEntry.ResumeLayout(false);
            this.grpEnterMarks.ResumeLayout(false);
            this.grpEnterMarks.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.examDGV)).EndInit();
            this.tabExamRecords.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMarks)).EndInit();
            this.tabReportGen.ResumeLayout(false);
            this.tabReportGen.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRanking)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabMarksEntry;
        private System.Windows.Forms.TabPage tabExamRecords;
        private System.Windows.Forms.TabPage tabReportGen;
        private System.Windows.Forms.DataGridView dgvMarks;
        private Microsoft.Reporting.WinForms.ReportViewer rptGen;
        private System.Windows.Forms.GroupBox grpEnterMarks;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbClass;
        private System.Windows.Forms.TextBox txtLastName;
        private System.Windows.Forms.TextBox txtStudentID;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtYear;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cmbTerm;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnLoadMarks;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.DataGridView dgvRanking;
        private System.Windows.Forms.Button btnGen;
        private System.Windows.Forms.Button btnRanking;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnclear;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnToExcel;
        private System.Windows.Forms.TextBox textYear;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox comboTerm;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox comboClass;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtStudent;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button printbtn;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ComboBox cmbExamType;
        private System.Windows.Forms.DataGridView examDGV;
        private System.Windows.Forms.ComboBox FirstNameComboBox;
    }
}