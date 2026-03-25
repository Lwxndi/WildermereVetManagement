    using IntergratedProject.Data;
using IntergratedProject.Models;
using IntergratedProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IntergratedProject.Security;


namespace IntergratedProject.Controllers
{
    [RoleAuthorize("Veterinarian")]
    public class VeterinariansController : Controller
    {
        private readonly IntergratedProjectDatabase _context;

        public VeterinariansController(IntergratedProjectDatabase context)
        {
            _context = context;
        }

        // GET: Veterinarians
        public async Task<IActionResult> Index()
        {
            return View(await _context.Veterinarian.ToListAsync());
        }

        public async Task<IActionResult> Dashboard()
        {
            var email = HttpContext.Session.GetString("Email");
            var vet = await _context.Veterinarian.FirstOrDefaultAsync(v => v.Email == email);
            if (vet == null) return RedirectToAction("Login", "Home");

            // Vet ID from session
            int currentVetId = vet.VeterinarianId;

            var vm = new VeterinarianDashboardViewModel
            {
                UpcomingAppointments = await _context.Appointment
                    .Include(a => a.Pet)
                        .ThenInclude(p => p.PetOwner)
                    .Where(a => a.VeterinarianId == currentVetId && a.Date >= DateTime.Now)
                    .OrderBy(a => a.Date)
                    .ThenBy(a => a.Time)
                    .Take(5)
                    .ToListAsync(),

                RecentMedicalRecords = await _context.MedicalRecord
                    .Include(m => m.Pet)
                        .ThenInclude(p => p.PetOwner)
                    .Where(m => m.VeterinarianId == currentVetId)
                    .OrderByDescending(m => m.CreatedAt)
                    .Take(5)
                    .Select(m => new MedicalRecordSummaryViewModel
                    {
                        Id = m.Id,
                        PetName = m.Pet.PetName,
                        OwnerName = m.Pet.PetOwner.Name,
                        Diagnosis = m.Diagnosis.Length > 50 ? m.Diagnosis.Substring(0, 50) + "..." : m.Diagnosis,
                        Priority = m.Priority.ToString(),
                        CreatedAt = m.CreatedAt
                    })
                    .ToListAsync()
            };

            return View(vm);
        }



        // GET: Veterinarians/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var veterinarian = await _context.Veterinarian
                .FirstOrDefaultAsync(m => m.VeterinarianId == id);
            if (veterinarian == null)
            {
                return NotFound();
            }

            return View(veterinarian);
        }

        // GET: Veterinarians/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Veterinarians/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateVeterinarianViewModel model)
        {
            if (ModelState.IsValid)
            {
                var veterinarian = new Veterinarian
                {
                    Name = model.Name,
                    LastName = model.LastName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Password = model.Password,
                    Role = "Veterinarian"// TODO: hash before saving
                };

                _context.Add(veterinarian);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }


        // GET: Veterinarians/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var veterinarian = await _context.Veterinarian.FindAsync(id);
            if (veterinarian == null)
            {
                return NotFound();
            }

            var model = new VeterinarianEditViewModel
            {
                VeterinarianId = veterinarian.VeterinarianId,
                Name = veterinarian.Name,
                LastName = veterinarian.LastName,
                Email = veterinarian.Email,
                PhoneNumber = veterinarian.PhoneNumber
            };

            return View(model);
        }

        // POST: Veterinarians/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, VeterinarianEditViewModel model)
        {
            if (id != model.VeterinarianId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var veterinarian = await _context.Veterinarian.FindAsync(id);
                    if (veterinarian == null)
                    {
                        return NotFound();
                    }

                    // Update only editable fields — do NOT update Password here
                    veterinarian.Name = model.Name;
                    veterinarian.LastName = model.LastName;
                    veterinarian.Email = model.Email;
                    veterinarian.PhoneNumber = model.PhoneNumber;

                    _context.Update(veterinarian);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VeterinarianExists(model.VeterinarianId))
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


        // GET: Veterinarians/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var veterinarian = await _context.Veterinarian
                .FirstOrDefaultAsync(m => m.VeterinarianId == id);
            if (veterinarian == null)
            {
                return NotFound();
            }

            return View(veterinarian);
        }

        // POST: Veterinarians/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var veterinarian = await _context.Veterinarian.FindAsync(id);
            if (veterinarian != null)
            {
                _context.Veterinarian.Remove(veterinarian);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VeterinarianExists(int id)
        {
            return _context.Veterinarian.Any(e => e.VeterinarianId == id);
        }
    }
}
