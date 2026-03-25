using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using IntergratedProject.Data;
using IntergratedProject.Models;
using IntergratedProject.ViewModels;

namespace IntergratedProject.Controllers
{
    [Authorize]
    public class MedicalRecordsController : Controller
    {
        private readonly IntergratedProjectDatabase _context;
        private readonly IWebHostEnvironment _env;

        public MedicalRecordsController(IntergratedProjectDatabase context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ---------------- INDEX ----------------
        [Authorize(Roles = "Admin,Vet,PetOwner")]
        public async Task<IActionResult> Index()
        {
            var role = GetCurrentUserRole();

            var records = await _context.MedicalRecord
                .Include(m => m.Pet)
                .Include(m => m.Veterinarian)
                .Include(m => m.Appointment)
                .Select(m => new MedicalRecordListViewModel
                {
                    Id = m.Id,
                    PetName = m.Pet.PetName,
                    VeterinarianName = m.Veterinarian.Name,
                    DiagnosisSummary = m.Diagnosis.Length > 50 ? m.Diagnosis.Substring(0, 50) + "..." : m.Diagnosis,
                    CreatedAt = m.CreatedAt,
                    Priority = m.Priority,
                    ViewerRole = role,
                    IsSigned = m.IsSigned,
                    SignedByRole = m.SignedByRole
                }).ToListAsync();

            return View(records);
        }

        // ---------------- DETAILS ----------------
        [Authorize(Roles = "Admin,Vet,PetOwner")]
        public async Task<IActionResult> Details(int id)
        {
            var record = await _context.MedicalRecord
                .Include(m => m.Pet)
                .Include(m => m.Veterinarian)
                .Include(m => m.Appointment)
                .Include(m => m.Files)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (record == null) return NotFound();
            return View(record);
        }

        // ---------------- CREATE ----------------
        [Authorize(Roles = "Admin,Vet")]
        public IActionResult Create() => View(PopulateCreateEditVM());

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Vet")]
        public async Task<IActionResult> Create(MedicalRecordCreateViewModel vm)
        {
            if (!ModelState.IsValid) return View(PopulateCreateEditVM(vm));

            var record = new MedicalRecord
            {
                PetId = vm.PetId,
                VeterinarianId = vm.VeterinarianId,
                AppointmentId = vm.AppointmentId,
                Diagnosis = vm.Diagnosis,
                Treatment = vm.Treatment,
                Notes = vm.Notes,
                Priority = vm.Priority,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.MedicalRecord.Add(record);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error saving medical record: " + ex.Message);
                return View(PopulateCreateEditVM(vm));
            }

            if (vm.UploadedFiles?.Any() == true)
                await HandleFileUploads(record, vm.UploadedFiles);

            return RedirectToAction(nameof(Index));
        }

        // ---------------- EDIT ----------------
        [Authorize(Roles = "Admin,Vet")]
        public async Task<IActionResult> Edit(int id)
        {
            var record = await _context.MedicalRecord.Include(m => m.Files).FirstOrDefaultAsync(m => m.Id == id);
            if (record == null) return NotFound();

            var vm = new MedicalRecordEditViewModel
            {
                Id = record.Id,
                PetId = record.PetId,
                VeterinarianId = record.VeterinarianId,
                AppointmentId = record.AppointmentId,
                Diagnosis = record.Diagnosis,
                Treatment = record.Treatment,
                Notes = record.Notes,
                Priority = record.Priority,
                ExistingFiles = record.Files.Select(f => new MedicalRecordFileViewModel
                {
                    Id = f.Id,
                    FileName = f.FileName,
                    FilePath = f.FilePath,
                    FileType = f.FileType,
                    FileSize = f.FileSize,
                    UploadedAt = f.UploadedAt,
                    UploadedByName = f.UploadedByName
                }).ToList()
            };

            return View(PopulateCreateEditVM(vm));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Vet")]
        public async Task<IActionResult> Edit(int id, MedicalRecordEditViewModel vm)
        {
            if (id != vm.Id) return NotFound();
            if (!ModelState.IsValid) return View(PopulateCreateEditVM(vm));

            var record = await _context.MedicalRecord.Include(r => r.Files).FirstOrDefaultAsync(r => r.Id == id);
            if (record == null) return NotFound();

            record.Diagnosis = vm.Diagnosis;
            record.Treatment = vm.Treatment;
            record.Notes = vm.Notes;
            record.Priority = vm.Priority;
            record.UpdatedAt = DateTime.UtcNow;

            _context.Update(record);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error updating medical record: " + ex.Message);
                return View(PopulateCreateEditVM(vm));
            }

            if (vm.NewFiles?.Any() == true)
                await HandleFileUploads(record, vm.NewFiles);

            return RedirectToAction(nameof(Index));
        }

        // ---------------- DELETE ----------------
        [Authorize(Roles = "Admin,Vet")]
        public async Task<IActionResult> Delete(int id)
        {
            var record = await _context.MedicalRecord.FindAsync(id);
            if (record == null) return NotFound();
            return View(record);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Vet")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var record = await _context.MedicalRecord.FindAsync(id);
            if (record != null)
            {
                _context.MedicalRecord.Remove(record);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error deleting medical record: " + ex.Message);
                    return View(record);
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // ---------------- SIGN ----------------
        [HttpPost]
        [Authorize(Roles = "Admin,Vet")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sign(int id)
        {
            var record = await _context.MedicalRecord.FindAsync(id);
            if (record == null) return NotFound();

            record.IsSigned = true;
            record.SignedByRole = GetCurrentUserRole();
            record.SignedAt = DateTime.UtcNow;

            _context.Update(record);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error signing medical record: " + ex.Message);
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }

        // ---------------- HELPERS ----------------
        private string GetCurrentUserName() => User?.Identity?.Name ?? "System";
        private string GetCurrentUserRole() => User.IsInRole("Vet") ? "Vet" : User.IsInRole("Admin") ? "Admin" : "PetOwner";

        private async Task HandleFileUploads(MedicalRecord record, IEnumerable<IFormFile> files)
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "medicalrecords");
            Directory.CreateDirectory(uploadsFolder);

            foreach (var file in files)
            {
                var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);

                var fileRecord = new MedicalRecordFile
                {
                    FileName = file.FileName,
                    FilePath = "/uploads/medicalrecords/" + uniqueFileName,
                    FileType = Path.GetExtension(file.FileName),
                    FileSize = file.Length,
                    UploadedAt = DateTime.UtcNow,
                    UploadedByName = GetCurrentUserName(),
                    UploadedByVetId = User.IsInRole("Vet") ? GetCurrentUserId() : 0, // Store actual Vet ID
                    MedicalRecord = record
                };
                _context.MedicalRecordFile.Add(fileRecord);

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    // Optional: log file save error
                }
            }
        }

        private int GetCurrentUserId()
        {
            // Replace with logic to fetch Vet ID from claims or database
            return 1; // placeholder
        }

        private MedicalRecordCreateViewModel PopulateCreateEditVM(MedicalRecordCreateViewModel vm = null)
        {
            vm ??= new MedicalRecordCreateViewModel();
            vm.PetOptions = _context.Pet.Select(p => new SelectListItem { Value = p.PetId.ToString(), Text = p.PetName }).ToList();
            vm.VeterinarianOptions = _context.Veterinarian.Select(v => new SelectListItem { Value = v.VeterinarianId.ToString(), Text = v.Name }).ToList();
            vm.AppointmentOptions = _context.Appointment
                .Select(a => new SelectListItem { Value = a.AppointmentId.ToString(), Text = (a.Date + a.Time).ToString("g") })
                .ToList();
            return vm;
        }

        // ---------------- DOWNLOAD PRESCRIPTION ----------------
        [HttpGet]
        [Authorize(Roles = "Admin,Vet,PetOwner")]
        public async Task<IActionResult> DownloadPrescription(int fileId)
        {
            var file = await _context.MedicalRecordFile
                .Include(f => f.MedicalRecord)
                .ThenInclude(m => m.Pet)
                .FirstOrDefaultAsync(f => f.Id == fileId && f.IsPrescription);

            if (file == null) return NotFound();

            if (User.IsInRole("PetOwner"))
            {
                var ownerId = HttpContext.Session.GetInt32("PetOwnerId");
                if (ownerId == null || file.MedicalRecord.Pet.PetOwnerId != ownerId.Value)
                    return Forbid();
            }

            var path = Path.Combine(_env.WebRootPath, file.FilePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (!System.IO.File.Exists(path)) return NotFound();

            var mime = file.FileType.ToLower() switch
            {
                ".pdf" => "application/pdf",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => "application/octet-stream"
            };

            return PhysicalFile(path, mime, file.FileName);
        }

        // ---------------- NOTIFICATIONS ----------------
        [Authorize(Roles = "PetOwner")]
        public async Task<IActionResult> Notifications()
        {
            var ownerId = HttpContext.Session.GetInt32("PetOwnerId");
            if (ownerId == null) return Forbid();

            var notifications = await _context.Notification
                .Where(n => n.PetOwnerId == ownerId.Value)
                .OrderByDescending(n => n.SentAt)
                .ToListAsync();

            return View(notifications);
        }

    }
}
