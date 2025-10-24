using contract_claim.Data;
using contract_claim.Models;
using Microsoft.AspNetCore.Mvc;

namespace contract_claim.Controllers
{
    public class CoordinatorController : BaseController
    {
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
                claim.Status = "Approved";
                ClaimRepository.SaveAll(claims);
            }

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
                claim.Status = "Rejected";
                ClaimRepository.SaveAll(claims);
            }

            return RedirectToAction("ClaimsList");
        }
    }
}
