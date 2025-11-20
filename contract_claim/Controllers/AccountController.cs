using contract_claim.Data;
using contract_claim.Models;
using Microsoft.AspNetCore.Mvc;

namespace contract_claim.Controllers
{
    public class AccountController : Controller
{
    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    public IActionResult Register(string username, string email, string password, string role)
    {
        if (UserRepository.Exists(email))
        {
            ViewBag.Error = "Email already registered!";
            return View();
        }

        var user = new User
        {
            Username = username,
            Email = email,
            Password = password,
            Role = role
        };

            UserRepository.Add(user);
            ViewBag.Success = "Registration successful! You can now login";
            return View();
        }

        [HttpGet]
    public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = UserRepository.Find(email, password);
            if (user == null)
            {
                ViewBag.Error = "Invalid email or password!";
                return View();
            }

            // Store login session for ALL users
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("Role", user.Role);

            // Redirect based on role
            return user.Role switch
            {
                "Lecturer" => RedirectToAction("Index", "Lecturer"),
                "Coordinator" => RedirectToAction("Index", "Coordinator"),
                "Manager" => RedirectToAction("Index", "Manager"),
                "HR" => RedirectToAction("Index", "HR"),
                _ => RedirectToAction("Login")
            };
        }


        public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}
}