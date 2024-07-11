using System.ComponentModel.DataAnnotations;

namespace AgenceImmobiliareApi.Models.DTOs
{
    public record RechercheCritiria
    {
        [Required]
        public string OffreType { get; set; }
        public string? Category { get; set; }
        public string? Wilaya { get; set; }
        public string? Ville { get; set; }
        public string? Rue { get; set; }
        public int Rooms { get; set; }
        public int floor { get; set; }
        public decimal surfaceMax { get; set; }
        public decimal surfaceMin { get; set; }
        public decimal PrixMax { get; set; }
        public decimal PrixMin { get; set; }
    }
}
