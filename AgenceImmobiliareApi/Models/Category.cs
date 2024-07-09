using System.ComponentModel.DataAnnotations;

namespace AgenceImmobiliareApi.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string CategoryName { get; set; }
        public int NbREstate { get; set; }
        public string? Description { get; set; }
        public ICollection<RealEstate> RealEstates { get; set; } = new List<RealEstate>();
    }
}
