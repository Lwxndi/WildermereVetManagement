using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IntergratedProject.Data;
using IntergratedProject.Models;
using IntergratedProject.Security;
using IntergratedProject.ViewModels;

namespace IntergratedProject.Controllers
{
    //[RoleAuthorize("Admin")]
    public class AdminsController : Controller
    {
        private readonly IntergratedProjectDatabase _context;

        public AdminsController(IntergratedProjectDatabase context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        // GET: Admins
        public async Task<IActionResult> Index(string searchTerm, string roleFilter)
        {
            var users = new List<UserViewModel>();

            // Load all users from DB
            var admins = await _context.Admin.ToListAsync();
            var vets = await _context.Veterinarian.ToListAsync();
            var owners = await _context.PetOwner.ToListAsync();

            // Combine into a unified list
            users.AddRange(admins.Select(a => new UserViewModel
            {
                Id = a.AdminId,
                FullName = a.FullName,
                Email = a.Email,
                Role = "Admin"
            }));

            users.AddRange(vets.Select(v => new UserViewModel
            {
                Id = v.VeterinarianId,
                FullName = v.Name,
                Email = v.Email,
                Role = "Veterinarian"
            }));

            users.AddRange(owners.Select(o => new UserViewModel
            {
                Id = o.OwnerId,
                FullName = o.Name,
                Email = o.Email,
                Role = "PetOwner"
            }));

            // Apply filters
            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                users = users.Where(u =>
                    u.FullName.ToLower().Contains(searchTerm) ||
                    u.Email.ToLower().Contains(searchTerm)
                ).ToList();
            }

            if (!string.IsNullOrEmpty(roleFilter) && roleFilter != "All")
            {
                users = users.Where(u => u.Role == roleFilter).ToList();
            }

            ViewBag.CurrentSearch = searchTerm;
            ViewBag.CurrentRole = roleFilter;

            return View(users);
        }


        // GET: Admins/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admin
                .FirstOrDefaultAsync(m => m.AdminId == id);
            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
        }

        // GET: Admins/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admins/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AdminId,FullName,Email,Password,Role")] Admin admin)
        {
            if (ModelState.IsValid)
            {
                _context.Add(admin);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(admin);
        }

        // GET: Admins/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admin.FindAsync(id);
            if (admin == null)
            {
                return NotFound();
            }
            return View(admin);
        }

        // POST: Admins/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AdminId,FullName,Email,Password,Role")] Admin admin)
        {
            if (id != admin.AdminId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(admin);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdminExists(admin.AdminId))
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
            return View(admin);
        }


        // GET: Admins/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admin
                .FirstOrDefaultAsync(m => m.AdminId == id);
            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
        }

        // POST: Admins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var admin = await _context.Admin.FindAsync(id);
            if (admin != null)
            {
                _context.Admin.Remove(admin);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdminExists(int id)
        {
            return _context.Admin.Any(e => e.AdminId == id);
        }
    }
}
