using System;
using System.ComponentModel.DataAnnotations;

namespace IntergratedProject.Models
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }

        [Required]
        [MaxLength(250)]
        public string Message { get; set; }

        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        public bool IsRead { get; set; } = false;

        [Required]
        public string RecipientRole { get; set; } // "PetOwner", "Veterinarian", "Admin"

       
        public int? PetOwnerId { get; set; }
        public PetOwner PetOwner { get; set; }

        public int? VeterinarianId { get; set; }
        public Veterinarian Veterinarian { get; set; }

        public int? AdminId { get; set; } 

        public int? AppointmentId { get; set; }
        public Appointment Appointment { get; set; }

        public int? MedicalRecordId { get; set; }
        public MedicalRecord MedicalRecord { get; set; }

        //public int? InvoiceId { get; set; }
        //public Invoice Invoice { get; set; }
    }
}
