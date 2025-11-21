Contract Monthly Claim System (CMCS)

A modern, full-featured ASP.NET Core MVC (.NET 8) web application built to streamline the monthly claim submission and approval workflow for Independent Contractor (IC) Lecturers. CMCS provides a complete, automated process from claim submission → approval → payroll, with dashboards and tools for Coordinators, Managers, and HR.

The system uses Session-based Authentication, Role-based Access, JSON File Repositories, QuestPDF Payroll Generation, and a modern Glass UI interface.

📌 Table of Contents

Overview

Key Features

System Roles

Automation Policies

Tech Stack

Project Structure

Getting Started

Screenshots

Future Enhancements

🧾 Overview

CMCS enables Independent Contractor Lecturers to easily submit their monthly claims (hours worked × hourly rate), upload supporting documents, and track the approval progress.

The system then routes claims through a structured workflow:

Lecturer → Coordinator → Manager → HR → Payroll Output

Every role receives its own personalized dashboard, advanced automation rules, and streamlined tools to ensure accuracy, transparency, and efficiency.

🚀 Key Features
✔ Authentication & Roles

Secure Email/Password login

Register with one of four roles:

Lecturer

Coordinator

Manager

HR

Role-based navigation and access control (Session-based)

✔ Lecturer Features

Submit monthly claim

Hours worked

Hourly rate

Notes

Document upload

Auto-calculated total amount

Track all claim statuses:

Pending

Approved

Rejected

View warnings for missing documents or policy issues

✔ Coordinator Features

View ALL lecturer claims in their department

Approve / Reject directly from the dashboard

See automatic policy warnings:

Missing documents

High payout

Suspicious values

Claim statistics summary:

Total

Pending

Approved

Rejected

✔ Manager Features

Institution-wide oversight

Review and approve/reject claims after Coordinator

View all lecturer submissions

Approval audit trail included:

ApprovedBy

ApprovedDate

✔ HR Features

Access only Approved claims

Manage Lecturer Records (Add, Edit, Remove)

Generate payroll:

CSV Export

PDF Payslip Export (QuestPDF)

Payroll summary dashboard

View analytics dashboard:

Approval trends

Lecturer performance

Status breakdown charts

🤖 Automation Policies

CMCS includes built-in automated validation using ClaimPolicyService:

Auto-Reject Conditions

HoursWorked > 300

HourlyRate outside R100–R1000

Duplicate claim submitted in the same month

Auto-Flag Warnings

Missing supporting document

Large payout (TotalAmount > R15,000)

These warnings appear to Coordinators/Managers during review.

🧱 Tech Stack

.NET 8 (ASP.NET Core MVC)

C#

Bootstrap 5

QuestPDF (PDF payroll generation)

JSON-based repositories (users.json, claims.json, payroll.json)

Session-based authentication

xUnit (unit testing)

📂 Project Structure
ContractClaim/
│
├── Controllers/
│   ├── AccountController.cs
│   ├── LecturerController.cs
│   ├── CoordinatorController.cs
│   ├── ManagerController.cs
│   ├── HRController.cs
│   ├── PayrollController.cs
│   ├── AnalyticsController.cs
│
├── Data/
│   ├── UserRepository.cs
│   ├── ClaimRepository.cs
│   ├── PayrollRepository.cs
│
├── Models/
│   ├── User.cs
│   ├── Claim.cs
│   ├── PayrollRecord.cs
│   ├── AnalyticsModels.cs
│
├── Services/
│   ├── ClaimPolicyService.cs
│   ├── PayrollService.cs
│
├── Views/
│   ├── Account/
│   ├── Lecturer/
│   ├── Coordinator/
│   ├── Manager/
│   ├── HR/
│   ├── Payroll/
│   ├── Analytics/
│   └── Shared/
│
├── wwwroot/
│   ├── css/
│   ├── js/
│   └── uploads/
│
├── App_Data/
│   ├── users.json
│   ├── claims.json
│   ├── payroll.json
│
└── ContractClaim.Tests/
    ├── Repositories/
    ├── Controllers/

💻 Getting Started
1. Prerequisites

Install .NET 8 SDK

Visual Studio / VS Code / Rider

2. Clone the Repository
git clone https://github.com/your-repo/contract_claim.git
cd contract_claim

3. Restore Dependencies
dotnet restore

4. Run the Application
dotnet run


Then open your browser:

➡ http://localhost:5000

or
➡ https://localhost:7000

🖼 Screenshots (Recommended Placement)

Include your screenshots in this order:

Landing Page

Lecturer Dashboard

Submit Claim Page

Track Claims Page

Coordinator Dashboard

Coordinator Review Table

Manager Dashboard

HR Dashboard

Analytics Dashboard

Payroll PDF/CSV Output

🔮 Future Enhancements

Email notifications on approval/rejection

Full database migration (SQL Server)

Azure deployment

Audit logging

Multi-department management

Notification bell UI
