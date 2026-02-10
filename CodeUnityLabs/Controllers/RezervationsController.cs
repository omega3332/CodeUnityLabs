using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CodeUnityLabs.Data;
using CodeUnityLabs.Models;

namespace CodeUnityLabs.Controllers
{
    public class RezervationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RezervationsController> _logger;

        public RezervationsController(ApplicationDbContext context, ILogger<RezervationsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Rezervations
        public async Task<IActionResult> Index()
        {
            var rezervations = await _context.Reservations
                .Include(r => r.User)
                .Include(r => r.Resource)
                .AsNoTracking()
                .ToListAsync();

            return View(rezervations);
        }

        // GET: Rezervations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var rezervation = await _context.Reservations
                .Include(r => r.User)
                .Include(r => r.Resource)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Reservation_Id == id);

            if (rezervation == null) return NotFound();

            return View(rezervation);
        }

        // GET: Rezervations/Create
        public IActionResult Create()
        {
            PopulateDropDowns();
            return View();
        }

        // POST: Rezervations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Reservation_Id,User_Id,Resource_Id,Start_Time,End_Time,Status,Priority")] Rezervation rezervation)
        {
            if (!ModelState.IsValid)
            {
                PopulateDropDowns();
                return View(rezervation);
            }

            // Set default values if needed
            if (rezervation.Start_Time == default) rezervation.Start_Time = System.DateTime.Now;
            if (rezervation.End_Time == default) rezervation.End_Time = System.DateTime.Now.AddHours(1);

            _context.Add(rezervation);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Reservation created successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Rezervations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var rezervation = await _context.Reservations.FindAsync(id);
            if (rezervation == null) return NotFound();

            PopulateDropDowns(rezervation);
            return View(rezervation);
        }

        // POST: Rezervations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Reservation_Id,User_Id,Resource_Id,Start_Time,End_Time,Status,Priority")] Rezervation rezervation)
        {
            if (id != rezervation.Reservation_Id) return NotFound();

            if (!ModelState.IsValid)
            {
                PopulateDropDowns(rezervation);
                return View(rezervation);
            }

            try
            {
                _context.Update(rezervation);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Reservation updated successfully!";
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency error updating reservation {Id}", id);
                if (!RezervationExists(id)) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Rezervations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var rezervation = await _context.Reservations
                .Include(r => r.User)
                .Include(r => r.Resource)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Reservation_Id == id);

            if (rezervation == null) return NotFound();

            return View(rezervation);
        }

        // POST: Rezervations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rezervation = await _context.Reservations.FindAsync(id);
            if (rezervation != null)
            {
                _context.Reservations.Remove(rezervation);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Reservation {rezervation.Reservation_Id} deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        // Private Helpers
        private bool RezervationExists(int id) =>
            _context.Reservations.Any(e => e.Reservation_Id == id);

        // Populate dropdowns
        private void PopulateDropDowns(Rezervation? rezervation = null)
        {
            // Users dropdown
            var users = _context.Users
                .AsNoTracking()
                .OrderBy(u => u.Name)
                .ToList(); // keep as entities, not anonymous types

            ViewData["User_Id"] = new SelectList(
                users,                  // pass full entity
                "User_Id",              // value field
                "Name",                 // text field
                rezervation?.User_Id);  // selected value

            // Resources dropdown
            var resources = _context.Resources
                .AsNoTracking()
                .OrderBy(r => r.Resource_Name)
                .ToList(); // full entity

            ViewData["Resource_Id"] = new SelectList(
                resources,
                "Resource_Id",
                "Resource_Name",
                rezervation?.Resource_Id);
        }
    }
}
