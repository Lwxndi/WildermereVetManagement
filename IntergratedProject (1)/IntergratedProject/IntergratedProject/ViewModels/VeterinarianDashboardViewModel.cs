using IntergratedProject.Models;

namespace IntergratedProject.ViewModels
{
    public class VeterinarianDashboardViewModel
    {
        public List<Appointment> UpcomingAppointments { get; set; } = new();
        public List<Appointment> CompletedAppointments { get; set; } = new();
        public List<MedicalRecordSummaryViewModel> RecentMedicalRecords { get; set; } = new();

        // For pet search
        public string SearchTerm { get; set; }
        public List<Pet> SearchResults { get; set; } = new();

        // Notifications
        public int UnreadNotificationsCount { get; set; } = 0;
        public List<Notification> Notifications { get; set; } = new();
    }
}
