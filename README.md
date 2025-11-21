Contract Monthly Claim System (CMCS)

A modern, full-featured ASP.NET Core MVC (.NET 8) web application built to streamline the monthly claim submission and approval workflow for Independent Contractor (IC) Lecturers.
CMCS delivers a complete automated process:

Claim Submission → Review → Approval → Payroll Summary → PDF/CSV Export

The system includes Session-based Authentication, Role-based Access Control, JSON File Repositories, Automation Rules, QuestPDF payroll generation, and a modern Glass-UI interface.

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

The Contract Monthly Claim System (CMCS) allows Independent Contractor Lecturers to quickly:

Submit their monthly claims (hours × rate)

Upload supporting documents

Track approval progress through each stage

The workflow is structured and transparent:

Lecturer → Coordinator → Manager → HR → Payroll Output

Each role receives:

✔ A personalized dashboard
✔ Role-specific tools
✔ Automation rules
✔ Accurate tracking and full visibility

🚀 Key Features
✔ Authentication & Roles

Secure Email/Password login

Register using one of four roles:

Lecturer

Coordinator

Manager

HR

Session-based authentication

Smart role-based navigation

✔ Lecturer Features

Submit a monthly claim:

Hours worked

Hourly rate

Notes

Supporting document upload

Auto-calculated total amount

Track all claim statuses:

Pending

Approved

Rejected

Warnings displayed for:

Missing documents

Unusual values

✔ Coordinator Features

Manage all lecturer claims in their department

Approve / Reject with one click

Automated policy checks:

Missing document flag

High payout warning

Duplicate monthly claim detection

Dashboard summaries:

Total claims

Pending

Approved

Rejected

✔ Manager Features

Full institution-wide claim visibility

Second-level approval after Coordinator

Detailed audit trail:

ApprovedBy

ApprovedDate

Access to all lecturer submissions

✔ HR Features

Access to all Approved claims

Lecturer Management

Add

Edit

Remove

Payroll Generation

Export CSV

Export PDF (QuestPDF)

Analytics Dashboard:

Approval rate trends

Payment statistics

Performance charts

🤖 Automation Policies

CMCS includes automated validation through ClaimPolicyService:

Auto-Reject Rules

❌ HoursWorked > 300
❌ Hourly Rate outside R100 – R1000
❌ Duplicate claim within the same month

Auto-Flag Warnings

⚠ Missing supporting document
⚠ TotalAmount over R15,000 (large payout)

Warnings appear on the Coordinator and Manager review screens.

🧱 Tech Stack

.NET 8

ASP.NET Core MVC

C#

Bootstrap 5 (UI)

QuestPDF (PDF payroll generation)

JSON-based persistence:

users.json

claims.json

payroll.json

Session-based authentication

xUnit tests

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
1️⃣ Prerequisites

Install .NET 8 SDK

Use Visual Studio, Rider, or VS Code

2️⃣ Clone the Repository
git clone https://github.com/your-repo/contract_claim.git
cd contract_claim

3️⃣ Restore Dependencies
dotnet restore

4️⃣ Run the Application
dotnet run


Then open:

➡ http://localhost:5000

or
➡ https://localhost:7000

🖼 Screenshots (Recommended Order)

Paste these into your README or PowerPoint:

Landing Page

Lecturer Dashboard

Submit Claim Page

Track Claims Page

Coordinator Dashboard

Coordinator Review Table (Policy Warnings Visible)

Manager Dashboard

HR Dashboard

Analytics Dashboard

Payroll Summary + CSV + PDF Output

🔮 Future Enhancements

📧 Email notifications for approvals

🗄 SQL Database migration (replace JSON)

☁ Azure deployment

🧾 Full audit logging

🏫 Multi-department management

🔔 Notification bell UI
