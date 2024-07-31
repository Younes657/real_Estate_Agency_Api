using System.ComponentModel.DataAnnotations;

namespace AgenceImmobiliareApi.Models.DTOs
{
    public class BlogCreateDto
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        public int NumberBlog { get; set; }
    }
}
