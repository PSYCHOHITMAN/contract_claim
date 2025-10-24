# contract_claim
# Contract Monthly Claim System (CMCS)

## Overview
The **Contract Monthly Claim System (CMCS)** is a .NET 8 MVC web application designed for **Independent Contractor (IC) Lecturers** to submit and track their monthly claims (hours worked × hourly rate). It also provides role-based dashboards for **Coordinators** and **Managers** to approve or reject submitted claims.

This project demonstrates **ASP.NET Core MVC**, **Session-based authentication**, **role-based navigation**, and **file persistence using JSON repositories**.  

---

## Features

###  Authentication & Roles
- Register and Login with **Email/Password**
- Role selection during registration:  
  - **Lecturer**  
  - **Coordinator**  
  - **Manager**
- Session-based login system

### Lecturer Dashboard
- Welcome page with quick navigation
- **Submit Claim**: Enter hours, rate, notes, and upload supporting documents
- **Track Claims**: View claim history, status (Pending, Approved, Rejected), delete individual claims, or clear all claims

### Coordinator Dashboard
- View all submitted claims
- Approve/Reject claims with one click
- Stats summary: Total, Pending, Approved, Rejected

### Manager Dashboard
- View all claims across lecturers
- Approve/Reject claims
- Stats summary and reporting

### Document Upload
- Upload supporting documents (`.pdf`, `.docx`, `.xlsx`)
- Files saved in `wwwroot/uploads`

### Data Persistence
- Users stored in `App_Data/users.json`
- Claims stored in `App_Data/claims.json`

###  Unit Testing
- xUnit test project for repositories and controllers
- Fake session implementation for controller tests

---

## Tech Stack
- **.NET 8**
- **ASP.NET Core MVC**
- **C#**
- **Bootstrap 5** (UI styling)
- **Session-based authentication**
- **xUnit** (unit testing)

---

## Project Structure
ContractClaim/
│
├── Controllers/
│ ├── AccountController.cs
│ ├── LecturerController.cs
│ ├── CoordinatorController.cs
│ ├── ManagerController.cs
│
├── Data/
│ ├── UserRepository.cs
│ ├── ClaimRepository.cs
│
├── Models/
│ ├── User.cs
│ ├── Claim.cs
│
├── Views/
│ ├── Account/
│ ├── Lecturer/
│ ├── Coordinator/
│ ├── Manager/
│ └── Shared/
│
├── wwwroot/
│ └── uploads/
│
├── App_Data/
│ ├── users.json
│ ├── claims.json
│
└── ContractClaim.Tests/ (xUnit test project)

---

## Getting Started

### Prerequisites
- Install [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- Any IDE (Visual Studio, Rider, or VS Code)

### Run Locally
1. Clone the repository:
   ```bash
   git clone https://github.com/your-repo/contract-claim-system.git
   cd contract-claim-system
