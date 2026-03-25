using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntergratedProject.Models
{
    public class Specialization
    {
        [Key]
        public int SpecializationId { get; set; }
        [Required]
        public string Name { get; set; }

        public ICollection<VeterinarianSpecialization> Veterinarians { get; set; } = new List<VeterinarianSpecialization>();
        public ICollection<AppointmentType> AppointmentTypes { get; set; } = new List<AppointmentType>();
    }
}
