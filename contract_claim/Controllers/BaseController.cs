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

        protected IActionResult? RedirectIfNotRole(params string[] roles)
        {
            var sessionRole = HttpContext.Session.GetString("Role");

            if (sessionRole == null || !roles.Contains(sessionRole))
                return RedirectToAction("Login", "Account");

            return null;
        }
    }
}
