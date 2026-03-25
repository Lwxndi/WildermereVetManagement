using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntergratedProject.Models
{
    public class Payment
    {

        [Key]
        public int PaymentId { get; set; }

        public int? AppointmentId { get; set; }
        [ForeignKey("AppointmentId")]
        public virtual Appointment Appointment { get; set; }

        [Required]
        public int OwnerId { get; set; }
        [ForeignKey("OwnerId")]
        public virtual PetOwner Owner { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]

        public decimal Amount { get; set; }
        [Required]
        public string Currency { get; set; } = "ZAR";

        [Required]
        public string PaymentMethod { get; set; }  // Card, EFT, MobileMoney, etc
        [Required]
        public string PaymentStatus { get; set; }  // Pending, Paid, Failed

        [Required]
        public string TransactionReference { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? PaidAt { get; set; }

        public string Notes { get; set; }          // EFT reference, mock info
        public string MaskedCard { get; set; }     // **** **** **** 4242
        public bool IsTest { get; set; } = true;

    }
    public enum PaymentMethods
    {
        Card,
        EFT,
        Cash
    }

    public enum PaymentStatus
    {
        Pending,
        Paid,
        Failed,
        Refunded
    }
}
