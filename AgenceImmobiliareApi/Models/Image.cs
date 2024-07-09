using System.ComponentModel.DataAnnotations;

namespace AgenceImmobiliareApi.Models
{
    public class Image
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string ImageLink { get; set; }
        [Required]
        public int RealEstateId { get; set; } //foreign key
        public RealEstate RealEstate { get; set; } = null!;
    }
}
