using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IntergratedProject.Data;
using IntergratedProject.Models;

namespace IntergratedProject.Controllers
{
    public class MedicalRecordFilesController : Controller
    {
        private readonly IntergratedProjectDatabase _context;

        public MedicalRecordFilesController(IntergratedProjectDatabase context)
        {
            _context = context;
        }

        // GET: MedicalRecordFiles
        public async Task<IActionResult> Index()
        {
            var intergratedProjectDatabase = _context.MedicalRecordFile.Include(m => m.MedicalRecord);
            return View(await intergratedProjectDatabase.ToListAsync());
        }

        // GET: MedicalRecordFiles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicalRecordFile = await _context.MedicalRecordFile
                .Include(m => m.MedicalRecord)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (medicalRecordFile == null)
            {
                return NotFound();
            }

            return View(medicalRecordFile);
        }

        // GET: MedicalRecordFiles/Create
        public IActionResult Create()
        {
            ViewData["MedicalRecordId"] = new SelectList(_context.MedicalRecord, "Id", "Diagnosis");
            return View();
        }

        // POST: MedicalRecordFiles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MedicalRecordId,FileName,FilePath,FileType,FileSize,UploadedAt,UpdatedAt,UploadedByVetId,UploadedByName")] MedicalRecordFile medicalRecordFile)
        {
            if (ModelState.IsValid)
            {
                _context.Add(medicalRecordFile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MedicalRecordId"] = new SelectList(_context.MedicalRecord, "Id", "Diagnosis", medicalRecordFile.MedicalRecordId);
            return View(medicalRecordFile);
        }

        // GET: MedicalRecordFiles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicalRecordFile = await _context.MedicalRecordFile.FindAsync(id);
            if (medicalRecordFile == null)
            {
                return NotFound();
            }
            ViewData["MedicalRecordId"] = new SelectList(_context.MedicalRecord, "Id", "Diagnosis", medicalRecordFile.MedicalRecordId);
            return View(medicalRecordFile);
        }

        // POST: MedicalRecordFiles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MedicalRecordId,FileName,FilePath,FileType,FileSize,UploadedAt,UpdatedAt,UploadedByVetId,UploadedByName")] MedicalRecordFile medicalRecordFile)
        {
            if (id != medicalRecordFile.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(medicalRecordFile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MedicalRecordFileExists(medicalRecordFile.Id))
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
            ViewData["MedicalRecordId"] = new SelectList(_context.MedicalRecord, "Id", "Diagnosis", medicalRecordFile.MedicalRecordId);
            return View(medicalRecordFile);
        }

        // GET: MedicalRecordFiles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicalRecordFile = await _context.MedicalRecordFile
                .Include(m => m.MedicalRecord)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (medicalRecordFile == null)
            {
                return NotFound();
            }

            return View(medicalRecordFile);
        }

        // POST: MedicalRecordFiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var medicalRecordFile = await _context.MedicalRecordFile.FindAsync(id);
            if (medicalRecordFile != null)
            {
                _context.MedicalRecordFile.Remove(medicalRecordFile);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MedicalRecordFileExists(int id)
        {
            return _context.MedicalRecordFile.Any(e => e.Id == id);
        }
    }
}
