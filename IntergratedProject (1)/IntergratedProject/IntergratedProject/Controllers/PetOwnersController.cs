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


namespace IntergratedProject.Controllers
{
    public class PetOwnersController : Controller
    {
        private readonly IntergratedProjectDatabase _context;

        public PetOwnersController(IntergratedProjectDatabase context)
        {
            _context = context;
        }

        // GET: PetOwners
        public async Task<IActionResult> Index()
        {
            return View(await _context.PetOwner.ToListAsync());
        }

        // GET: PetOwners/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var petOwner = await _context.PetOwner
                .FirstOrDefaultAsync(m => m.OwnerId == id);
            if (petOwner == null)
            {
                return NotFound();
            }

            return View(petOwner);
        }

        // GET: PetOwners/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PetOwners/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public async Task<IActionResult> Create([Bind("OwnerId,Name,MiddleName,LastName,Email,Address,PhoneNumber,Password,ConfirmPassword")] PetOwner petOwner)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // 🔍 Check if email already exists
                    bool emailExists = await _context.PetOwner.AnyAsync(o => o.Email == petOwner.Email);
                    if (emailExists)
                    {
                        ModelState.AddModelError("Email", "A user with this email already exists.");
                        return View(petOwner);
                    }

                    // ✅ Continue if no duplicate
                    petOwner.Role = "PetOwner";
                    _context.Add(petOwner);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Something went wrong while saving data.");
                Console.WriteLine("Error during save: " + ex.Message);
            }

            return RedirectToAction("Home", "Dashboard");
        }



        // GET: PetOwners/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var petOwner = await _context.PetOwner.FindAsync(id);
            if (petOwner == null)
            {
                return NotFound();
            }

            // Map PetOwner entity to PetOwnerEditViewModel
            var model = new PetOwnerEditViewModel
            {
                OwnerId = petOwner.OwnerId,
                Name = petOwner.Name,
                MiddleName = petOwner.MiddleName,
                LastName = petOwner.LastName,
                Email = petOwner.Email,
                Address = petOwner.Address,
                PhoneNumber = petOwner.PhoneNumber
            };

            return View(model);
        }


        // POST: PetOwners/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PetOwnerEditViewModel model)
        {
            if (id != model.OwnerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var petOwner = await _context.PetOwner.FindAsync(id);
                    if (petOwner == null)
                    {
                        return NotFound();
                    }

                    // Update only editable fields (no password)
                    petOwner.Name = model.Name;
                    petOwner.MiddleName = model.MiddleName;
                    petOwner.LastName = model.LastName;
                    petOwner.Email = model.Email;
                    petOwner.Address = model.Address;
                    petOwner.PhoneNumber = model.PhoneNumber;

                    _context.Update(petOwner);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PetOwnerExists(model.OwnerId))
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

            return View(model);
        }


        // GET: PetOwners/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var petOwner = await _context.PetOwner
                .FirstOrDefaultAsync(m => m.OwnerId == id);
            if (petOwner == null)
            {
                return NotFound();
            }

            return View(petOwner);
        }

        // POST: PetOwners/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var petOwner = await _context.PetOwner.FindAsync(id);
            if (petOwner != null)
            {
                _context.PetOwner.Remove(petOwner);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PetOwnerExists(int id)
        {
            return _context.PetOwner.Any(e => e.OwnerId == id);
        }

        public async Task<IActionResult> Dashboard()
        {
            int? ownerId = HttpContext.Session.GetInt32("PetOwnerId");

            if (ownerId == null)
                return RedirectToAction("Login", "Home");

            var petOwner = await _context.PetOwner
                .Include(o => o.Pets)
                    .ThenInclude(p => p.Breed)
                .Include(o => o.Pets)
                    .ThenInclude(p => p.Species)
                .FirstOrDefaultAsync(o => o.OwnerId == ownerId);

            if (petOwner == null)
                return NotFound();

            return View(petOwner);
        }


    }
}
