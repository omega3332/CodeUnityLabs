using CodeUnityLabs.Data;
using CodeUnityLabs.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeUnityLabs.Controllers
{
    public class AuthorizationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthorizationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Authorizations
        public async Task<IActionResult> Index()
        {
            return View(await _context.Authorizations.ToListAsync());
        }

        // GET: Authorizations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var authorizations = await _context.Authorizations
                .FirstOrDefaultAsync(m => m.Authorization_Id == id);
            if (authorizations == null)
            {
                return NotFound();
            }
            ViewBag.UserTypeId = HttpContext.Session.GetInt32("UserTypeId");
            return View(authorizations);
        }

        // GET: Authorizations/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Authorizations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Authorization_Id,User_Id,Permission,Granted_By,Granted_At,Expires_At,Priority")] Authorizations authorizations)
        {
            if (ModelState.IsValid)
            {
                _context.Add(authorizations);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(authorizations);
        }

        // GET: Authorizations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var authorizations = await _context.Authorizations.FindAsync(id);
            if (authorizations == null)
            {
                return NotFound();
            }
            return View(authorizations);
        }

        // POST: Authorizations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Authorization_Id,User_Id,Permission,Granted_By,Granted_At,Expires_At,Priority")] Authorizations authorizations)
        {
            if (id != authorizations.Authorization_Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(authorizations);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AuthorizationsExists(authorizations.Authorization_Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(authorizations);
        }

        // GET: Authorizations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var authorizations = await _context.Authorizations
                .FirstOrDefaultAsync(m => m.Authorization_Id == id);
            if (authorizations == null)
            {
                return NotFound();
            }

            return View(authorizations);
        }

        // POST: Authorizations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var authorizations = await _context.Authorizations.FindAsync(id);
            if (authorizations != null)
            {
                _context.Authorizations.Remove(authorizations);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AuthorizationsExists(int id)
        {
            return _context.Authorizations.Any(e => e.Authorization_Id == id);
        }
    }
}
