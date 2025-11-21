using contract_claim.Data;
using contract_claim.Models;
using contract_claim.Services;
using Microsoft.AspNetCore.Mvc;

namespace contract_claim.Controllers
{
    public class CoordinatorController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ClaimsList()
        {
            var redirect = RedirectIfNotRole("Coordinator");
            if (redirect != null) return redirect;

            var claims = ClaimRepository.GetAll();
            return View(claims);
        }

        [HttpPost]
        public IActionResult Approve(int id)
        {
            var claims = ClaimRepository.GetAll();
            var claim = claims.FirstOrDefault(c => c.Id == id);

            if (claim == null) return NotFound();

            // RUN POLICY CHECKS
            var policy = ClaimPolicyService.ValidateClaim(claim, claims);

            // AUTOMATIC REJECTION
            if (policy.AutoReject)
            {
                claim.Status = "Rejected";
                claim.ApprovedBy = HttpContext.Session.GetString("Username");
                claim.ApprovedDate = DateTime.Now;
                ClaimRepository.SaveAll(claims);

                TempData["Message"] = "⛔ Claim automatically rejected due to policy violation.";
                return RedirectToAction("ClaimsList");
            }

            // OTHERWISE APPROVE
            claim.Status = "Approved";
            claim.ApprovedBy = HttpContext.Session.GetString("Username");
            claim.ApprovedDate = DateTime.Now;
            ClaimRepository.SaveAll(claims);

            // WARNINGS
            if (policy.Warnings.Any())
            {
                TempData["Message"] = "⚠️ Claim approved, but with warnings: " +
                                       string.Join("; ", policy.Warnings);
            }
            else
            {
                TempData["Message"] = "✔ Claim approved successfully.";
            }

            return RedirectToAction("ClaimsList");
        }


        [HttpPost]
        public IActionResult Reject(int id)
        {
            var claims = ClaimRepository.GetAll();
            var claim = claims.FirstOrDefault(c => c.Id == id);

            if (claim == null) return NotFound();

            claim.Status = "Rejected";
            claim.ApprovedBy = HttpContext.Session.GetString("Username");
            claim.ApprovedDate = DateTime.Now;

            ClaimRepository.SaveAll(claims);

            TempData["Message"] = "Claim manually rejected.";
            return RedirectToAction("ClaimsList");
        }
    }
}
