using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CodeUnityLabs.Data;
using CodeUnityLabs.Models;
using System;

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
            if (ModelState.IsValid)
            {
                var existing = await _context.Reservations
                    .Where(r => r.Resource_Id == rezervation.Resource_Id
                             && r.Status != ReservationStatus.Approved
                             && r.Status != ReservationStatus.Rejected)
                    .OrderBy(r => r.Priority)
                    .FirstOrDefaultAsync();

                if (existing == null)
                {
                    // FIRST → Pending
                    rezervation.Status = ReservationStatus.Pending;
                    rezervation.Priority = 1;
                }
                else
                {
                    // WAITING LIST
                    rezervation.Status = ReservationStatus.Waiting;

                    int maxPriority = await _context.Reservations
                        .Where(r => r.Resource_Id == rezervation.Resource_Id)
                        .MaxAsync(r => (int?)r.Priority) ?? 0;

                    rezervation.Priority = maxPriority + 1;

                    // 🔥 ADD TO WAITING LIST TABLE
                    WaitingList waiting = new WaitingList
                    {
                        User_Id = rezervation.User_Id,
                        Requested_At = DateTime.Now,
                        Status = "Waiting",
                        Priority = rezervation.Priority
                    };

                    _context.WaitingList.Add(waiting);
                }

                _context.Reservations.Add(rezervation);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            PopulateDropDowns();
            return View(rezervation);
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
            var next = await _context.WaitingList
                .Where(w => w.Status == "Pending")
                .OrderBy(w => w.Priority)
                .ThenBy(w => w.Requested_At)
                .FirstOrDefaultAsync();

            if (next == null)
                return;

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
        public async Task<IActionResult> Approve(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);

            if (reservation == null)
                return NotFound();

            // Check if there is already an approved reservation for same room/date
            bool alreadyApproved = _context.Reservations.Any(r =>
                r.Room_Id == reservation.Room_Id &&
                r.Start_Time == reservation.Start_Time &&
                r.Status == ReservationStatus.Approved);

            if (alreadyApproved)
            {
                reservation.Status = ReservationStatus.Waiting;
            }
            else
            {
                reservation.Status = ReservationStatus.Approved;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}