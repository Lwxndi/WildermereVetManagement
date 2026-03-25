using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntergratedProject.Models
{
    public class PetAuditTrail
    {
        [Key]
        public int Id { get; set; }

        public int PetId { get; set; }
        [ForeignKey("PetId")]
        public Pet Pet { get; set; } = null!;

        public string Action { get; set; } = null!; // e.g., "StatusUpdated"
        public string PreviousStatus { get; set; } = null!;
        public string NewStatus { get; set; } = null!;
        public string PerformedBy { get; set; } = null!;
        public DateTime Timestamp { get; set; }

        public int? CurrentHospitalId { get; set; }
    }
}
