using System.ComponentModel.DataAnnotations;

namespace AgenceImmobiliareApi.Models.DTOs
{
    public class LoginRequestDto
    {
        [Required]
        public string UserNameEmail { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
