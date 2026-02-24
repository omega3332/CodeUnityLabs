using CodeUnityLabs.Data;
using CodeUnityLabs.Models;
using Microsoft.AspNetCore.Mvc;

namespace CodeUnityLabs.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Login page
        public IActionResult Login() => View();

        // POST: Handle login
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = _context.Users
                .FirstOrDefault(u => u.Email == email && u.Password == password);

            if (user == null)
            {
                ViewBag.Error = "Invalid email or password";
                return View();
            }

            // Store session
            HttpContext.Session.SetInt32("UserId", user.User_Id);
            HttpContext.Session.SetString("UserName", user.Name);
            HttpContext.Session.SetInt32("UserTypeId", user.User_Type_Id);

            return RedirectToAction("Index", "Home");
        }

        // GET: Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}