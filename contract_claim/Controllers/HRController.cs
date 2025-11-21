using Microsoft.AspNetCore.Mvc;
using contract_claim.Data;
using contract_claim.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Text;

namespace contract_claim.Controllers
{
    public class HRController : BaseController
    {
        private IActionResult CheckRole()
        {
            var redirect = RedirectIfNotRole("HR");
            return redirect ?? null;
        }

        private List<Claim> GetApprovedClaims(int year, int month)
        {
            return ClaimRepository.GetAll()
                .Where(c =>
                    c.Status == "Approved" &&
                    c.ApprovedDate.HasValue &&
                    c.ApprovedDate.Value.Year == year &&
                    c.ApprovedDate.Value.Month == month)
                .ToList();
        }

        
        public IActionResult Index()
        {
            var redirect = CheckRole();
            if (redirect != null) return redirect;

            
            ViewBag.DisplayName = HttpContext.Session.GetString("username") ?? "HR";

            var claims = ClaimRepository.GetAll();

            var approved = claims.Where(c => c.Status == "Approved").ToList();
            var pending = claims.Where(c => c.Status == "Pending").ToList();
            var rejected = claims.Where(c => c.Status == "Rejected").ToList();

            ViewBag.TotalLecturers = UserRepository.GetAll().Count(u => u.Role == "Lecturer");
            ViewBag.TotalApproved = approved.Count;
            ViewBag.TotalPayout = approved.Sum(c => c.TotalAmount);

            
            ViewBag.TotalApprovedAmount = approved.Sum(c => c.TotalAmount);
            ViewBag.TotalApprovedCount = approved.Count;
            ViewBag.PendingCount = pending.Count;
            ViewBag.RejectedCount = rejected.Count;

            return View(claims);
        }

        public IActionResult Lecturers()
        {
            var redirect = CheckRole();
            if (redirect != null) return redirect;

            var lecturers = UserRepository.GetAll()
                .Where(u => u.Role == "Lecturer")
                .ToList();

            return View(lecturers);
        }

        // ---------------------------
        // Individual Payroll Slip PDF
        // ---------------------------
        public IActionResult Slip(string email)
        {
            var redirect = CheckRole();
            if (redirect != null) return redirect;

            var claims = ClaimRepository.GetAll()
                .Where(c => c.LecturerName == email && c.Status == "Approved")
                .ToList();

            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.A4);

                    page.Header()
                        .Text($"Payroll Slip - {email}")
                        .FontSize(20)
                        .Bold();

                    page.Content().Column(col =>
                    {
                        foreach (var claim in claims)
                        {
                            col.Item().Text(
                                $"Hours: {claim.HoursWorked}, Rate: R{claim.HourlyRate}, Total: R{claim.TotalAmount}"
                            );
                        }
                    });
                });
            });

            var pdfBytes = doc.GeneratePdf();
            return File(pdfBytes, "application/pdf", $"{email}_PayrollSlip.pdf");
        }

        
        [HttpGet]
        public IActionResult Payroll()
        {
            var redirect = CheckRole();
            if (redirect != null) return redirect;

            return View();
        }

        [HttpPost]
        public IActionResult Payroll(int year, int month)
        {
            var redirect = CheckRole();
            if (redirect != null) return redirect;

            var claims = GetApprovedClaims(year, month);

            ViewBag.Year = year;
            ViewBag.Month = month;
            ViewBag.Total = claims.Sum(c => c.TotalAmount);

            return View(claims);
        }

      
        [HttpGet]
        public IActionResult AddLecturer()
        {
            var redirect = CheckRole();
            if (redirect != null) return redirect;

            return View();
        }

        
        [HttpPost]
        public IActionResult AddLecturer(string Username, string Email, decimal? Rate)
        {
            var redirect = CheckRole();
            if (redirect != null) return redirect;

            if (UserRepository.Exists(Email))
            {
                TempData["Error"] = "A lecturer with this email already exists.";
                return RedirectToAction("Lecturers");
            }

            var newUser = new User
            {
                Username = Username,
                Email = Email,
                Password = "12345",        // Default temp password
                Role = "Lecturer",
                HourlyRate = Rate ?? 0
            };

            UserRepository.Add(newUser);

            TempData["Message"] = "Lecturer added successfully!";
            return RedirectToAction("Lecturers");
        }

        [HttpGet]
        public IActionResult EditLecturer(int id)
        {
            var redirect = CheckRole();
            if (redirect != null) return redirect;

            var lecturer = UserRepository.GetById(id);
            if (lecturer == null) return NotFound();

            return View(lecturer);
        }

        
        [HttpPost]
        public IActionResult EditLecturer(int id, string Username, string Email, decimal HourlyRate)
        {
            var redirect = CheckRole();
            if (redirect != null) return redirect;

            var lecturer = UserRepository.GetById(id);
            if (lecturer == null) return NotFound();

            lecturer.Username = Username;
            lecturer.Email = Email;
            lecturer.HourlyRate = HourlyRate;

            UserRepository.Update(lecturer);

            TempData["Message"] = "Lecturer information updated!";
            return RedirectToAction("Lecturers");
        }

        
        [HttpPost]
        public IActionResult DeleteLecturer(int id)
        {
            var redirect = CheckRole();
            if (redirect != null) return redirect;

            UserRepository.Delete(id);

            TempData["Message"] = "Lecturer removed successfully.";
            return RedirectToAction("Lecturers");
        }



        public IActionResult Analytics(int? year, int? month)
        {
            var redirect = CheckRole();
            if (redirect != null) return redirect;

            var claims = ClaimRepository.GetAll();

            int selectedYear = year ?? DateTime.Now.Year;
            int selectedMonth = month ?? 0;

            claims = claims.Where(c =>
                c.ApprovedDate.HasValue &&
                c.ApprovedDate.Value.Year == selectedYear
            ).ToList();

            if (selectedMonth > 0)
            {
                claims = claims.Where(c =>
                    c.ApprovedDate.HasValue &&
                    c.ApprovedDate.Value.Month == selectedMonth
                ).ToList();
            }

            ViewBag.Approved = claims.Count(c => c.Status == "Approved");
            ViewBag.Pending = claims.Count(c => c.Status == "Pending");
            ViewBag.Rejected = claims.Count(c => c.Status == "Rejected");

            ViewBag.TotalPayout = claims
                .Where(c => c.Status == "Approved")
                .Sum(c => c.TotalAmount);

            ViewBag.Year = selectedYear;
            ViewBag.Month = selectedMonth;

            return View(claims);
        }

        public IActionResult ExportPayrollPdf(int year, int month)
        {
            var redirect = CheckRole();
            if (redirect != null) return redirect;

            var claims = GetApprovedClaims(year, month);

            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(25);
                    page.Size(PageSizes.A4);

                    page.Header()
                        .Text($"Payroll Report - {month}/{year}")
                        .FontSize(18)
                        .Bold();

                    page.Content().Column(col =>
                    {
                        foreach (var claim in claims)
                        {
                            col.Item().Text(
                                $"{claim.LecturerName} - Hours: {claim.HoursWorked}, Rate: R{claim.HourlyRate}, Total: R{claim.TotalAmount}"
                            );
                        }
                    });
                });
            });

            var pdfBytes = doc.GeneratePdf();
            return File(pdfBytes, "application/pdf", $"Payroll_{year}_{month}.pdf");
        }

        public IActionResult ExportApproved()
        {
            var redirect = CheckRole();
            if (redirect != null) return redirect;

            var claims = ClaimRepository.GetAll()
                .Where(c => c.Status == "Approved")
                .ToList();

            var sb = new StringBuilder();
            sb.AppendLine("Lecturer,Hours,Rate,Total,ApprovedBy,ApprovedDate");

            foreach (var c in claims)
            {
                sb.AppendLine(
                    $"{c.LecturerName},{c.HoursWorked},{c.HourlyRate},{c.TotalAmount},{c.ApprovedBy},{c.ApprovedDate}"
                );
            }

            return File(
                Encoding.UTF8.GetBytes(sb.ToString()),
                "text/csv",
                $"HR_Payroll_{DateTime.Now:yyyy_MM_dd}.csv"
            );
        }
    }
}