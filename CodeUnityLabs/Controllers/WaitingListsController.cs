using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CodeUnityLabs.Data;
using CodeUnityLabs.Models;

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
            return View(await _context.WaitingList.ToListAsync());
        }

        // GET: WaitingLists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var waitingList = await _context.WaitingList
                .FirstOrDefaultAsync(m => m.Waiting_Id == id);
            if (waitingList == null)
            {
                return NotFound();
            }

            return View(waitingList);
        }

        // GET: WaitingLists/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: WaitingLists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Waiting_Id,User_Id,Requested_At,Status,Priority")] WaitingList waitingList)
        {
            if (ModelState.IsValid)
            {
                _context.Add(waitingList);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(waitingList);
        }

        // GET: WaitingLists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var waitingList = await _context.WaitingList.FindAsync(id);
            if (waitingList == null)
            {
                return NotFound();
            }
            return View(waitingList);
        }

        // POST: WaitingLists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Waiting_Id,User_Id,Requested_At,Status,Priority")] WaitingList waitingList)
        {
            if (id != waitingList.Waiting_Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(waitingList);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WaitingListExists(waitingList.Waiting_Id))
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
            return View(waitingList);
        }

        // GET: WaitingLists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var waitingList = await _context.WaitingList
                .FirstOrDefaultAsync(m => m.Waiting_Id == id);
            if (waitingList == null)
            {
                return NotFound();
            }

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
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WaitingListExists(int id)
        {
            return _context.WaitingList.Any(e => e.Waiting_Id == id);
        }
    }
}
