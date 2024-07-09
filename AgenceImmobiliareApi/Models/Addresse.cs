using System.ComponentModel.DataAnnotations;

namespace AgenceImmobiliareApi.Models
{
    public class Addresse
    {
        [Key]
        public int Id { get; set; }
        public string Wilaya { get; set; }
        [Required]
        public string Ville { get; set; }
        [Required]
        public string Rue { get; set; }
        public int? PostalCode { get; set; }
        public RealEstate RealEstate { get; set; } = null!;
        //don't forget to add country it is algeria so the link to google maps successed
    }
}
