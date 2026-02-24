using CodeUnityLabs.Data;
using CodeUnityLabs.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CodeUnityLabs.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }


        public IActionResult Index()
        {
            // Dashboard stats
            ViewBag.UserCount = _context.Users.Count();
            ViewBag.ReservationCount = _context.Reservations.Count();
            ViewBag.WaitingCount = _context.WaitingList.Count();

            // Session-based user info (optional)
            ViewBag.UserName = HttpContext.Session.GetString("UserName");

            return View();
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
