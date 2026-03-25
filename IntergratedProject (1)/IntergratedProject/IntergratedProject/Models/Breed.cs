using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntergratedProject.Models
{
    public class Breed
    {
        [Key]
        public int BreedId { get; set; }  
        public string Name { get; set; } = null!;

        public int SpeciesId { get; set; }  
        public Species Species { get; set; } = null!;
        public ICollection<Pet> Pets { get; set; } = new List<Pet>();
    }
}
