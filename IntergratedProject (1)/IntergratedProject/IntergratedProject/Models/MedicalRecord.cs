using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace IntergratedProject.Models
{
    public class MedicalRecord
    {
        [Key]

        public int Id { get; set; }

        // Foreign Keys
        public int PetId { get; set; }
        public int VeterinarianId { get; set; }
        public int? AppointmentId { get; set; } // optional

        // Medical Info
        [Required]
        public string Diagnosis { get; set; }
        [Required]
        public string Treatment { get; set; }
        public string Notes { get; set; }

        public MedicalRecordPriority Priority { get; set; } = MedicalRecordPriority.Normal;

        // Timestamps
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public Pet Pet { get; set; }
        public Veterinarian Veterinarian { get; set; }
        public Appointment Appointment { get; set; }
        public ICollection<MedicalRecordFile> Files { get; set; } = new List<MedicalRecordFile>();
        public ICollection<MedicalRecordAuditTrail> AuditTrails { get; set; } = new List<MedicalRecordAuditTrail>();

        public bool IsSigned { get; set; } = false;       // Has the record been signed?
        public string? SignedByRole { get; set; }         // Who signed it? Vet/Admin
        public DateTime? SignedAt { get; set; }
    }

    public enum MedicalRecordPriority
    {
        Low,
        Normal,
        High,
        Emergency
    }




}
