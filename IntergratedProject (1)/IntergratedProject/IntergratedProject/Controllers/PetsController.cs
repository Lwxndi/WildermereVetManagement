using IntergratedProject.Data;
using IntergratedProject.Models;
using IntergratedProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Http;
using System;

namespace IntergratedProject.Controllers
{
    public class PetsController : Controller
    {
        private readonly IntergratedProjectDatabase _context;

        public PetsController(IntergratedProjectDatabase context)
        {
            _context = context;
        }

        // GET: Pets
        public async Task<IActionResult> Index()
        {
            var ownerId = HttpContext.Session.GetInt32("PetOwnerId");
            if (ownerId == null) return RedirectToAction("Index", "Home");

            var pets = await _context.Pet
                .Include(p => p.Species)
                .Include(p => p.Breed)
                .Include(p => p.PrimaryVeterinarian)
                .Where(p => p.PetOwnerId == ownerId.Value)
                .ToListAsync();

            return View(pets);
        }

        // GET: Pets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var ownerId = HttpContext.Session.GetInt32("PetOwnerId");
            if (ownerId == null) return RedirectToAction("Login", "Account");

            var pet = await _context.Pet
                .Include(p => p.Species)
                .Include(p => p.Breed)
                .Include(p => p.PrimaryVeterinarian)
                .Include(p => p.PetOwner)
                .FirstOrDefaultAsync(p => p.PetId == id);

            if (pet == null) return NotFound();

            // Make sure this pet belongs to the logged-in PetOwner
            if (pet.PetOwnerId != ownerId.Value) return Forbid();

            return View(pet);
        }


        // GET: Pets/Create
        public IActionResult Create()
        {
            var model = new PetCreateViewModel
            {
                SpeciesList = new SelectList(_context.Species.OrderBy(s => s.Name), "SpeciesId", "Name"),
                BreedList = new SelectList(Enumerable.Empty<Breed>(), "BreedId", "Name"),
                Veterinarians = _context.Veterinarian
                    .Select(v => new SelectListItem { Value = v.VeterinarianId.ToString(), Text = $"Dr. {v.Name} {v.LastName}" })
                    .ToList()
            };

            return View(model);
        }

        // POST: Pets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PetCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(model);
                return View(model);
            }

            var ownerId = HttpContext.Session.GetInt32("PetOwnerId");
            if (ownerId == null) return RedirectToAction("Login", "Account");

            string? imageFileName = null;
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                imageFileName = await SaveImage(model.ImageFile);
            }

            var pet = new Pet
            {
                PetName = model.PetName,
                SpeciesId = model.SpeciesId,
                BreedId = model.BreedId,
                AgeYears = model.AgeYears,
                WeightKg = model.WeightKg,
                HeightCm = model.HeightCm,
                ColorMarkings = model.ColorMarkings,
                Gender = model.Gender,
                PetOwnerId = ownerId.Value,
                ImageFileName = imageFileName,
                PrimaryVeterinarianId = model.PrimaryVeterinarianId
            };

            _context.Pet.Add(pet);
            await _context.SaveChangesAsync();

            return RedirectToAction("Dashboard", "PetOwners");
        }

        // GET: Pets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var pet = await _context.Pet.FindAsync(id);
            if (pet == null) return NotFound();

            var model = new PetCreateViewModel
            {
                PetId = pet.PetId,
                PetName = pet.PetName,
                SpeciesId = pet.SpeciesId,
                BreedId = pet.BreedId,
                AgeYears = pet.AgeYears,
                WeightKg = pet.WeightKg,
                HeightCm = pet.HeightCm,
                ColorMarkings = pet.ColorMarkings,
                Gender = pet.Gender,
                PrimaryVeterinarianId = pet.PrimaryVeterinarianId,
            };

            await PopulateDropdowns(model, pet.SpeciesId, pet.BreedId);

            return View(model);
        }

        // POST: Pets/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PetCreateViewModel model)
        {
            if (id != model.PetId) return NotFound();

            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(model, model.SpeciesId, model.BreedId);
                return View(model);
            }

            var ownerId = HttpContext.Session.GetInt32("PetOwnerId");
            if (ownerId == null) return RedirectToAction("Login", "Account");

            var pet = await _context.Pet.FindAsync(id);
            if (pet == null) return NotFound();

            pet.PetName = model.PetName;
            pet.SpeciesId = model.SpeciesId;
            pet.BreedId = model.BreedId;
            pet.AgeYears = model.AgeYears;
            pet.WeightKg = model.WeightKg;
            pet.HeightCm = model.HeightCm;
            pet.ColorMarkings = model.ColorMarkings;
            pet.Gender = model.Gender;
            pet.PrimaryVeterinarianId = model.PrimaryVeterinarianId;

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                pet.ImageFileName = await SaveImage(model.ImageFile);
            }

            _context.Update(pet);
            await _context.SaveChangesAsync();

            return RedirectToAction("Dashboard", "PetOwners");
        }

        // GET: Pets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var pet = await _context.Pet
                .Include(p => p.Species)
                .Include(p => p.Breed)
                .Include(p => p.PrimaryVeterinarian)
                .FirstOrDefaultAsync(p => p.PetId == id);

            if (pet == null) return NotFound();

            return View(pet);
        }

        // POST: Pets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pet = await _context.Pet.FindAsync(id);
            if (pet == null) return NotFound();

            // Optional: Delete associated image
            if (!string.IsNullOrEmpty(pet.ImageFileName))
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", pet.ImageFileName);
                if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
            }

            _context.Pet.Remove(pet);
            await _context.SaveChangesAsync();
            return RedirectToAction("Dashboard", "PetOwners");
        }

        // AJAX: Get breeds for species
        [HttpGet]
        public IActionResult GetBreeds(int speciesId)
        {
            var breeds = _context.Breed
                .Where(b => b.SpeciesId == speciesId)
                .Select(b => new { b.BreedId, b.Name })
                .ToList();

            return Json(breeds);
        }

        #region Helpers

        private async Task PopulateDropdowns(PetCreateViewModel model, int? selectedSpeciesId = null, int? selectedBreedId = null)
        {
            model.SpeciesList = new SelectList(await _context.Species.OrderBy(s => s.Name).ToListAsync(), "SpeciesId", "Name", selectedSpeciesId);

            model.BreedList = selectedSpeciesId.HasValue
                ? new SelectList(await _context.Breed.Where(b => b.SpeciesId == selectedSpeciesId.Value).ToListAsync(), "BreedId", "Name", selectedBreedId)
                : new SelectList(Enumerable.Empty<Breed>(), "BreedId", "Name");

            model.Veterinarians = await _context.Veterinarian
                .Select(v => new SelectListItem { Value = v.VeterinarianId.ToString(), Text = $"Dr. {v.Name} {v.LastName}" })
                .ToListAsync();
        }

        private async Task<string> SaveImage(IFormFile image)
        {
            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(image.FileName)}";
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

            using var stream = new FileStream(path, FileMode.Create);
            await image.CopyToAsync(stream);

            return fileName;
        }

        // GET: Pets/UpdateStatus/5
        [HttpGet]
        public async Task<IActionResult> UpdateStatus(int? id)
        {
            if (id == null) return NotFound();

            var pet = await _context.Pet.FindAsync(id);
            if (pet == null) return NotFound();

            // Build a dropdown for statuses
            var model = new PetStatusViewModel
            {
                PetId = pet.PetId,
                PetName = pet.PetName,
                CurrentStatus = pet.Status,
                StatusOptions = Enum.GetValues(typeof(PetStatus))
                                    .Cast<PetStatus>()
                                    .Select(s => new SelectListItem { Value = s.ToString(), Text = s.ToString() })
                                    .ToList()
            };

            return View(model);
        }

        // POST: Pets/UpdateStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(PetStatusViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var pet = await _context.Pet.FindAsync(model.PetId);
            if (pet == null) return NotFound();

            var oldStatus = pet.Status;
            pet.Status = model.NewStatus;
            if (model.NewStatus == PetStatus.Transferred && model.NewHospitalId.HasValue)
            {
                pet.CurrentHospitalId = model.NewHospitalId;
            }

            _context.Update(pet);
            await _context.SaveChangesAsync();

            // Audit trail
            var audit = new PetAuditTrail
            {
                PetId = pet.PetId,
                Action = "StatusUpdated",
                PreviousStatus = oldStatus.ToString(),
                NewStatus = model.NewStatus.ToString(),
                PerformedBy = User.Identity.Name,
                Timestamp = DateTime.UtcNow,
                CurrentHospitalId = pet.CurrentHospitalId
            };
            _context.Add(audit);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        #endregion
    }
}
