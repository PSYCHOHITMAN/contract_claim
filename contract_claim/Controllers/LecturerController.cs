using contract_claim.Data;
using contract_claim.Models;
using Microsoft.AspNetCore.Mvc;

namespace contract_claim.Controllers
{
    public class LecturerController : BaseController
    {
        [HttpGet]
        public IActionResult SubmitClaim()
        {
            var redirect = RedirectIfNotRole("Lecturer");
            if (redirect != null) return redirect;

            return View();
        }

        [HttpPost]
        public IActionResult SubmitClaim(Claim claim, IFormFile file)
        {
            var redirect = RedirectIfNotRole("Lecturer");
            if (redirect != null) return redirect;

            if (file != null && file.Length > 0)
            {
                var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsDir))
                    Directory.CreateDirectory(uploadsDir);

                var fileName = Path.GetFileName(file.FileName);
                var filePath = Path.Combine(uploadsDir, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                claim.FileName = fileName;
            }

            claim.Status = "Pending";
            ClaimRepository.Add(claim);

            TempData["Message"] = "Claim submitted successfully!";
            return RedirectToAction("TrackClaims");
        }

        [HttpGet]
        public IActionResult TrackClaims()
        {
            var redirect = RedirectIfNotRole("Lecturer");
            if (redirect != null) return redirect;

            var allClaims = ClaimRepository.GetAll();
            return View(allClaims);
        }
    }
}
