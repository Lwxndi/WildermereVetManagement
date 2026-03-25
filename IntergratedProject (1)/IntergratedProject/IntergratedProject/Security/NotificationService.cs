using IntergratedProject.Data;
using IntergratedProject.Models;

namespace IntergratedProject.Security
{
    public class NotificationService
    {
            private readonly IntergratedProjectDatabase _context;

            public NotificationService(IntergratedProjectDatabase context)
            {
                _context = context;
            }

            public async Task CreateNotificationAsync(string message, string role, int? petOwnerId = null, int? vetId = null, int? adminId = null, int? appointmentId = null, int? medicalRecordId = null, int? invoiceId = null)
            {
                var notification = new Notification
                {
                    Message = message,
                    RecipientRole = role,
                    PetOwnerId = petOwnerId,
                    VeterinarianId = vetId,
                    AdminId = adminId,
                    AppointmentId = appointmentId,
                    MedicalRecordId = medicalRecordId,
                    //InvoiceId = invoiceId,
                    SentAt = DateTime.UtcNow,
                    IsRead = false
                };

                _context.Notification.Add(notification);
                await _context.SaveChangesAsync();
            }
        }
    }


