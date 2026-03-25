using IntergratedProject.Data;
using IntergratedProject.Models;
using IntergratedProject.ViewModels;
using IntergratedProject.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntergratedProject.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly IntergratedProjectDatabase _context;
        private readonly IEmailSender _emailSender;

        private const int DefaultLeadTimeHours = 2;
        private const int SurgeryLeadTimeHours = 24;
        private const int MaxAppointmentsPerVetPerShift = 10;
        private const int MaxAppointmentsPerSlot = 3;
        private const int SlotStepMinutes = 15;
        private const int SuggestionSearchDays = 7;

        public AppointmentsController(IntergratedProjectDatabase context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }

        // GET: Appointments list
        // Full Index page: filters + table container
      public async Task<IActionResult> Index()
    {
        ViewBag.Vets = await _context.Veterinarian.ToListAsync();
        ViewBag.AppointmentTypes = await _context.AppointmentTypes.ToListAsync();

        // Default view → load today + future appointments
        var today = DateTime.Today;
        var defaultAppointments = await _context.Appointment
            .Include(a => a.AppointmentType)
            .Include(a => a.Pet)
            .Include(a => a.PetOwner)
            .Include(a => a.Veterinarian)
            .Where(a => a.Date >= today)
            .OrderBy(a => a.Date)
            .ThenBy(a => a.Time)
            .Take(20) // default first page
            .ToListAsync();

        return View(defaultAppointments);
    }

    // Partial for filtering, pagination, sorting
    public async Task<IActionResult> Filter(
        string PetName,
        string OwnerName,
        int? VetId,
        int? TypeId,
        DateTime? StartDate,
        DateTime? EndDate,
        int page = 1,
        int pageSize = 20,
        string sortBy = "Date",
        string sortDir = "asc")
    {
        var query = _context.Appointment
            .Include(a => a.AppointmentType)
            .Include(a => a.Pet)
            .Include(a => a.PetOwner)
            .Include(a => a.Veterinarian)
            .AsQueryable();

        // 🔒 Role restrictions
        if (User.IsInRole("Veterinarian"))
        {
            var vetId = HttpContext.Session.GetInt32("VeterinarianId");
            if (vetId == null) return Unauthorized();

            query = query.Where(a => a.VeterinarianId == vetId);
            OwnerName = null;
            VetId = null;
        }
        else if (User.IsInRole("PetOwner"))
        {
            var ownerId = HttpContext.Session.GetInt32("PetOwnerId");
            if (ownerId == null) return Unauthorized();

            query = query.Where(a => a.PetOwnerId == ownerId);
            OwnerName = null;
            VetId = null;
        }
        else if (!User.IsInRole("Admin"))
        {
            return Forbid();
        }

        // 🛠 Filters
        if (!string.IsNullOrEmpty(PetName))
            query = query.Where(a => a.Pet.PetName.ToLower().Contains(PetName.ToLower()));

        if (!string.IsNullOrEmpty(OwnerName))
            query = query.Where(a =>
                a.PetOwner.Name.ToLower().Contains(OwnerName.ToLower()) ||
                a.PetOwner.Email.ToLower().Contains(OwnerName.ToLower()));

        if (VetId.HasValue)
            query = query.Where(a => a.VeterinarianId == VetId.Value);

        if (TypeId.HasValue)
            query = query.Where(a => a.AppointmentTypeId == TypeId.Value);

        if (StartDate.HasValue)
            query = query.Where(a => a.Date >= StartDate.Value);

        if (EndDate.HasValue)
            query = query.Where(a => a.Date <= EndDate.Value);

        // 🔃 Sorting
        query = sortBy switch
        {
            "Pet" => (sortDir == "asc") ? query.OrderBy(a => a.Pet.PetName) : query.OrderByDescending(a => a.Pet.PetName),
            "Owner" => (sortDir == "asc") ? query.OrderBy(a => a.PetOwner.Name) : query.OrderByDescending(a => a.PetOwner.Name),
            "Vet" => (sortDir == "asc") ? query.OrderBy(a => a.Veterinarian.Name) : query.OrderByDescending(a => a.Veterinarian.Name),
            "Type" => (sortDir == "asc") ? query.OrderBy(a => a.AppointmentType.Name) : query.OrderByDescending(a => a.AppointmentType.Name),
            "Date" => (sortDir == "asc") ? query.OrderBy(a => a.Date).ThenBy(a => a.Time) : query.OrderByDescending(a => a.Date).ThenByDescending(a => a.Time),
            _ => query.OrderBy(a => a.Date).ThenBy(a => a.Time)
        };

        // 📄 Pagination
        var totalCount = await query.CountAsync();
        var appointments = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        ViewBag.CurrentPage = page;
        ViewBag.SortBy = sortBy;
        ViewBag.SortDir = sortDir;

        return PartialView("_AppointmentsTable", appointments);
    }






        // GET: Appointment create form
        public async Task<IActionResult> Create(int? petId)
        {
            var petOwnerId = HttpContext.Session.GetInt32("PetOwnerId");
            if (petOwnerId == null) return RedirectToAction("Login", "Account");

            var pets = await _context.Pet
                .Where(p => p.PetOwnerId == petOwnerId.Value)
                .Select(p => new SelectListItem { Value = p.PetId.ToString(), Text = p.PetName })
                .ToListAsync();

            var vm = new AppointmentCreateViewModel
            {
                AppointmentTypes = _context.AppointmentTypes
                    .Select(a => new SelectListItem { Value = a.AppointmentTypeId.ToString(), Text = a.Name })
                    .ToList(),
                AppointmentDateTime = DateTime.Now.AddHours(DefaultLeadTimeHours),
                Pets = pets,
                PetId = petId
            };

            if (petId.HasValue)
            {
                var pet = await _context.Pet.FindAsync(petId.Value);
                if (pet != null)
                {
                    vm.PetName = pet.PetName;

                    if (pet.PrimaryVeterinarianId.HasValue)
                    {
                        var vet = await _context.Veterinarian.FindAsync(pet.PrimaryVeterinarianId.Value);
                        if (vet != null)
                        {
                            vm.VeterinarianId = vet.VeterinarianId;
                            vm.Veterinarians = new List<SelectListItem>
                            {
                                new SelectListItem { Value = vet.VeterinarianId.ToString(), Text = vet.Name }
                            };

                            var nextSlot = await FindNextAvailableSlotForVet(vet.VeterinarianId, vm.AppointmentDateTime, 60, SuggestionSearchDays);
                            if (nextSlot == null)
                                ViewBag.VetUnavailableMessage = $"Dr. {vet.Name} has no available slots. Admin will assign a vet.";
                        }
                    }
                }
            }
            else
            {
                vm.Veterinarians = _context.Veterinarian
                    .Select(v => new SelectListItem { Value = v.VeterinarianId.ToString(), Text = v.Name })
                    .ToList();
            }

            return View(vm);
        }

        // POST: Create booking
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppointmentCreateViewModel vm)
        {
            var petOwnerId = HttpContext.Session.GetInt32("PetOwnerId");
            if (petOwnerId == null)
            {
                ModelState.AddModelError("", "You must be logged in.");
                PopulateDropdowns(vm);
                return View(vm);
            }

            PopulateDropdowns(vm);

            var pet = await _context.Pet.FirstOrDefaultAsync(p => p.PetId == vm.PetId && p.PetOwnerId == petOwnerId.Value);
            if (pet == null)
            {
                ModelState.AddModelError(nameof(vm.PetId), "Invalid pet selection.");
                return View(vm);
            }

            var appointmentType = await _context.AppointmentTypes.FindAsync(vm.AppointmentTypeId);
            if (appointmentType == null)
            {
                ModelState.AddModelError(nameof(vm.AppointmentTypeId), "Invalid appointment type.");
                return View(vm);
            }

            int leadHours = appointmentType.Name.ToLower().Contains("surgery") ? SurgeryLeadTimeHours : DefaultLeadTimeHours;
            if (vm.AppointmentDateTime < DateTime.Now.AddHours(leadHours))
            {
                ModelState.AddModelError(nameof(vm.AppointmentDateTime), $"Requires at least {leadHours} hours notice.");
                return View(vm);
            }

            var appointmentEnd = vm.AppointmentDateTime.AddMinutes(appointmentType.DurationMinutes);

            var petAppointments = await _context.Appointment
                .Include(a => a.AppointmentType)
                .Where(a => a.PetId == vm.PetId && a.Date == vm.AppointmentDateTime.Date)
                .ToListAsync();

            var conflict = petAppointments.FirstOrDefault(a =>
                vm.AppointmentDateTime < a.Date.Add(a.Time).AddMinutes(a.AppointmentType.DurationMinutes) &&
                appointmentEnd > a.Date.Add(a.Time));

            if (conflict != null)
            {
                ModelState.AddModelError("", $"Pet already has an appointment from {conflict.Date.Add(conflict.Time):HH:mm} to {conflict.Date.Add(conflict.Time).AddMinutes(conflict.AppointmentType.DurationMinutes):HH:mm}");
                return View(vm);
            }

            int? assignedVetId = vm.VeterinarianId;

            var appointmentsForDate = await _context.Appointment
                .Include(a => a.AppointmentType)
                .Where(a => a.Date == vm.AppointmentDateTime.Date)
                .ToListAsync();

            var overlappingAppointments = appointmentsForDate.Count(a =>
            {
                var aStart = a.Date + a.Time;
                var aEnd = aStart.AddMinutes(a.AppointmentType.DurationMinutes);
                return vm.AppointmentDateTime < aEnd && appointmentEnd > aStart;
            });

            if (overlappingAppointments >= MaxAppointmentsPerSlot)
            {
                ModelState.AddModelError("", "Time slot full. Pick another.");
                return View(vm);
            }

            string status = assignedVetId.HasValue ? "Approved" : "Pending Vet Assignment";

            var appointment = new Appointment
            {
                AppointmentTypeId = vm.AppointmentTypeId,
                VeterinarianId = assignedVetId,
                PetId = vm.PetId.Value,
                Date = vm.AppointmentDateTime.Date,
                Time = vm.AppointmentDateTime.TimeOfDay,
                AdditionalNotes = vm.AdditionalNotes,
                PetOwnerId = petOwnerId.Value,
                Status = status
            };

            _context.Add(appointment);
            await _context.SaveChangesAsync();

            var notificationService = new NotificationService(_context);
            await notificationService.CreateNotificationAsync(
                $"Your appointment for {pet.PetName} is booked for {vm.AppointmentDateTime:dddd, MMM dd yyyy HH:mm}.",
                "PetOwner",
                petOwnerId: petOwnerId.Value,
                appointmentId: appointment.AppointmentId
            );

            if (assignedVetId.HasValue)
            {
                await notificationService.CreateNotificationAsync(
                    $"New appointment for {pet.PetName} at {vm.AppointmentDateTime:dddd, MMM dd yyyy HH:mm}.",
                    "Veterinarian",
                    vetId: assignedVetId.Value,
                    appointmentId: appointment.AppointmentId
                );
            }

            var owner = await _context.PetOwner.FindAsync(petOwnerId.Value);
            if (owner != null)
            {
                try
                {
                    await _emailSender.SendEmailAsync(owner.Email, "Appointment Booked",
                        $"Dear {owner.Name},<br>Your appointment for {pet.PetName} is scheduled for {vm.AppointmentDateTime:dddd, MMM dd yyyy HH:mm}.");
                }
                catch { }
            }

            //TempData["SuccessMessage"] = "Appointment booked successfully.";
            return RedirectToAction("Details", "Appointments", new { id = appointment.AppointmentId });


        }
        [HttpGet]
        public async Task<JsonResult> GetAvailableVets(DateTime date)
        {
            var vets = await _context.Veterinarian.ToListAsync();
            var appointments = await _context.Appointment
                .Include(a => a.AppointmentType)
                .Where(a => a.Date == date.Date)
                .ToListAsync();

            var result = new List<object>();

            foreach (var vet in vets)
            {
                var vetAppointments = appointments.Where(a => a.VeterinarianId == vet.VeterinarianId).ToList();
                bool isFull = false;
                for (int hour = 0; hour < 24; hour++)
                {
                    for (int min = 0; min < 60; min += 5)
                    {
                        var slotStart = new DateTime(date.Year, date.Month, date.Day, hour, min, 0);
                        var overlapping = vetAppointments.Any(a =>
                        {
                            var start = a.Date + a.Time;
                            var end = start.AddMinutes(a.AppointmentType.DurationMinutes);
                            return slotStart < end && (slotStart.AddMinutes(30) > start);
                        });

                        if (!overlapping)
                        {
                            result.Add(new
                            {
                                VetId = vet.VeterinarianId,
                                VetName = vet.Name,
                                NextAvailableSlot = slotStart.ToString("yyyy-MM-ddTHH:mm")
                            });
                            isFull = false;
                            break;
                        }
                        isFull = true;
                    }
                    if (!isFull) break;
                }

                if (isFull)
                {
                    result.Add(new
                    {
                        VetId = vet.VeterinarianId,
                        VetName = vet.Name,
                        NextAvailableSlot = (string?)null
                    });
                }
            }

            return Json(result);
        }

        private async Task<DateTime?> FindNextAvailableSlotForVet(int vetId, DateTime start, int durationMinutes, int searchDays)
        {
            for (int d = 0; d < searchDays; d++)
            {
                var date = start.AddDays(d).Date;
                var appointments = await _context.Appointment
                    .Include(a => a.AppointmentType)
                    .Where(a => a.Date == date && a.VeterinarianId == vetId)
                    .ToListAsync();

                for (int h = 0; h < 24; h++)
                {
                    for (int m = 0; m < 60; m += 5)
                    {
                        var slotStart = new DateTime(date.Year, date.Month, date.Day, h, m, 0);
                        var overlapping = appointments.Any(a =>
                        {
                            var aStart = a.Date + a.Time;
                            var aEnd = aStart.AddMinutes(a.AppointmentType.DurationMinutes);
                            return slotStart < aEnd && slotStart.AddMinutes(durationMinutes) > aStart;
                        });
                        if (!overlapping) return slotStart;
                    }
                }
            }
            return null;
        }


        private void PopulateDropdowns(AppointmentCreateViewModel vm)
        {
            vm.Pets = _context.Pet.Select(p => new SelectListItem { Value = p.PetId.ToString(), Text = p.PetName }).ToList();
            vm.AppointmentTypes = _context.AppointmentTypes.Select(a => new SelectListItem { Value = a.AppointmentTypeId.ToString(), Text = a.Name }).ToList();
            vm.Veterinarians = _context.Veterinarian.Select(v => new SelectListItem { Value = v.VeterinarianId.ToString(), Text = v.Name }).ToList();
        }

        [HttpGet]
        public IActionResult GetPrice(int appointmentTypeId)
        {
            var type = _context.AppointmentTypes.FirstOrDefault(t => t.AppointmentTypeId == appointmentTypeId);
            if (type == null) return Json(new { price = 0, durationMinutes = 0 });

            return Json(new { price = type.Price, durationMinutes = type.DurationMinutes });
        }


        [HttpGet]
        public async Task<JsonResult> GetBreeds(int speciesId)
        {
            var breeds = await _context.Breed
                .Where(b => b.SpeciesId == speciesId)
                .Select(b => new { id = b.BreedId, name = b.Name })
                .ToListAsync();

            return Json(breeds);
        }

        // GET: Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var appointment = await _context.Appointment
                .Include(a => a.Pet).ThenInclude(p => p.PetOwner)
                .Include(a => a.Veterinarian)
                .Include(a => a.AppointmentType)
                .Include(a => a.Payment) // Include Payment for details
                .FirstOrDefaultAsync(m => m.AppointmentId == id);

            if (appointment == null) return NotFound();

            // Role restrictions
            if (User.IsInRole("PetOwner"))
            {
                var ownerId = HttpContext.Session.GetInt32("PetOwnerId");
                if (ownerId == null || appointment.PetOwnerId != ownerId.Value)
                    return Forbid();
            }
            else if (User.IsInRole("Veterinarian"))
            {
                var vetId = HttpContext.Session.GetInt32("VeterinarianId");
                if (vetId == null || appointment.VeterinarianId != vetId.Value)
                    return Forbid();
            }
            // Admin: no restrictions

            return View(appointment);
        }

        // GET: Payments/Confirmation/5
        public async Task<IActionResult> Confirmation(int id)
        {
            // Get payment with appointment and owner info
            var payment = await _context.Payment
                .Include(p => p.Appointment)
                    .ThenInclude(a => a.Pet)
                .Include(p => p.Appointment)
                    .ThenInclude(a => a.Veterinarian)
                .Include(p => p.Owner)
                .FirstOrDefaultAsync(p => p.PaymentId == id);

            if (payment == null)
            {
                TempData["PopupMessage"] = "Payment not found.";
                TempData["PopupType"] = "error";
                return RedirectToAction("Index", "Appointments");
            }

            var vm = new PaymentConfirmationViewModel
            {
                PaymentId = payment.PaymentId,
                Amount = payment.Amount,
                Currency = payment.Currency,
                PaymentMethod = payment.PaymentMethod,
                PaymentStatus = payment.PaymentStatus,
                TransactionReference = payment.TransactionReference,
                PaidAt = payment.PaidAt,
                Notes = payment.Notes,

                AppointmentId = payment.AppointmentId,
                PetName = payment.Appointment?.Pet?.PetName ?? "Unknown",
                VetName = payment.Appointment?.Veterinarian != null
                          ? $"{payment.Appointment.Veterinarian.Name} {payment.Appointment.Veterinarian.LastName}".Trim()
                          : "Not assigned",
                AppointmentDateTime = payment.Appointment?.Date.Add(payment.Appointment.Time) ?? DateTime.MinValue,
                AppointmentType = payment.Appointment?.AppointmentType?.Name ?? "-"
            };

            return View(vm);
        }



        #region Helpers

        private async Task<DateTime?> FindNextAvailableSlotClinicFallback(DateTime from, int durationMinutes, int daysToSearch)
        {
            for (int day = 0; day <= daysToSearch; day++)
            {
                var date = from.Date.AddDays(day);
                var appointments = await _context.Appointment.Where(a => a.Date == date).ToListAsync();
                var slotCounts = appointments.GroupBy(a => a.Time).ToDictionary(g => g.Key, g => g.Count());

                var cursor = date.Date;
                var dayEnd = date.Date.AddDays(1);

                while (cursor.AddMinutes(durationMinutes) <= dayEnd)
                {
                    int existingAtSlot = slotCounts.ContainsKey(cursor.TimeOfDay) ? slotCounts[cursor.TimeOfDay] : 0;
                    if (existingAtSlot < MaxAppointmentsPerSlot && cursor >= from)
                        return cursor;
                    cursor = cursor.AddMinutes(SlotStepMinutes);
                }
            }
            return null;
        }

        // GET: Reschedule
        [HttpGet]
        public async Task<IActionResult> Reschedule(int id)
        {
            var petOwnerId = HttpContext.Session.GetInt32("PetOwnerId");
            if (petOwnerId == null) return RedirectToAction("Login", "Account");

            var appointment = await _context.Appointment
                .Include(a => a.Pet)
                .Include(a => a.Veterinarian)
                .Include(a => a.AppointmentType)
                .FirstOrDefaultAsync(a => a.AppointmentId == id && a.PetOwnerId == petOwnerId.Value);

            if (appointment == null) return NotFound();

            if (appointment.Date.Add(appointment.Time) < DateTime.UtcNow.AddHours(1))
            {
                TempData["ErrorMessage"] = "Cannot reschedule less than 1 hour before appointment.";
                return RedirectToAction(nameof(Index));
            }

            var vm = new AppointmentCreateViewModel
            {
                AppointmentId = appointment.AppointmentId,
                PetId = appointment.PetId,
                PetName = appointment.Pet.PetName,
                VeterinarianId = appointment.VeterinarianId,
                AppointmentTypeId = appointment.AppointmentTypeId,
                AppointmentDateTime = appointment.Date.Add(appointment.Time),
                AdditionalNotes = appointment.AdditionalNotes,
                AppointmentTypes = _context.AppointmentTypes
                    .Select(a => new SelectListItem { Value = a.AppointmentTypeId.ToString(), Text = a.Name })
                    .ToList(),
                Veterinarians = _context.Veterinarian
                    .Select(v => new SelectListItem { Value = v.VeterinarianId.ToString(), Text = v.Name })
                    .ToList(),
                Pets = _context.Pet
                    .Where(p => p.PetOwnerId == petOwnerId.Value)
                    .Select(p => new SelectListItem { Value = p.PetId.ToString(), Text = p.PetName })
                    .ToList()
            };

            return View("Reschedule", vm);
        }

        // POST: Reschedule
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reschedule(AppointmentCreateViewModel vm)
        {
            var petOwnerId = HttpContext.Session.GetInt32("PetOwnerId");
            if (petOwnerId == null) return RedirectToAction("Login", "Account");

            var appointment = await _context.Appointment
                .Include(a => a.Pet)
                .Include(a => a.AppointmentType)
                .FirstOrDefaultAsync(a => a.AppointmentId == vm.AppointmentId && a.PetOwnerId == petOwnerId.Value);

            if (appointment == null)
            {
                ModelState.AddModelError("", "Appointment not found.");
                return View(vm);
            }

            var appointmentType = await _context.AppointmentTypes.FindAsync(vm.AppointmentTypeId);
            if (appointmentType == null)
            {
                ModelState.AddModelError(nameof(vm.AppointmentTypeId), "Invalid appointment type.");
                return View(vm);
            }

            int leadHours = appointmentType.Name.ToLower().Contains("surgery") ? 24 : 2;
            var newStart = vm.AppointmentDateTime;

            if (newStart < DateTime.Now.AddHours(leadHours))
            {
                ModelState.AddModelError(nameof(vm.AppointmentDateTime), $"Requires {leadHours}h notice.");
                return View(vm);
            }

            var newEnd = newStart.AddMinutes(appointmentType.DurationMinutes);

            var petAppointments = await _context.Appointment
                .Where(a => a.PetId == vm.PetId && a.AppointmentId != vm.AppointmentId && a.Date == newStart.Date)
                .Include(a => a.AppointmentType)
                .ToListAsync();

            if (petAppointments.Any(a => newStart < a.Date.Add(a.Time).AddMinutes(a.AppointmentType.DurationMinutes) && newEnd > a.Date.Add(a.Time)))
            {
                ModelState.AddModelError("", "This pet has a conflicting appointment.");
                return View(vm);
            }

            appointment.Date = newStart.Date;
            appointment.Time = newStart.TimeOfDay;
            appointment.AppointmentTypeId = vm.AppointmentTypeId;
            appointment.AdditionalNotes = vm.AdditionalNotes;
            appointment.Status = "Pending Vet Assignment";

            await _context.SaveChangesAsync();

            var notificationService = new NotificationService(_context);
            await notificationService.CreateNotificationAsync(
                $"Your appointment for {appointment.Pet.PetName} has been rescheduled to {newStart:dddd, MMM dd yyyy HH:mm}.",
                "PetOwner",
                petOwnerId: appointment.PetOwnerId,
                appointmentId: appointment.AppointmentId
            );

            TempData["SuccessMessage"] = "Appointment rescheduled successfully.";
            return RedirectToAction(nameof(Index));
        }




        // GET: Cancel
        [HttpGet]
        public async Task<IActionResult> Cancel(int id)
        {
            var petOwnerId = HttpContext.Session.GetInt32("PetOwnerId");
            if (petOwnerId == null) return RedirectToAction("Login", "Account");

            var appointment = await _context.Appointment
                .Include(a => a.Pet)
                .Include(a => a.Veterinarian)
                .FirstOrDefaultAsync(a => a.AppointmentId == id && a.PetOwnerId == petOwnerId.Value);

            if (appointment == null) return NotFound();

            if (appointment.Date.Add(appointment.Time) < DateTime.UtcNow.AddHours(1))
            {
                TempData["ErrorMessage"] = "Cannot cancel less than 1 hour before appointment.";
                return RedirectToAction(nameof(Index));
            }

            return View(appointment);
        }

        // POST: Cancel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelConfirmed(int id)
        {
            var petOwnerId = HttpContext.Session.GetInt32("PetOwnerId");
            if (petOwnerId == null) return RedirectToAction("Login", "Account");

            var appointment = await _context.Appointment
                .Include(a => a.Pet)
                .FirstOrDefaultAsync(a => a.AppointmentId == id && a.PetOwnerId == petOwnerId.Value);

            if (appointment == null)
            {
                TempData["ErrorMessage"] = "Appointment not found.";
                return RedirectToAction(nameof(Index));
            }

            appointment.Status = "Cancelled";
            await _context.SaveChangesAsync();

            var notificationService = new NotificationService(_context);
            await notificationService.CreateNotificationAsync(
                $"Your appointment for {appointment.Pet.PetName} on {appointment.Date.Add(appointment.Time):dddd, MMM dd yyyy HH:mm} has been cancelled.",
                "PetOwner",
                petOwnerId: appointment.PetOwnerId,
                appointmentId: appointment.AppointmentId
            );

            TempData["SuccessMessage"] = "Appointment cancelled successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Details
    

        // GET: Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var appointment = await _context.Appointment
                .Include(a => a.Pet)
                .Include(a => a.Veterinarian)
                .FirstOrDefaultAsync(m => m.AppointmentId == id);

            if (appointment == null)
            {
                TempData["PopupMessage"] = "Appointment not found.";
                TempData["PopupType"] = "error";
                return RedirectToAction(nameof(Index));
            }

            return View(appointment);
        }

        // POST: DeleteConfirmed
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _context.Appointment.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointment.Remove(appointment);
                await _context.SaveChangesAsync();

                TempData["PopupMessage"] = "Appointment deleted successfully.";
                TempData["PopupType"] = "success";
            }
            else
            {
                TempData["PopupMessage"] = "Error deleting appointment.";
                TempData["PopupType"] = "error";
            }

            return RedirectToAction(nameof(Index));
        }

        #endregion
    }
}



//using IntergratedProject.Data;
//using IntergratedProject.Models;
//using IntergratedProject.ViewModels;
//using IntergratedProject.Security;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace IntergratedProject.Controllers
//{
//    public class AppointmentsController : Controller
//    {
//        private readonly IntergratedProjectDatabase _context;
//        private readonly IEmailSender _emailSender;

//        private const int DefaultLeadTimeHours = 2;
//        private const int SurgeryLeadTimeHours = 24;
//        private const int MaxAppointmentsPerVetPerShift = 10;
//        private const int MaxAppointmentsPerSlot = 3;
//        private const int SlotStepMinutes = 15;
//        private const int SuggestionSearchDays = 7;

//        public AppointmentsController(IntergratedProjectDatabase context, IEmailSender emailSender)
//        {
//            _context = context;
//            _emailSender = emailSender;
//        }

//        // GET: Appointment create form
//        public async Task<IActionResult> Create(int? petId)
//        {
//            var petOwnerId = HttpContext.Session.GetInt32("PetOwnerId");
//            if (petOwnerId == null)
//                return RedirectToAction("Login", "Account");

//            var pets = await _context.Pet
//                .Where(p => p.PetOwnerId == petOwnerId.Value)
//                .Select(p => new SelectListItem
//                {
//                    Value = p.PetId.ToString(),
//                    Text = p.PetName
//                }).ToListAsync();

//            var vm = new AppointmentCreateViewModel
//            {
//                AppointmentTypes = _context.AppointmentTypes
//                    .Select(a => new SelectListItem { Value = a.AppointmentTypeId.ToString(), Text = a.Name })
//                    .ToList(),
//                AppointmentDateTime = DateTime.Now.AddHours(DefaultLeadTimeHours),
//                Pets = pets,
//                PetId = petId
//            };

//            if (petId.HasValue)
//            {
//                var pet = await _context.Pet.FindAsync(petId.Value);
//                if (pet == null)
//                    return RedirectToAction("Dashboard", "Pets");

//                vm.PetName = pet.PetName;

//                if (pet.PrimaryVeterinarianId.HasValue)
//                {
//                    var vet = await _context.Veterinarian.FindAsync(pet.PrimaryVeterinarianId.Value);
//                    if (vet != null)
//                    {
//                        vm.VeterinarianId = vet.VeterinarianId;
//                        vm.Veterinarians = new List<SelectListItem>
//                        {
//                            new SelectListItem { Value = vet.VeterinarianId.ToString(), Text = vet.Name }
//                        };
//                    }
//                }
//            }
//            else
//            {
//                vm.Veterinarians = _context.Veterinarian
//                    .Select(v => new SelectListItem { Value = v.VeterinarianId.ToString(), Text = v.Name })
//                    .ToList();
//            }

//            return View(vm);
//        }

//        // POST: Create booking
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create(AppointmentCreateViewModel vm)
//        {
//            var petOwnerId = HttpContext.Session.GetInt32("PetOwnerId");
//            if (petOwnerId == null)
//            {
//                ModelState.AddModelError("", "You must be logged in to book an appointment.");
//                PopulateDropdowns(vm);
//                return View(vm);
//            }

//            PopulateDropdowns(vm);

//            // Validate pet
//            var pet = await _context.Pet.FindAsync(vm.PetId);
//            if (pet == null)
//            {
//                ModelState.AddModelError(nameof(vm.PetId), "Selected pet does not exist.");
//                return View(vm);
//            }

//            // Validate appointment type
//            var appointmentType = await _context.AppointmentTypes.FindAsync(vm.AppointmentTypeId);
//            if (appointmentType == null)
//            {
//                ModelState.AddModelError(nameof(vm.AppointmentTypeId), "Invalid appointment type selected.");
//                return View(vm);
//            }

//            // Lead time validation (skip if emergency)
//            int leadHours = appointmentType.Name?.ToLower().Contains("surgery") == true ? SurgeryLeadTimeHours : DefaultLeadTimeHours;
//            var appointmentStart = vm.AppointmentDateTime;

//            if (!vm.IsEmergency && appointmentStart < DateTime.Now.AddHours(leadHours))
//            {
//                ModelState.AddModelError(nameof(vm.AppointmentDateTime),
//                    $"Booking requires at least {leadHours} hours notice for this appointment type.");
//                return View(vm);
//            }

//            if (appointmentStart < DateTime.Now)
//            {
//                ModelState.AddModelError(nameof(vm.AppointmentDateTime), "You cannot book an appointment in the past.");
//                return View(vm);
//            }

//            var appointmentEnd = appointmentStart.AddMinutes(appointmentType.DurationMinutes);

//            // Pet conflict check
//            if (!vm.IsEmergency)
//            {
//                var petAppointments = await _context.Appointment
//                    .Include(a => a.AppointmentType)
//                    .Where(a => a.PetId == vm.PetId && a.Date == appointmentStart.Date)
//                    .ToListAsync();

//                bool petConflict = petAppointments.Any(a =>
//                {
//                    var existingStart = a.Date.Date.Add(a.Time);
//                    var existingEnd = existingStart.AddMinutes(a.AppointmentType.DurationMinutes);
//                    return appointmentStart < existingEnd && appointmentEnd > existingStart;
//                });

//                if (petConflict)
//                {
//                    ModelState.AddModelError("", "This pet already has an overlapping appointment. Choose another time.");
//                    return View(vm);
//                }
//            }

//            // Slot capacity check
//            int concurrentAtSlot = await _context.Appointment
//                .CountAsync(a => a.Date == appointmentStart.Date && a.Time == appointmentStart.TimeOfDay);

//            if (!vm.IsEmergency && concurrentAtSlot >= MaxAppointmentsPerSlot)
//            {
//                ModelState.AddModelError("", "This time slot is full. Please pick a different time.");
//                var suggestion = await FindNextAvailableSlotClinicFallback(appointmentStart, appointmentType.DurationMinutes, SuggestionSearchDays);
//                if (suggestion != null)
//                    ModelState.AddModelError("", $"Next available slot: {suggestion:dddd, MMM dd yyyy HH:mm}");
//                return View(vm);
//            }

//            // Vet assignment & specialty constraint
//            int? assignedVetId = null;
//            if (vm.VeterinarianId.HasValue && vm.VeterinarianId > 0)
//            {
//                var vet = await _context.Veterinarian
//                    .Include(v => v.Shifts).ThenInclude(s => s.Breaks)
//                    .Include(v => v.Specialties)
//                    .FirstOrDefaultAsync(v => v.VeterinarianId == vm.VeterinarianId.Value);

//                if (vet != null)
//                {
//                    // Check vet specialty
//                    if (appointmentType.RequiredSpecialtyId.HasValue &&
//                        !vet.Specialties.Any(s => s.SpecialtyId == appointmentType.RequiredSpecialtyId.Value))
//                    {
//                        ModelState.AddModelError("", $"Dr. {vet.Name} cannot perform this appointment type.");
//                        return View(vm);
//                    }

//                    if (!vm.IsEmergency)
//                    {
//                        var nextSlot = await FindNextAvailableSlotForVet(vet.VeterinarianId, appointmentStart, appointmentType.DurationMinutes, SuggestionSearchDays);
//                        if (nextSlot == null)
//                        {
//                            ViewBag.VetUnavailableMessage = $"Dr. {vet.Name} has no available slots. Admin will assign another vet.";
//                        }
//                        else
//                        {
//                            assignedVetId = vet.VeterinarianId;
//                        }
//                    }
//                    else
//                    {
//                        // Emergency: assign vet regardless of normal availability
//                        assignedVetId = vet.VeterinarianId;
//                    }
//                }
//            }

//            // Determine status
//            string status = assignedVetId.HasValue ? "Approved" : "Pending Vet Assignment";

//            // Save appointment
//            var appointment = new Appointment
//            {
//                AppointmentTypeId = vm.AppointmentTypeId,
//                VeterinarianId = assignedVetId,
//                PetId = vm.PetId.Value,
//                Date = appointmentStart.Date,
//                Time = appointmentStart.TimeOfDay,
//                AdditionalNotes = vm.AdditionalNotes,
//                PetOwnerId = petOwnerId.Value,
//                Status = status,
//                IsEmergency = vm.IsEmergency
//            };

//            _context.Add(appointment);
//            await _context.SaveChangesAsync();

//            // Audit trail for admin actions
//            if (!assignedVetId.HasValue)
//            {
//                _context.AppointmentAudits.Add(new AppointmentAudit
//                {
//                    AppointmentId = appointment.AppointmentId,
//                    OldVetId = null,
//                    NewVetId = assignedVetId,
//                    ChangedByAdminId = HttpContext.Session.GetInt32("AdminId"),
//                    ChangeDate = DateTime.Now,
//                    Action = "Pending Vet Assignment"
//                });
//                await _context.SaveChangesAsync();
//            }

//            // Send email confirmation
//            var owner = await _context.PetOwner.FindAsync(petOwnerId.Value);
//            if (owner != null)
//            {
//                string message = $"Dear {owner.Name},\n\n" +
//                    $"Your appointment for {pet.PetName} has been scheduled.\n" +
//                    $"Type: {appointmentType.Name}\n" +
//                    $"Date: {appointmentStart:dddd, MMM dd yyyy}\n" +
//                    $"Time: {appointmentStart:hh:mm tt}\n";

//                if (assignedVetId.HasValue)
//                {
//                    var vetEntity = await _context.Veterinarian.FindAsync(assignedVetId.Value);
//                    if (vetEntity != null)
//                        message += $"Veterinarian: Dr. {vetEntity.Name}\n";
//                }

//                await _emailSender.SendEmailAsync(owner.Email, "Appointment Confirmation", message);
//            }

//            return RedirectToAction(nameof(Index));
//        }

//        private void PopulateDropdowns(AppointmentCreateViewModel vm)
//        {
//            vm.AppointmentTypes = _context.AppointmentTypes
//                .Select(a => new SelectListItem { Value = a.AppointmentTypeId.ToString(), Text = a.Name })
//                .ToList();

//            vm.Veterinarians ??= _context.Veterinarian
//                .Select(v => new SelectListItem { Value = v.VeterinarianId.ToString(), Text = v.Name })
//                .ToList();

//            vm.Pets ??= _context.Pet
//                .Select(p => new SelectListItem { Value = p.PetId.ToString(), Text = p.PetName })
//                .ToList();
//        }

//     public JsonResult GetBreeds(int speciesId)
//        {
//            var breeds = _context.Breed
//                .Where(b => b.SpeciesId == speciesId)
//                .Select(b => new { breedId = b.BreedId, name = b.Name })
//                .ToList();

//            return Json(breeds);
//        }


//        #region Helpers

//        private async Task<DateTime?> FindNextAvailableSlotClinicFallback(DateTime from, int durationMinutes, int daysToSearch)
//        {
//            for (int day = 0; day <= daysToSearch; day++)
//            {
//                var date = from.Date.AddDays(day);
//                var appointments = await _context.Appointment.Where(a => a.Date == date).ToListAsync();
//                var slotCounts = appointments.GroupBy(a => a.Time).ToDictionary(g => g.Key, g => g.Count());

//                var cursor = date.Date;
//                var dayEnd = date.Date.AddDays(1);

//                while (cursor.AddMinutes(durationMinutes) <= dayEnd)
//                {
//                    int existingAtSlot = slotCounts.ContainsKey(cursor.TimeOfDay) ? slotCounts[cursor.TimeOfDay] : 0;
//                    if (existingAtSlot < MaxAppointmentsPerSlot && cursor >= from)
//                        return cursor;
//                    cursor = cursor.AddMinutes(SlotStepMinutes);
//                }
//            }
//            return null;
//        }

//        private async Task<DateTime?> FindNextAvailableSlotForVet(int veterinarianId, DateTime from, int durationMinutes, int daysToSearch)
//        {
//            var vet = await _context.Veterinarian
//                .Include(v => v.Shifts).ThenInclude(s => s.Breaks)
//                .FirstOrDefaultAsync(v => v.VeterinarianId == veterinarianId);

//            if (vet == null || vet.Shifts == null) return null;

//            for (int day = 0; day <= daysToSearch; day++)
//            {
//                var date = from.Date.AddDays(day);
//                var shifts = vet.Shifts.Where(s => s.DayOfWeek == date.DayOfWeek).ToList();
//                if (!shifts.Any()) continue;

//                var vetAppointments = await _context.Appointment
//                    .Include(a => a.AppointmentType)
//                    .Where(a => a.VeterinarianId == veterinarianId && a.Date == date)
//                    .ToListAsync();

//                var slotCounts = await _context.Appointment
//                    .Where(a => a.Date == date)
//                    .GroupBy(a => a.Time)
//                    .ToDictionaryAsync(g => g.Key, g => g.Count());

//                foreach (var shift in shifts)
//                {
//                    var cursor = date.Date.Add(shift.StartTime);
//                    var shiftEnd = date.Date.Add(shift.EndTime);

//                    while (cursor.AddMinutes(durationMinutes) <= shiftEnd)
//                    {
//                        bool inBreak = (shift.Breaks ?? Enumerable.Empty<VetBreak>())
//                            .Any(b => cursor.TimeOfDay < b.EndTime && cursor.AddMinutes(durationMinutes).TimeOfDay > b.StartTime);
//                        if (inBreak) { cursor = cursor.AddMinutes(SlotStepMinutes); continue; }

//                        bool conflict = vetAppointments.Any(a =>
//                        {
//                            var s = a.Date.Date.Add(a.Time);
//                            var e = s.AddMinutes(a.AppointmentType.DurationMinutes);
//                            return cursor < e && cursor.AddMinutes(durationMinutes) > s;
//                        });
//                        if (conflict) { cursor = cursor.AddMinutes(SlotStepMinutes); continue; }

//                        int existingAtSlot = slotCounts.ContainsKey(cursor.TimeOfDay) ? slotCounts[cursor.TimeOfDay] : 0;
//                        if (existingAtSlot >= MaxAppointmentsPerSlot) { cursor = cursor.AddMinutes(SlotStepMinutes); continue; }

//                        if (cursor >= from) return cursor;

//                        cursor = cursor.AddMinutes(SlotStepMinutes);
//                    }
//                }
//            }
//            return null;
//        }

//    }
//}
