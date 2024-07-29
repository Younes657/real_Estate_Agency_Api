using System.ComponentModel.DataAnnotations;

namespace AgenceImmobiliareApi.Models
{
    public class RealEstate
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string RName { get; set; } 
        [Required]
        public decimal Price { get; set; }
        [Required]
        public decimal Surface { get; set; }
        [Required]
        public int CategoryId { get; set; }//foreign key
        public Category Category { get; set; } = null!;
        [Required]
        public string OffreType { get; set; }
        [Required]
        public int AddressId { get; set; } //foreign key
        public Addresse Addresse { get; set; } = null!;
        public int Floor { get; set; }
        public int? BathRoom {  get; set; }
        public string? Description { get; set; }
        public int Room { get; set; }
        public int Garage { get; set; }
        public int NbImage { get; set; }
        [Required]
        public DateTime PostingDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public ICollection<Image> Images { get; set; } = new List<Image>();

    }
}
