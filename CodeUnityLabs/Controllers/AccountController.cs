using CodeUnityLabs.Data;
using CodeUnityLabs.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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
        public IActionResult Login()
        {
            return View();
        }

        // POST: Handle login
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = _context.Users
                .FirstOrDefault(u => u.Email == email && u.Password == password);

            if (user == null)
            {
                ViewBag.Error = "Invalid email or password";
                ViewBag.Email = email; // <-- pass email back to view
                return View();
            }

            // Store session
            HttpContext.Session.SetInt32("UserId", user.User_Id);
            HttpContext.Session.SetString("UserName", user.Name);
            HttpContext.Session.SetInt32("UserTypeId", user.User_Type_Id);

            return RedirectToAction("Index", "Home");
        }
        // GET
        public IActionResult Register()
        {
            ViewBag.UserTypes = new SelectList(
                _context.UserTypes.ToList(),
                "User_Type_Id",
                "Type_Name"
            );

            return View();
        }

        // POST
        [HttpPost]
        public IActionResult Register(User user)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.UserTypes = new SelectList(
                    _context.UserTypes.ToList(),
                    "User_Type_Id",
                    "Type_Name"
                );

                return View(user);
            }

            var exists = _context.Users.Any(u => u.Email == user.Email);

            if (exists)
            {
                ViewBag.Error = "Email already exists";

                ViewBag.UserTypes = new SelectList(
                    _context.UserTypes.ToList(),
                    "User_Type_Id",
                    "Type_Name"
                );

                return View(user);
            }

            _context.Users.Add(user);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }

        // GET: Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}