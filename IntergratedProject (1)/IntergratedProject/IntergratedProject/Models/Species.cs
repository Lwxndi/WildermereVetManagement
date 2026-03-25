using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntergratedProject.Models
{
    public class Species
    {
        [Key]
        public int SpeciesId { get; set; }  // PK
        public string Name { get; set; } = null!;

        // Navigation
        public ICollection<Breed> Breeds { get; set; } = new List<Breed>();

    }
}
