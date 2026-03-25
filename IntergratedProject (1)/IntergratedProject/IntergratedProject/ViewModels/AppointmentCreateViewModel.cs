using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntergratedProject.ViewModels
{
    public class AppointmentCreateViewModel
    {
        public int? AppointmentId { get; set; }

        [Required]
        [Display(Name = "Appointment Type")]
        public int AppointmentTypeId { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Display(Name = "Veterinarian")]
        public int? VeterinarianId { get; set; }

        [Required]
        [Display(Name = "Pet")]
        public int ?PetId { get; set; }

        public string? PetName { get; set; }

        [Required]
        [Display(Name = "Appointment Date & Time")]
        public DateTime AppointmentDateTime { get; set; }

        [Display(Name = "Additional Notes")]
        public string? AdditionalNotes { get; set; }



        
        public int DurationMinutes { get; set; }

        
        public DateTime EndTime => AppointmentDateTime.AddMinutes(DurationMinutes);

        
        public IEnumerable<SelectListItem> AppointmentTypes { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Veterinarians { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Pets { get; set; } = new List<SelectListItem>();

         public string? SystemMessage { get; set; }
    }
}
