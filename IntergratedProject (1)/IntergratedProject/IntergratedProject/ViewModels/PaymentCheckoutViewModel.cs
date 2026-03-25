using IntergratedProject.Models;
using System.ComponentModel.DataAnnotations;

namespace IntergratedProject.ViewModels
{
    public class PaymentCheckoutViewModel
    {
        [Required]
        public int AppointmentId { get; set; }

        [Required]
        public string PetName { get; set; }


        [Required]
        public string AppointmentType { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public TimeSpan Time { get; set; }

        [Required]
        [Range(1, 100000, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Please select a payment method")]
        public PaymentMethods SelectedMethod { get; set; }
    }
}
