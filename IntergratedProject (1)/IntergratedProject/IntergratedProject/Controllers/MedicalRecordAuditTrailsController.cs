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
    public class MedicalRecordAuditTrailsController : Controller
    {
        private readonly IntergratedProjectDatabase _context;

        public MedicalRecordAuditTrailsController(IntergratedProjectDatabase context)
        {
            _context = context;
        }

        // GET: MedicalRecordAuditTrails
        public async Task<IActionResult> Index()
        {
            var intergratedProjectDatabase = _context.MedicalRecordAuditTrail.Include(m => m.MedicalRecord);
            return View(await intergratedProjectDatabase.ToListAsync());
        }

        // GET: MedicalRecordAuditTrails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicalRecordAuditTrail = await _context.MedicalRecordAuditTrail
                .Include(m => m.MedicalRecord)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (medicalRecordAuditTrail == null)
            {
                return NotFound();
            }

            return View(medicalRecordAuditTrail);
        }

        // GET: MedicalRecordAuditTrails/Create
        public IActionResult Create()
        {
            ViewData["MedicalRecordId"] = new SelectList(_context.MedicalRecord, "Id", "Diagnosis");
            return View();
        }

        // POST: MedicalRecordAuditTrails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MedicalRecordId,Action,PerformedByUserId,PerformedByRole,Timestamp,OldValue,NewValue,ChangeData")] MedicalRecordAuditTrail medicalRecordAuditTrail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(medicalRecordAuditTrail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MedicalRecordId"] = new SelectList(_context.MedicalRecord, "Id", "Diagnosis", medicalRecordAuditTrail.MedicalRecordId);
            return View(medicalRecordAuditTrail);
        }

        // GET: MedicalRecordAuditTrails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicalRecordAuditTrail = await _context.MedicalRecordAuditTrail.FindAsync(id);
            if (medicalRecordAuditTrail == null)
            {
                return NotFound();
            }
            ViewData["MedicalRecordId"] = new SelectList(_context.MedicalRecord, "Id", "Diagnosis", medicalRecordAuditTrail.MedicalRecordId);
            return View(medicalRecordAuditTrail);
        }

        // POST: MedicalRecordAuditTrails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MedicalRecordId,Action,PerformedByUserId,PerformedByRole,Timestamp,OldValue,NewValue,ChangeData")] MedicalRecordAuditTrail medicalRecordAuditTrail)
        {
            if (id != medicalRecordAuditTrail.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(medicalRecordAuditTrail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MedicalRecordAuditTrailExists(medicalRecordAuditTrail.Id))
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
            ViewData["MedicalRecordId"] = new SelectList(_context.MedicalRecord, "Id", "Diagnosis", medicalRecordAuditTrail.MedicalRecordId);
            return View(medicalRecordAuditTrail);
        }

        // GET: MedicalRecordAuditTrails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicalRecordAuditTrail = await _context.MedicalRecordAuditTrail
                .Include(m => m.MedicalRecord)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (medicalRecordAuditTrail == null)
            {
                return NotFound();
            }

            return View(medicalRecordAuditTrail);
        }

        // POST: MedicalRecordAuditTrails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var medicalRecordAuditTrail = await _context.MedicalRecordAuditTrail.FindAsync(id);
            if (medicalRecordAuditTrail != null)
            {
                _context.MedicalRecordAuditTrail.Remove(medicalRecordAuditTrail);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MedicalRecordAuditTrailExists(int id)
        {
            return _context.MedicalRecordAuditTrail.Any(e => e.Id == id);
        }
    }
}
