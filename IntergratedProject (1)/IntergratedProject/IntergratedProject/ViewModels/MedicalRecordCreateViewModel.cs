using IntergratedProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IntergratedProject.ViewModels
{
    public class MedicalRecordCreateViewModel
    {

        [Required]
        public int PetId { get; set; }

        [Required]
        public int VeterinarianId { get; set; }

        public int? AppointmentId { get; set; }

        [Required, MaxLength(1000)]
        public string Diagnosis { get; set; }

        [Required, MaxLength(1000)]
        public string Treatment { get; set; }

        [MaxLength(2000)]
        public string Notes { get; set; }
        public MedicalRecordPriority Priority { get; set; } = MedicalRecordPriority.Normal;

        public List<IFormFile> UploadedFiles { get; set; } = new();

        public string CreatedByRole { get; set; } = "Vet";

        public List<SelectListItem> PetOptions { get; set; } = new();
        public List<SelectListItem> VeterinarianOptions { get; set; } = new();
        public List<SelectListItem> AppointmentOptions { get; set; } = new();




    }
}
