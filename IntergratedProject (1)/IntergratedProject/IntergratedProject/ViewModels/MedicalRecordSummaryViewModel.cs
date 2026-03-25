namespace IntergratedProject.ViewModels
{
    public class MedicalRecordSummaryViewModel
    {
        public int Id { get; set; }
        public string PetName { get; set; }
        public string OwnerName { get; set; }
        public string DiagnosisSummary { get; set; } // truncated for dashboard
        public string Priority { get; set; }
        public bool IsSigned { get; set; } // for vet to quickly see signed status
        public DateTime CreatedAt { get; set; }
        public string Diagnosis { get; set; }

    }
}
