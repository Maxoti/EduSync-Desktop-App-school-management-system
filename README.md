EduSync School Management System — Desktop Edition

EduSync Desktop is a robust school management system developed by Engineer Maxwell Onyango using C# and Windows Forms and Sql Server.
It is designed to automate school operations including student management, fee tracking, reporting, and role-based access control — all in a lightweight, offline-capable desktop environment.

 Core Features
1. Administration

Manage student, teacher, and class records.

Control user roles and authentication.

Track school activities from a unified dashboard.

2. Finance & Fees

Automate fee collection and balance calculations.

Generate fee reports and payment receipts.

Supports local currency transactions.

3. Academic Management

Record and analyze student performance.

Generate and print exam reports via RDLC Reports.

Maintain class-specific timetables and subjects.

4.Security & Access

Role-based login system (Admin, Teacher, Parent).

Password hashing using bcrypt.

Input validation and SQL Injection protection.

 TECHNOLOGY STACK
Layer	Technology
Frontend	C# WinForms
Backend	ADO.NET + SQL Server
Reporting	RDLC Reports
Database	SQL Server
Language	C# (.NET Framework)

*** Installation Guide

Clone the repository

git clone https://github.com/Maxoti/EduSync-Desktop-App.git


Open in Visual Studio

Go to File > Open > Project/Solution

Select the .sln file inside the EduSync folder.

Restore Dependencies

Right-click the solution → “Restore NuGet Packages”.

Database Setup

Open SQL Server Management Studio (SSMS).

Create a new database called EduSyncDB.

Run the SQL script provided in the project folder (if available).

Configure Connection

Open App.config.

Edit the connection string:

<connectionStrings>
    <add name="EduSyncDB" 
         connectionString="Data Source=YOUR_SERVER_NAME;Initial Catalog=EduSyncDB;Integrated Security=True" 
         providerName="System.Data.SqlClient" />
</connectionStrings>


Run the Project

Press F5 or click Start Debugging.

5.Reporting

EduSync Desktop uses RDLC reports for:

Fee summaries

Attendance reports

Student performance charts

Each report can be printed or exported directly from the system.

** Highlights

Clean, intuitive user interface.

Lightweight — runs offline without internet dependency.

Modular structure — easily expandable with more features.

** Future Enhancements

Integration with M-Pesa STK Push (Daraja API).

Real-time data sync with EduSync Web version.

SMS and Email notifications for parents.

DEVELOPER-------

Engineer Maxwell Onyango
Software Engineer & System Designer
📍 Nairobi, Kenya
💼 GitHub Profile
