using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IntergratedProject.ViewModels
{
    public class MedicalRecordEditViewModel : MedicalRecordCreateViewModel
    {
        [Required]
        public int Id { get; set; }

        
        public List<MedicalRecordFileViewModel> ExistingFiles { get; set; } = new();

        
        public List<IFormFile> NewFiles { get; set; } = new();

        
        public string UpdatedByRole { get; set; }
    }
}
