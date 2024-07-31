using System.ComponentModel.DataAnnotations;

namespace AgenceImmobiliareApi.Models
{
    public class BlogArticle
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        public DateTime PublicationDate { get; set; } //add the modification date
        public DateTime? UpdatedDate { get; set; }
        [Required]
        public int NumberBlog { get; set; }

    }
}
