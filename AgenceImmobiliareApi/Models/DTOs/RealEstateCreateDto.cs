using System.ComponentModel.DataAnnotations;

namespace AgenceImmobiliareApi.Models.DTOs
{
    public class RealEstateCreateDto
    {
        [Required]
        public string RName { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public decimal Surface { get; set; }
        [Required]
        public int CategoryId { get; set; }//foreign key
        [Required]
        public string OffreType { get; set; }
        [Required]
        public int AddressId { get; set; } //foreign key
        public int Floor { get; set; }
        public int? BathRoom { get; set; }
        public string? Description { get; set; }
        public int Room { get; set; }
        public int Garage { get; set; }
        public bool IsActive { get; set; }
        public string? Wilaya { get; set; }
        [Required]
        public string Ville { get; set; }
        [Required]
        public string Rue { get; set; }
        public int PostalCode { get; set; }
        public List<IFormFile>? ImagesFiles { get; set; } = new List<IFormFile>();
        //postingdate updadateddate nbImages
    }
}
