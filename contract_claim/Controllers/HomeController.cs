using System.Diagnostics;
using contract_claim.Models;
using Microsoft.AspNetCore.Mvc;

namespace contract_claim.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Welcome()
        {
            var role = HttpContext.Session.GetString("Role");
            if (!string.IsNullOrEmpty(role))
            {
                return role switch
                {
                    "Lecturer" => RedirectToAction("SubmitClaim", "Lecturer"),
                    "Coordinator" => RedirectToAction("ClaimsList", "Coordinator"),
                    "Manager" => RedirectToAction("ClaimsList", "Manager"),
                    _ => RedirectToAction("Login", "Account")
                };
            }

            return View(); // your welcome/landing page
        }

       

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
