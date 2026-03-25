namespace IntergratedProject.Models
{
    public class VeterinarianSpecialization
    {
        public int VeterinarianId { get; set; }
        public Veterinarian Veterinarian { get; set; }

        public int SpecializationId { get; set; }
        public Specialization Specialization { get; set; }
    }
}
