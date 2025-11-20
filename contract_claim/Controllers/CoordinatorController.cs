using contract_claim.Data;
using contract_claim.Models;
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
            var redirect = RedirectIfNotRole("Coordinator");
            if (redirect != null) return redirect;

            var claims = ClaimRepository.GetAll();
            var claim = claims.FirstOrDefault(c => c.Id == id);

            if (claim != null)
            {
                if (claim.Status == "Approved")
                {
                    TempData["Message"] = "This claim is already approved.";
                    return RedirectToAction("ClaimsList");
                }

                claim.Status = "Approved";
                claim.ApprovedBy = HttpContext.Session.GetString("Username");
                claim.ApprovedDate = DateTime.Now;

                ClaimRepository.SaveAll(claims);
            }

            TempData["Message"] = "Claim approved.";
            return RedirectToAction("ClaimsList");
        }

        [HttpPost]
        public IActionResult Reject(int id)
        {
            var redirect = RedirectIfNotRole("Coordinator");
            if (redirect != null) return redirect;

            var claims = ClaimRepository.GetAll();
            var claim = claims.FirstOrDefault(c => c.Id == id);

            if (claim != null)
            {
                if (claim.Status == "Rejected")
                {
                    TempData["Message"] = "This claim is already rejected.";
                    return RedirectToAction("ClaimsList");
                }

                claim.Status = "Rejected";
                claim.ApprovedBy = HttpContext.Session.GetString("Username");
                claim.ApprovedDate = DateTime.Now;

                ClaimRepository.SaveAll(claims);
            }

            TempData["Message"] = "Claim rejected.";
            return RedirectToAction("ClaimsList");
        }
    }
}
