namespace IntergratedProject.ViewModels
{
    public class PaymentConfirmationViewModel
    {
        public int PaymentId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }
        public string TransactionReference { get; set; }
        public DateTime? PaidAt { get; set; }
        public string Notes { get; set; }

        // Appointment info
        public int? AppointmentId { get; set; }
        public string PetName { get; set; }
        public string VetName { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public string AppointmentType { get; set; }
    }
}
