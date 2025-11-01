using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EduSync
{
    public partial class Dashboard : Form
    {
        public Dashboard()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            LoadForm(new Student());

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            LoadForm(new Teacherbtn());
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            LoadForm(new txtAttend());
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            LoadForm(new FeeManager());
        } 

        private void btnStudent_Click(object sender, EventArgs e)
        {
           
        }

        private void btnTeacher_Click(object sender, EventArgs e)
        {
           
        }

        private void btnFee_Click(object sender, EventArgs e)
        {

          
        }

        private void btnAttendance_Click(object sender, EventArgs e)
        {
           
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
        }
        private void LoadForm(Form form)
        {
            // Clear previous form
            MainPanel.Controls.Clear();

            // Prepare the form
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;

            // Load into panel
            MainPanel.Controls.Add(form);
            MainPanel.Tag = form;
            form.Show();
        }



        private void label4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnTeacherPerform_Click(object sender, EventArgs e)
        {



        }

        private void btnStudent_Click_1(object sender, EventArgs e)
        {
            LoadForm(new Student());
        }

        private void btnTeacher_Click_1(object sender, EventArgs e)
        {
           
            LoadForm(new Teacherbtn());

        }

        private void btnFee_Click_1(object sender, EventArgs e)
        {
           
            LoadForm(new FeeManager());

        }

        private void btnAttendance_Click_1(object sender, EventArgs e)
        {
            LoadForm(new txtAttend());

        }

        private void btnReport_Click_1(object sender, EventArgs e)
        {


        }

        private void btnPerformance_Click(object sender, EventArgs e)
        {
           


        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {

        }

        private void Dashboard_Load(object sender, EventArgs e)
        {
        
            string role = log.LoggedInRole;

            if (role == "Admin")
            {
                // Admin sees everything
                picFee.Enabled = true;
                picTeacher.Enabled = true;
                picExam.Enabled = true;
                picStudent.Enabled = true;
                picAttendance.Enabled = true;

                // ✅ Show Backup/Restore module for Admin
                picSettings.Enabled = true;
                picSettings.Visible = true;
            }
            else if (role == "Teacher")
            {
                picFee.Enabled = false;
                picFee.Visible = false;
                picTeacher.Visible = false;

                picExam.Enabled = true;
                picStudent.Enabled = true;
                picAttendance.Enabled = true;

                // ❌ Hide Backup/Restore
                picSettings.Enabled = false;
                picSettings.Visible = false;
            }
            else if (role == "Accountant")
            {
                picFee.Enabled = true;

                picExam.Enabled = false;
                picStudent.Enabled = false;
                picAttendance.Enabled = false;

                picExam.Visible = false;
                picStudent.Visible = false;
                picAttendance.Visible = false;

                // ❌ Hide Backup/Restore
                picSettings.Enabled = false;
                picSettings.Visible = false;
            }
            else
            {
                // Unknown role → lock everything
                picFee.Enabled = false;
                picExam.Enabled = false;
                picStudent.Enabled = false;
                picAttendance.Enabled = false;

                // ❌ Hide Backup/Restore
                picSettings.Enabled = false;
                picSettings.Visible = false;
            }
        }

         
        

                


        

        private void MainPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnTrMan_Click(object sender, EventArgs e)
        {
            LoadForm(new Teacherbtn());
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnExam_Click(object sender, EventArgs e)
        {
            LoadForm(new Exam_Management());
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
           
            DialogResult result = MessageBox.Show("Are you sure you want to exit?", "Exit Edusync", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if(result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void btnPerform_Click(object sender, EventArgs e)
        {
        }

        private void picExam_Click(object sender, EventArgs e)
        {
            LoadForm(new Exam_Management());
        }

        private void picSettings_Click(object sender, EventArgs e)
        {
            LoadForm(new DatabaseMaintenanceForm());

        }
    } 
}
