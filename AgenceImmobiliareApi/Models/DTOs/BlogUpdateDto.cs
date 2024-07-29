using System.ComponentModel.DataAnnotations;

namespace AgenceImmobiliareApi.Models.DTOs
{
    public class BlogUpdateDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Author { get; set; }
    }
}
