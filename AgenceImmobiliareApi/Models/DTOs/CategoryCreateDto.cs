using System.ComponentModel.DataAnnotations;

namespace AgenceImmobiliareApi.Models.DTOs
{
    public class CategoryCreateDto
    {
        
        [Required]
        public string CategoryName { get; set; }
        public string? Description { get; set; }
       
    }
}
