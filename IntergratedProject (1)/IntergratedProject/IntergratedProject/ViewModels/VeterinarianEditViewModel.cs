using System.ComponentModel.DataAnnotations;

namespace IntergratedProject.ViewModels
{
    public class VeterinarianEditViewModel
    {
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
    }
}
