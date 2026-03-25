using System;

namespace IntergratedProject.ViewModels
{
    public class MedicalRecordAuditTrailViewModel
    {
        public string Action { get; set; }
        public string PerformedBy { get; set; }
        public string Role { get; set; }
        public DateTime Timestamp { get; set; }

        public string OldValue { get; set; }
        public string NewValue { get; set; }

        public string ChangeData { get; set; }
    }
}
