using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntergratedProject.Models
{
    public class Veterinarian
    {
        [Key]
        public int VeterinarianId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 8)]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).{8,}$")]
        public string Password { get; set; }

        [NotMapped]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        public string Role { get; set; }

        // Foreign key to Specialization
        public int? SpecializationId { get; set; }

        [ForeignKey("SpecializationId")]
        public Specialization Specialization { get; set; }

        // Navigation properties
        public ICollection<VetShift> Shifts { get; set; } = new List<VetShift>();
        public ICollection<VetBreak> Breaks { get; set; } = new List<VetBreak>();
        public ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();

        public ICollection<VeterinarianSpecialization> Specializations { get; set; } = new List<VeterinarianSpecialization>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
