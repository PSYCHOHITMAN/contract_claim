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

        // ---------------------------
        // HR MAIN DASHBOARD (Index)
        // ---------------------------
        public IActionResult Index()
        {
            var redirect = CheckRole();
            if (redirect != null) return redirect;

            // Logged-in HR name
            ViewBag.DisplayName = HttpContext.Session.GetString("username") ?? "HR";

            var claims = ClaimRepository.GetAll();

            var approved = claims.Where(c => c.Status == "Approved").ToList();
            var pending = claims.Where(c => c.Status == "Pending").ToList();
            var rejected = claims.Where(c => c.Status == "Rejected").ToList();

            ViewBag.TotalLecturers = UserRepository.GetAll().Count(u => u.Role == "Lecturer");
            ViewBag.TotalApproved = approved.Count;
            ViewBag.TotalPayout = approved.Sum(c => c.TotalAmount);

            // For table
            ViewBag.TotalApprovedAmount = approved.Sum(c => c.TotalAmount);
            ViewBag.TotalApprovedCount = approved.Count;
            ViewBag.PendingCount = pending.Count;
            ViewBag.RejectedCount = rejected.Count;

            return View(claims);
        }

        // ---------------------------
        // Lecturers List
        // ---------------------------
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

        // ---------------------------
        // Payroll
        // ---------------------------
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

        // ---------------------------
        // Analytics
        // ---------------------------
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

        // ---------------------------
        // EXPORT PDF
        // ---------------------------
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

        // ---------------------------
        // EXPORT CSV
        // ---------------------------
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