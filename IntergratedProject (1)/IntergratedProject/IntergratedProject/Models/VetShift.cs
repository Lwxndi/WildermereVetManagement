using System.ComponentModel.DataAnnotations;

namespace IntergratedProject.Models
{
    public class VetShift
    {
        [Key]
        public int VetShiftId { get; set; }
        public int VeterinarianId { get; set; }
        public Veterinarian Veterinarian { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        [Required]
        public TimeSpan StartTime { get; set; }
        [Required]
        public TimeSpan EndTime { get; set; }
        public ICollection<VetBreak> Breaks { get; set; } = new List<VetBreak>();
    }
}
