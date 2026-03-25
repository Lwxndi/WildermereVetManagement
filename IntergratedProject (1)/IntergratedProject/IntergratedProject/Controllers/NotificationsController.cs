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
    public class NotificationsController : Controller
    {
        private readonly IntergratedProjectDatabase _context;
        private readonly NotificationService _notificationService;

        public NotificationsController(IntergratedProjectDatabase context,NotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        // GET: Notifications
        public async Task<IActionResult> Index(string filter, int page = 1)
        {
            var role = HttpContext.Session.GetString("Role");
            var userId = HttpContext.Session.GetInt32("UserId");

            if (string.IsNullOrEmpty(role) || userId == null)
                return RedirectToAction("Login", "Account");

            const int pageSize = 10;
            IQueryable<Notification> query = _context.Notification
                .Include(n => n.Appointment)
                .Include(n => n.MedicalRecord)
                .Include(n => n.PetOwner)
                .Include(n => n.Veterinarian);

            // Filter by role
            switch (role)
            {
                case "PetOwner":
                    query = query.Where(n => n.PetOwnerId == userId);
                    break;
                case "Veterinarian":
                    query = query.Where(n => n.VeterinarianId == userId);
                    break;
                case "Admin":
                    query = query.Where(n => n.AdminId == userId);
                    break;
                default:
                    return RedirectToAction("Index", "Home");
            }

            // Filter by read/unread
            if (filter == "read")
                query = query.Where(n => n.IsRead);
            else if (filter == "unread")
                query = query.Where(n => !n.IsRead);

            // Pagination
            var totalCount = await query.CountAsync();
            var notifications = await query
                .OrderByDescending(n => n.SentAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = (int)Math.Ceiling(totalCount / (double)pageSize);
            ViewData["Filter"] = filter;

            return View(notifications);
        }


        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var notification = await _context.Notification.FindAsync(id);
            if (notification != null)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }


        // GET: Notifications/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notification = await _context.Notification
                .Include(n => n.Appointment)
                .Include(n => n.MedicalRecord)
                .Include(n => n.PetOwner)
                .Include(n => n.Veterinarian)
                .FirstOrDefaultAsync(m => m.NotificationId == id);
            if (notification == null)
            {
                return NotFound();
            }

            return View(notification);
        }

        // GET: Notifications/Create
        public IActionResult Create()
        {
            ViewData["AppointmentId"] = new SelectList(_context.Appointment, "AppointmentId", "AppointmentId");
            ViewData["MedicalRecordId"] = new SelectList(_context.MedicalRecord, "Id", "Diagnosis");
            ViewData["PetOwnerId"] = new SelectList(_context.PetOwner, "OwnerId", "Address");
            ViewData["VeterinarianId"] = new SelectList(_context.Veterinarian, "VeterinarianId", "Email");
            return View();
        }

        // POST: Notifications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NotificationId,Message,SentAt,IsRead,RecipientRole,PetOwnerId,VeterinarianId,AdminId,AppointmentId,MedicalRecordId")] Notification notification)
        {
            if (ModelState.IsValid)
            {
                _context.Add(notification);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AppointmentId"] = new SelectList(_context.Appointment, "AppointmentId", "AppointmentId", notification.AppointmentId);
            ViewData["MedicalRecordId"] = new SelectList(_context.MedicalRecord, "Id", "Diagnosis", notification.MedicalRecordId);
            ViewData["PetOwnerId"] = new SelectList(_context.PetOwner, "OwnerId", "Address", notification.PetOwnerId);
            ViewData["VeterinarianId"] = new SelectList(_context.Veterinarian, "VeterinarianId", "Email", notification.VeterinarianId);
            return View(notification);
        }

        // GET: Notifications/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notification = await _context.Notification.FindAsync(id);
            if (notification == null)
            {
                return NotFound();
            }
            ViewData["AppointmentId"] = new SelectList(_context.Appointment, "AppointmentId", "AppointmentId", notification.AppointmentId);
            ViewData["MedicalRecordId"] = new SelectList(_context.MedicalRecord, "Id", "Diagnosis", notification.MedicalRecordId);
          ViewData["PetOwnerId"] = new SelectList(_context.PetOwner.Select(po => new {
                po.OwnerId,
                Name = po.Name
            }), "PetOwnerId", "Name");

            ViewData["VeterinarianId"] = new SelectList(_context.Veterinarian.Select(v => new {
                v.VeterinarianId,
                Name = v.Name
            }), "VeterinarianId", "Name");

            return View(notification);
        }

        // POST: Notifications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NotificationId,Message,SentAt,IsRead,RecipientRole,PetOwnerId,VeterinarianId,AdminId,AppointmentId,MedicalRecordId")] Notification notification)
        {
            if (id != notification.NotificationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(notification);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NotificationExists(notification.NotificationId))
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
            ViewData["AppointmentId"] = new SelectList(_context.Appointment, "AppointmentId", "AppointmentId", notification.AppointmentId);
            ViewData["MedicalRecordId"] = new SelectList(_context.MedicalRecord, "Id", "Diagnosis", notification.MedicalRecordId);
            ViewData["PetOwnerId"] = new SelectList(_context.PetOwner.Select(po => new {
                po.OwnerId,
                Name = po.Name
            }), "PetOwnerId", "Name");

            ViewData["VeterinarianId"] = new SelectList(_context.Veterinarian.Select(v => new {
                v.VeterinarianId,
                Name = v.Name
            }), "VeterinarianId", "Name");
            return View(notification);
        }

        // GET: Notifications/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notification = await _context.Notification
                .Include(n => n.Appointment)
                .Include(n => n.MedicalRecord)
                .Include(n => n.PetOwner)
                .Include(n => n.Veterinarian)
                .FirstOrDefaultAsync(m => m.NotificationId == id);
            if (notification == null)
            {
                return NotFound();
            }

            return View(notification);
        }

        // POST: Notifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var notification = await _context.Notification.FindAsync(id);
            if (notification != null)
            {
                _context.Notification.Remove(notification);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NotificationExists(int id)
        {
            return _context.Notification.Any(e => e.NotificationId == id);
        }
    }
}
