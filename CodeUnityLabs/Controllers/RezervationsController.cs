using CodeUnityLabs.Data;
using CodeUnityLabs.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CodeUnityLabs.Controllers
{
    public class RezervationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RezervationsController> _logger;
        private bool IsAdmin() => HttpContext.Session.GetInt32("UserTypeId") == 1;
        private bool IsStaff() => HttpContext.Session.GetInt32("UserTypeId") == 2;
        private bool IsUser() => HttpContext.Session.GetInt32("UserTypeId") == 3;

        public RezervationsController(ApplicationDbContext context, ILogger<RezervationsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // ===============================
        // GET: Rezervations
        // ===============================

        public async Task<IActionResult> Index()
        {
            var rezervations = await _context.Reservations
                .Include(r => r.User)
                .Include(r => r.Resource)
                .AsNoTracking()
                .ToListAsync();

            return View(rezervations);
        }

        // ===============================
        // GET: Details
        // ===============================

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var rezervation = await _context.Reservations
                .Include(r => r.User)
                .Include(r => r.Resource)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Reservation_Id == id);

            if (rezervation == null) return NotFound();
            ViewBag.UserTypeId = HttpContext.Session.GetInt32("UserTypeId");
            return View(rezervation);
        }

        // ===============================
        // GET: Create
        // ===============================

        public IActionResult Create()
        {
            PopulateDropDowns();
            return View();
        }

        // ===============================
        // POST: Create
        // ===============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Rezervation rezervation)
        {
            if (!ModelState.IsValid)
            {
                PopulateDropDowns(rezervation);
                return View(rezervation);
            }

            var resource = await _context.Resources
                .Include(r => r.Reservations)
                .FirstOrDefaultAsync(r => r.Resource_Id == rezervation.Resource_Id);

            if (resource == null)
            {
                ModelState.AddModelError("", "Resource not found.");
                PopulateDropDowns(rezervation);
                return View(rezervation);
            }

            // Count approved reservations
            int activeReservations = resource.Reservations?
                .Count(r => r.Status == ReservationStatus.Approved || r.Status == ReservationStatus.Pending) ?? 0;

            if (activeReservations >= resource.Quantity)
            {
                rezervation.Status = ReservationStatus.Rejected;
                ModelState.AddModelError("", "Resource is fully booked. Reservation rejected.");
                PopulateDropDowns(rezervation);
                return View(rezervation);
            }

            // Otherwise, normal pending reservation
            rezervation.Status = ReservationStatus.Pending;
            _context.Reservations.Add(rezervation);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        // ===============================
        // GET: Edit
        // ===============================

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var rezervation = await _context.Reservations.FindAsync(id);

            if (rezervation == null) return NotFound();

            PopulateDropDowns(rezervation);

            return View(rezervation);
        }

        // ===============================
        // POST: Edit
        // ===============================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Rezervation rezervation)
        {
            if (id != rezervation.Reservation_Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                PopulateDropDowns(rezervation);
                return View(rezervation);
            }

            try
            {
                _context.Update(rezervation);

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Reservation updated.";
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Edit error");

                if (!RezervationExists(id))
                    return NotFound();

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // ===============================
        // GET: Delete
        // ===============================

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var rezervation = await _context.Reservations
                .Include(r => r.User)
                .Include(r => r.Resource)
                .FirstOrDefaultAsync(r => r.Reservation_Id == id);

            if (rezervation == null)
                return NotFound();

            return View(rezervation);
        }

        // ===============================
        // POST: DeleteConfirmed
        // ===============================

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rezervation = await _context.Reservations.FindAsync(id);

            if (rezervation != null)
            {
                int resourceId = rezervation.Resource_Id;

                _context.Reservations.Remove(rezervation);

                await _context.SaveChangesAsync();

                await ApproveNextWaiting(resourceId);

                TempData["SuccessMessage"] = "Reservation deleted.";
            }

            return RedirectToAction(nameof(Index));
        }

        // ===============================
        // Approve Next Waiting
        // ===============================

        private async Task ApproveNextWaiting(int resourceId)
        {
            var resource = await _context.Resources
                .Include(r => r.Reservations)
                .FirstOrDefaultAsync(r => r.Resource_Id == resourceId);

            if (resource == null) return;

            int active = resource.Reservations.Count(r => r.Status == ReservationStatus.Approved);
            var next = await _context.WaitingList
                .Where(w => w.Status == "Pending")
                .OrderBy(w => w.Priority)
                .ThenBy(w => w.Requested_At)
                .FirstOrDefaultAsync();

            if (next != null && active < resource.Quantity)
            {
                var reservation = new Rezervation
                {
                    User_Id = next.User_Id,
                    Resource_Id = resourceId,
                    Start_Time = DateTime.Now,
                    End_Time = DateTime.Now.AddHours(1),
                    Status = ReservationStatus.Approved,
                    Priority = next.Priority
                };

                _context.Reservations.Add(reservation);
                _context.WaitingList.Remove(next);
                await _context.SaveChangesAsync();
            }
        }

        // ===============================
        // Exists
        // ===============================

        private bool RezervationExists(int id)
        {
            return _context.Reservations.Any(e => e.Reservation_Id == id);
        }

        // ===============================
        // Dropdowns
        // ===============================

        private void PopulateDropDowns(Rezervation? rezervation = null)
        {
            ViewData["User_Id"] = new SelectList(
                _context.Users.OrderBy(u => u.Name),
                "User_Id",
                "Name",
                rezervation?.User_Id
            );

            ViewData["Resource_Id"] = new SelectList(
                _context.Resources.OrderBy(r => r.Resource_Name),
                "Resource_Id",
                "Resource_Name",
                rezervation?.Resource_Id
            );
        }
        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            if (!IsAdmin() && !IsStaff())
                return Forbid(); // normal users cannot approve

            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null) return NotFound();

            var resource = await _context.Resources
                .Include(r => r.Reservations)
                .FirstOrDefaultAsync(r => r.Resource_Id == reservation.Resource_Id);

            int active = resource.Reservations.Count(r => r.Status == ReservationStatus.Approved);
            if (active >= resource.Quantity)
                reservation.Status = ReservationStatus.Rejected;
            else
                reservation.Status = ReservationStatus.Approved;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}