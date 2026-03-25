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
    public class PaymentsController : Controller
    {
        private readonly IntergratedProjectDatabase _context;
        private readonly IEmailSender _emailSender;
        private readonly NotificationService _notificationService;
        public PaymentsController(IntergratedProjectDatabase context, IEmailSender emailSender, NotificationService notificationService)
        {
            _context = context;
            _emailSender = emailSender;
            _notificationService = notificationService;
        }

        // GET: Payments
        public async Task<IActionResult> Index()
        {
            var intergratedProjectDatabase = _context.Payment.Include(p => p.Appointment).Include(p => p.Owner);
            return View(await intergratedProjectDatabase.ToListAsync());
        }

        // GET: Payments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payment
                .Include(p => p.Appointment)
                .Include(p => p.Owner)
                .FirstOrDefaultAsync(m => m.PaymentId == id);
            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        // GET: Payments/Create
        public IActionResult Create()
        {
            ViewData["AppointmentId"] = new SelectList(_context.Appointment, "AppointmentId", "AppointmentId");
            ViewData["OwnerId"] = new SelectList(_context.PetOwner, "OwnerId", "Address");
            return View();
        }

        // POST: Payments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PaymentCheckoutViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var userId = int.Parse(User.FindFirst("OwnerId").Value);
            var status = vm.SelectedMethod == PaymentMethods.Cash ? "Pending" : "Paid";
            var maskedCard = vm.SelectedMethod == PaymentMethods.Card ? "**** **** **** 4242" : null;
            var payment = new Payment
            {
                AppointmentId = vm.AppointmentId,
                OwnerId = userId,
                Amount = vm.Amount,
                Currency = "ZAR",                       
                PaymentMethod = vm.SelectedMethod.ToString(), 
                PaymentStatus = status,
                TransactionReference = GenerateTransactionReference(vm.SelectedMethod),
                CreatedAt = DateTime.UtcNow,
                MaskedCard = maskedCard
            };

            _context.Add(payment);

            // Update the appointment status to "Paid"
            var appointment = await _context.Appointment.FindAsync(vm.AppointmentId);
            if (appointment != null)
            {
                appointment.Status = "Paid";
                _context.Update(appointment);
            }

            await _context.SaveChangesAsync();

            // Redirect to confirmation page
            return RedirectToAction("Confirmation", new { id = payment.PaymentId });
        }


        // GET: Payments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payment.FindAsync(id);
            if (payment == null)
            {
                return NotFound();
            }
            ViewData["AppointmentId"] = new SelectList(_context.Appointment, "AppointmentId", "AppointmentId", payment.AppointmentId);
            ViewData["OwnerId"] = new SelectList(_context.PetOwner, "OwnerId", "Address", payment.OwnerId);
            return View(payment);
        }

        // POST: Payments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PaymentId,AppointmentId,OwnerId,Amount,Currency,PaymentMethod,PaymentStatus,TransactionReference,CreatedAt,UpdatedAt,PaidAt,Notes,MaskedCard,IsTest")] Payment payment)
        {
            if (id != payment.PaymentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(payment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaymentExists(payment.PaymentId))
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
            ViewData["AppointmentId"] = new SelectList(_context.Appointment, "AppointmentId", "AppointmentId", payment.AppointmentId);
            ViewData["OwnerId"] = new SelectList(_context.PetOwner, "OwnerId", "Address", payment.OwnerId);
            return View(payment);
        }

        // GET: Payments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payment
                .Include(p => p.Appointment)
                .Include(p => p.Owner)
                .FirstOrDefaultAsync(m => m.PaymentId == id);
            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        // POST: Payments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var payment = await _context.Payment.FindAsync(id);
            if (payment != null)
            {
                _context.Payment.Remove(payment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PaymentExists(int id)
        {
            return _context.Payment.Any(e => e.PaymentId == id);
        }


        public async Task<IActionResult> Checkout(int appointmentId)
        {
            var appointment = await _context.Appointment
                .Include(a => a.Pet)
                .Include(a => a.AppointmentType)
                .Include(a => a.PetOwner)
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);

            if (appointment == null) return NotFound();

            var existingPayment = await _context.Payment
                .FirstOrDefaultAsync(p => p.AppointmentId == appointmentId && p.PaymentStatus == "Paid");

            if (existingPayment != null)
                return BadRequest("This appointment has already been paid.");

            var vm = new PaymentCheckoutViewModel
            {
                AppointmentId = appointment.AppointmentId,
                PetName = appointment.Pet.PetName,
                AppointmentType = appointment.AppointmentType.Name,
                Date = appointment.Date,
                Time = appointment.Time,
                Amount = appointment.AppointmentType.Price
            };

            return View(vm);
        }

        // POST: Payments/ProcessPayment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessPayment(PaymentCheckoutViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Checkout", model);

            var userId = int.Parse(User.FindFirst("OwnerId").Value); // adjust based on auth
            var appointment = await _context.Appointment
                .Include(a => a.PetOwner)
                .Include(a => a.Pet)
                .Include(a => a.AppointmentType)
                .FirstOrDefaultAsync(a => a.AppointmentId == model.AppointmentId);

            if (appointment == null) return NotFound();

            // Determine payment status
            string status = model.SelectedMethod == PaymentMethods.Cash ? "Pending" : "Paid";

            // Mock masked card for Card payments
            string maskedCard = model.SelectedMethod == PaymentMethods.Card ? "**** **** **** 4242" : null;

            var payment = new Payment
            {
                AppointmentId = appointment.AppointmentId,
                OwnerId = userId,
                Amount = model.Amount,
                Currency = "ZAR",
                PaymentMethod = model.SelectedMethod.ToString(),
                PaymentStatus = status,
                TransactionReference = GenerateTransactionReference(model.SelectedMethod),
                CreatedAt = DateTime.UtcNow,
                MaskedCard = maskedCard
            };

            _context.Payment.Add(payment);
            await _context.SaveChangesAsync();

            // Send Notification
            string notifMessage = $"Payment {status} for appointment {appointment.AppointmentId}. Amount: R{payment.Amount}, Method: {payment.PaymentMethod}, Ref: {payment.TransactionReference}";
            await _notificationService.CreateNotificationAsync(notifMessage, "PetOwner", petOwnerId: userId, appointmentId: appointment.AppointmentId);

            // Send Email
            string emailBody = $@"
                <h3>Payment {status}</h3>
                <p>Appointment: {appointment.Pet.PetName} ({appointment.AppointmentType.Name})</p>
                <p>Date: {appointment.Date.ToShortDateString()} Time: {appointment.Time}</p>
                <p>Amount: R{payment.Amount}</p>
                <p>Payment Method: {payment.PaymentMethod}</p>
                <p>Transaction Reference: {payment.TransactionReference}</p>";

            if (!string.IsNullOrEmpty(maskedCard))
                emailBody += $"<p>Card: {maskedCard}</p>";

            await _emailSender.SendEmailAsync(appointment.PetOwner.Email, "Payment Confirmation", emailBody);

            return RedirectToAction("PaymentConfirmation", new { paymentId = payment.PaymentId });
        }



        // Helper for transaction reference

        private string GenerateTransactionReference(PaymentMethods method)
        {
            var prefix = method.ToString().ToUpper();
            var random = new Random();
            return $"{prefix}-{DateTime.UtcNow:yyyyMMddHHmmss}-{random.Next(100, 999)}";
        }

        // GET: Payments/PaymentConfirmation/5
        public async Task<IActionResult> PaymentConfirmation(int paymentId)
        {
            var payment = await _context.Payment
                .Include(p => p.Appointment)
                .ThenInclude(a => a.Pet)
                .Include(p => p.Appointment)
                .ThenInclude(a => a.AppointmentType)
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId);

            if (payment == null) return NotFound();

            return View(payment);
        }

        // Admin: confirm cash payment (optional)
        public async Task<IActionResult> ConfirmCash(int paymentId)
        {
            var payment = await _context.Payment.FindAsync(paymentId);
            if (payment == null || payment.PaymentMethod != "Cash") return BadRequest();

            payment.PaymentStatus = "Paid";
            payment.PaidAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Optional: notify user
            await _notificationService.CreateNotificationAsync(
                $"Cash payment confirmed. Appointment {payment.AppointmentId} is now paid.",
                "PetOwner",
                petOwnerId: payment.OwnerId,
                appointmentId: payment.AppointmentId
            );

            return RedirectToAction("Details", new { id = paymentId });
        }

        public async Task<IActionResult> Receipt(int id)
        {
            var payment = await _context.Payment
                .Include(p => p.Appointment)
                    .ThenInclude(a => a.Pet)
                .Include(p => p.Appointment)
                    .ThenInclude(a => a.Veterinarian)
                .Include(p => p.Owner)
                .FirstOrDefaultAsync(p => p.PaymentId == id);

            if (payment == null) return NotFound();

            // For simplicity, render HTML as PDF (you can integrate a PDF library like Rotativa or iTextSharp)
            var htmlContent = $"<h2>Payment Receipt</h2>" +
                              $"<p>Payment ID: {payment.PaymentId}</p>" +
                              $"<p>Amount: {payment.Amount:C}</p>" +
                              $"<p>Status: {payment.PaymentStatus}</p>" +
                              $"<p>Transaction Ref: {payment.TransactionReference}</p>" +
                              $"<p>Pet: {payment.Appointment?.Pet?.PetName}</p>" +
                              $"<p>Vet: {(payment.Appointment?.Veterinarian != null ? payment.Appointment.Veterinarian.Name : "Not assigned")}</p>" +
                              $"<p>Date: {payment.Appointment?.Date.Add(payment.Appointment.Time):f}</p>";

            // For now, just return as raw HTML file for download
            var fileBytes = System.Text.Encoding.UTF8.GetBytes(htmlContent);
            return File(fileBytes, "text/html", $"Receipt_{payment.PaymentId}.html");
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



    }
}











