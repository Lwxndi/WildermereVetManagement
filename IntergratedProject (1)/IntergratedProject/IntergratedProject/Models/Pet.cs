using System.ComponentModel.DataAnnotations;

namespace IntergratedProject.Models
{
    public class Pet
    {
        [Key]
        public int PetId { get; set; }
        public string? ImageFileName { get; set; }
        public string PetName { get; set; }
        public Species Species { get; set; } = null!;
        public int SpeciesId { get; set; }
        public int? BreedId { get; set; }
        public Breed? Breed { get; set; }
        [Range(0, 100)]
        public int? AgeYears { get; set; }
        [Required]
        [Display(Name = "Gender")]
        public string Gender { get; set; }
        public double? WeightKg { get; set; }
        public double? HeightCm { get; set; }
        public string? ColorMarkings { get; set; }
        public int? PetOwnerId { get; set; }
        public PetOwner? PetOwner { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
        public int? PrimaryVeterinarianId { get; set; } 
        public Veterinarian? PrimaryVeterinarian { get; set; }
        public ICollection<MedicalRecord> MedicalRecords { get; set; }

        public PetStatus Status { get; set; } = PetStatus.Active;
        public int? CurrentHospitalId { get; set; }
    }

    public enum PetStatus
    {
        Active,
        Deceased,
        Retired,
        Transferred
    }


}
