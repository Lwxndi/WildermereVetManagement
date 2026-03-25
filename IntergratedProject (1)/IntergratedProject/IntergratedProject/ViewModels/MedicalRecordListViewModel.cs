using System;
using  IntergratedProject.Models;

namespace IntergratedProject.ViewModels
{
    public class MedicalRecordListViewModel
    {
        public int Id { get; set; }
        public string PetName { get; set; }
        public string VeterinarianName { get; set; }
        public string DiagnosisSummary { get; set; }
        public DateTime CreatedAt { get; set; }
        public MedicalRecordPriority Priority { get; set; }

        public string ViewerRole { get; set; }
        public bool CanViewDetails => ViewerRole == "Vet" || ViewerRole == "Admin" || ViewerRole == "PetOwner";


        // Signing info
        public bool IsSigned { get; set; }
        public string? SignedByRole { get; set; }
    }
}
