using CodeUnityLabs.Data;
using CodeUnityLabs.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CodeUnityLabs.Controllers
{
    public class WaitingListsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WaitingListsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: WaitingLists
        public async Task<IActionResult> Index()
        {
            ViewBag.UserTypeId = HttpContext.Session.GetInt32("UserTypeId");
            var list = await _context.WaitingList.Include(w => w.User).ToListAsync();
            return View(list);
        }

        // GET: WaitingLists/Create
        public IActionResult Create()
        {
            PopulateUsersDropdown();
            return View();
        }

        // POST: WaitingLists/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Waiting_Id,User_Id,Requested_At,Status,Priority")] WaitingList waitingList)
        {
            // Set defaults to avoid ModelState errors
            if (waitingList.Requested_At == default)
                waitingList.Requested_At = DateTime.Now;
            if (string.IsNullOrEmpty(waitingList.Status))
                waitingList.Status = "Pending";

            if (!ModelState.IsValid)
            {
                PopulateUsersDropdown(waitingList.User_Id);
                return View(waitingList);
            }

            _context.Add(waitingList);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        // GET: WaitingLists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var waitingList = await _context.WaitingList.FindAsync(id);
            if (waitingList == null) return NotFound();

            PopulateUsersDropdown(waitingList.User_Id);
            return View(waitingList);
        }

        // POST: WaitingLists/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Waiting_Id,User_Id,Requested_At,Status,Priority")] WaitingList waitingList)
        {
            if (id != waitingList.Waiting_Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(waitingList);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WaitingListExists(waitingList.Waiting_Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            PopulateUsersDropdown(waitingList.User_Id);
            return View(waitingList);
        }

        // GET: WaitingLists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var waitingList = await _context.WaitingList
                .Include(w => w.User)
                .FirstOrDefaultAsync(m => m.Waiting_Id == id);
            if (waitingList == null) return NotFound();

            return View(waitingList);
        }

        // POST: WaitingLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var waitingList = await _context.WaitingList.FindAsync(id);
            if (waitingList != null)
            {
                _context.WaitingList.Remove(waitingList);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool WaitingListExists(int id)
        {
            return _context.WaitingList.Any(e => e.Waiting_Id == id);
        }

        // Helper: populate users dropdown
        private void PopulateUsersDropdown(object? selectedUser = null)
        {
            ViewBag.Users = new SelectList(_context.Users, "User_Id", "Name", selectedUser);
        }
    }
}
