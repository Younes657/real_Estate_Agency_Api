using System.ComponentModel.DataAnnotations;

namespace AgenceImmobiliareApi.Models.DTOs
{
    public class CategoryUpdateDto
    {
        public int Id { get; set; }
        [Required]
        public string CategoryName { get; set; }
        
        public string? Description { get; set; }
    }
}
