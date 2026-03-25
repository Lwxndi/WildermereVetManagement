//using Microsoft.AspNetCore.Mvc.Rendering;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace IntergratedProject.ViewModels
//{
//    public class PetCreateViewModel
//    {
//        public int? PetId { get; set; }

//        [Required]
//        public string Name { get; set; } = null!;

//        [Required]
//        public int SpeciesId { get; set; }

//        public int? BreedId { get; set; } 

//        public int? AgeYears { get; set; }
//        [Required]
//        [Display(Name = "Gender")]
//        public string Gender { get; set; }


//        public double? WeightKg { get; set; }
//        public double? HeightCm { get; set; }
//        public string? ColorMarkings { get; set; }
//        [NotMapped]
//        [Display(Name = "Upload Pet Image")]
//        public IFormFile? ImageFile { get; set; }

//        [Display(Name = "Primary Veterinarian")]
//        public int? PrimaryVeterinarianId { get; set; }


//        public IEnumerable<SelectListItem> Veterinarians { get; set; } = new List<SelectListItem>();
//        public IEnumerable<SelectListItem> SpeciesList { get; set; } = new List<SelectListItem>();
//        public IEnumerable<SelectListItem> BreedList { get; set; } = new List<SelectListItem>();
//    }
//}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IntergratedProject.ViewModels
{
    public class PetCreateViewModel
    {
        public int? PetId { get; set; }

        [Required]
        public string PetName { get; set; } = null!;

        [Required]
        [Display(Name = "Species")]
        public int SpeciesId { get; set; }

        [Display(Name = "Breed")]
        public int? BreedId { get; set; }

        [Range(0, 100)]
        [Display(Name = "Age (Years)")]
        public int? AgeYears { get; set; }

        [Required]
        [Display(Name = "Gender")]
        public string Gender { get; set; } = null!;

        [Display(Name = "Weight (Kg)")]
        public double? WeightKg { get; set; }

        [Display(Name = "Height (Cm)")]
        public double? HeightCm { get; set; }

        [Display(Name = "Color/Markings")]
        public string? ColorMarkings { get; set; }

        [Display(Name = "Upload Pet Image")]
        public IFormFile? ImageFile { get; set; }

        public string? CroppedImageData { get; set; }

        // NEW: Primary Veterinarian
        [Display(Name = "Primary Veterinarian")]
        public int? PrimaryVeterinarianId { get; set; }

        // Dropdown lists
        public IEnumerable<SelectListItem> SpeciesList { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> BreedList { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Veterinarians { get; set; } = new List<SelectListItem>();
    }
}
