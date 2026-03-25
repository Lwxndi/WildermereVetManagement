using System.ComponentModel.DataAnnotations;

namespace IntergratedProject.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
