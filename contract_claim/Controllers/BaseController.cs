using Microsoft.AspNetCore.Mvc;

namespace contract_claim.Controllers
{
    public class BaseController : Controller
    {
        protected bool IsLoggedIn()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString("Username"));
        }

        protected string GetUserRole()
        {
            return HttpContext.Session.GetString("Role");
        }

        protected IActionResult RedirectIfNotLoggedIn()
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");
            return null;
        }

        protected IActionResult RedirectIfNotRole(string role)
        {
            if (!IsLoggedIn() || GetUserRole() != role)
                return RedirectToAction("Login", "Account");
            return null;
        }
    }
}
