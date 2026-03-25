    using System.ComponentModel.DataAnnotations;

    namespace IntergratedProject.Models
    {
        public class Appointment
        {
            [Key]
            public int AppointmentId { get; set; }

            [Required]
            public DateTime Date { get; set; }

            [Required]
            [DataType(DataType.Time)]
            public TimeSpan Time { get; set; }

            [MaxLength(1000)]
            public string? AdditionalNotes { get; set; }


            [Required]
            public int PetId { get; set; }
            public Pet Pet { get; set; }

            [Required]
            public int PetOwnerId { get; set; }
            public PetOwner PetOwner { get; set; }

        
            public int? VeterinarianId { get; set; }
            public Veterinarian? Veterinarian { get; set; }

            [Required]
            public int AppointmentTypeId { get; set; }
            public AppointmentType AppointmentType { get; set; }

            public string Status { get; set; } = "Approved";
            public ICollection<MedicalRecord> MedicalRecords { get; set; }


            public virtual Payment Payment { get; set; }

         }

    }
