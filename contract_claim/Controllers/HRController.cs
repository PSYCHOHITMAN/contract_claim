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
        // Individual Payroll Slip
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
                        col.Item().Text(" ");
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
        // HR Dashboard Overview
        // ---------------------------
        public IActionResult Index()
        {
            var redirect = CheckRole();
            if (redirect != null) return redirect;

            var allClaims = ClaimRepository.GetAll();

            var approved = allClaims.Where(c => c.Status == "Approved").ToList();
            var pending = allClaims.Where(c => c.Status == "Pending").ToList();
            var rejected = allClaims.Where(c => c.Status == "Rejected").ToList();

            ViewBag.TotalApprovedAmount = approved.Sum(c => c.TotalAmount);
            ViewBag.TotalApprovedCount = approved.Count;
            ViewBag.PendingCount = pending.Count;
            ViewBag.RejectedCount = rejected.Count;

            return View(allClaims);
        }

        // ---------------------------
        // Dashboard Summary
        // ---------------------------
        public IActionResult Dashboard()
        {
            var redirect = CheckRole();
            if (redirect != null) return redirect;

            var claims = ClaimRepository.GetAll();
            var approved = claims.Where(c => c.Status == "Approved").ToList();

            ViewBag.TotalLecturers = UserRepository.GetAll()
                .Count(u => u.Role == "Lecturer");

            ViewBag.TotalApproved = approved.Count;
            ViewBag.TotalPayout = approved.Sum(c => c.TotalAmount);

            return View();
        }

        // ---------------------------
        // Payroll Page
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
            var redirect = RedirectIfNotRole("HR");
            if (redirect != null) return redirect;

            var claims = ClaimRepository.GetAll();

            // Determine the filters being used
            int selectedYear = year ?? DateTime.Now.Year;
            int selectedMonth = month ?? 0; // 0 = all months

            // Apply YEAR filter
            claims = claims.Where(c =>
                c.ApprovedDate.HasValue &&
                c.ApprovedDate.Value.Year == selectedYear
            ).ToList();

            // Apply MONTH filter only if user selected one
            if (selectedMonth > 0)
            {
                claims = claims.Where(c =>
                    c.ApprovedDate.HasValue &&
                    c.ApprovedDate.Value.Month == selectedMonth
                ).ToList();
            }

            // Stats
            ViewBag.Approved = claims.Count(c => c.Status == "Approved");
            ViewBag.Pending = claims.Count(c => c.Status == "Pending");
            ViewBag.Rejected = claims.Count(c => c.Status == "Rejected");

            ViewBag.TotalPayout = claims
                .Where(c => c.Status == "Approved")
                .Sum(c => c.TotalAmount);

            // Pass filters back so UI stays on selected filters
            ViewBag.Year = selectedYear;
            ViewBag.Month = selectedMonth;

            return View(claims);
        }

        // ---------------------------
        // Export Payroll PDF
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
        // Export CSV (Approved Claims)
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