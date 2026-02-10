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
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            return View(
                await _context.Users
                    .Include(u => u.UserType)
                    .ToListAsync()
            );
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.Users
                .Include(u => u.UserType)
                .FirstOrDefaultAsync(m => m.User_Id == id);

            if (user == null) return NotFound();

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            PopulateUserTypesDropDown();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user)
        {
            if (_context.Users.Any(u => u.Email == user.Email))
            {
                ModelState.AddModelError("Email", "Email already exists");
            }

            if (!ModelState.IsValid)
            {
                PopulateUserTypesDropDown(user.User_Type_Id);
                return View(user);
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }




        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            PopulateUserTypesDropDown(user.User_Type_Id);
            return View(user);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("User_Id,Name,Email,Password,User_Type_Id")] User user)
        {
            if (id != user.User_Id) return NotFound();

            // Prevent duplicate email (exclude current user)
            if (_context.Users.Any(u => u.Email == user.Email && u.User_Id != user.User_Id))
            {
                ModelState.AddModelError("Email", "A user with this email already exists.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.User_Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            PopulateUserTypesDropDown(user.User_Type_Id);
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.Users
                .Include(u => u.UserType)
                .FirstOrDefaultAsync(m => m.User_Id == id);

            if (user == null) return NotFound();

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null) _context.Users.Remove(user);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Helper: Populate UserType dropdown
        private void PopulateUserTypesDropDown(object? selectedType = null)
        {
            var userTypes = _context.UserTypes.ToList();

            // Fallback if table is empty
            if (userTypes == null || !userTypes.Any())
            {
                userTypes = new List<UserType>
                {
                    new UserType { User_Type_Id = 1, Type_Name = "Admin" },
                    new UserType { User_Type_Id = 2, Type_Name = "Regular" }
                };
            }

            ViewBag.UserTypeId = new SelectList(userTypes, "User_Type_Id", "Type_Name", selectedType);
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.User_Id == id);
        }
    }
}
