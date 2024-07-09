using System.ComponentModel.DataAnnotations;

namespace AgenceImmobiliareApi.Models
{
    public class BlogArticle
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string SubTitle { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        public DateTime PublicationDate { get; set; }

    }
}
