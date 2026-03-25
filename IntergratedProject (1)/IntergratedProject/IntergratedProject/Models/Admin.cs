using System.ComponentModel.DataAnnotations;

namespace IntergratedProject.Models
{
    public class Admin
    {
        [Key]
        public int AdminId { get; set; }


        [Required]
        public string FullName { get; set; }


        [Required, EmailAddress]
        public string Email { get; set; }


        [Required]
        public string Password { get; set; }
        public string Role { get; set; }

    }
}
