using System.ComponentModel.DataAnnotations;

namespace IntergratedProject.Models
{
    public class PetOwnerEditViewModel
    {
        public int OwnerId { get; set; }

        [Required]
        public string Name { get; set; }

        public string? MiddleName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
    }
}
