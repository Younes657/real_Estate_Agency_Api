using System.ComponentModel.DataAnnotations;

namespace AgenceImmobiliareApi.Models.DTOs
{
    public class UserContactCreateDto
    {
        [Required]
        public string Name { get; set; }
        public string Email { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Message { get; set; }
        [Required]
        public string Sujet { get; set; }
    }
}
