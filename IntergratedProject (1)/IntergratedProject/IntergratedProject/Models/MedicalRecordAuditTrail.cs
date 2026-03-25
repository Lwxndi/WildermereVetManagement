using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace IntergratedProject.Models
{
    public class MedicalRecordAuditTrail
    {
        [Key]

        public int Id { get; set; }
        public int MedicalRecordId { get; set; }
        public MedicalRecord MedicalRecord { get; set; }

        [Required] public string Action { get; set; } // Created, Updated, Signed, FileUploaded
        public int PerformedByUserId { get; set; }
        public string PerformedByRole { get; set; } // Admin, Vet
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public string OldValue { get; set; }
        public string NewValue { get; set; }

        // JSON field for complex changes
        public string ChangeData { get; set; }
    }
}
