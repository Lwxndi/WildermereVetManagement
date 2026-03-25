using IntergratedProject.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace IntergratedProject.ViewModels
{
    public class PetStatusViewModel
    {
        public int PetId { get; set; }
        public string PetName { get; set; }
        public PetStatus CurrentStatus { get; set; }
        public PetStatus NewStatus { get; set; }
        public int? NewHospitalId { get; set; } // optional for transfer
        public List<SelectListItem> StatusOptions { get; set; }
    }
}
