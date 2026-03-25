using System.ComponentModel.DataAnnotations;

namespace IntergratedProject.Models
{
    public class VetBreak
    {
        [Key]
        public int VetBreakId { get; set; }
        public int VeterinarianId { get; set; }
        public Veterinarian Veterinarian { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }
        [Required]
        public TimeSpan EndTime { get; set; }
    }
}
