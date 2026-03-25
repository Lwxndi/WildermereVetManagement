using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntergratedProject.Models
{
    public class AppointmentType
    {
        [Key]
        public int AppointmentTypeId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;


        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        public int DurationMinutes { get; set; }

        public int SpecializationId { get; set; }
        public Specialization Specialization { get; set; }

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
