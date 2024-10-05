using System.ComponentModel.DataAnnotations;

namespace AgenceImmobiliareApi.Models.DTOs
{
    public class ResetPasswordDTO
    {
        [Required(ErrorMessage = "Mot de passe requis !!")]
        public string Password { get; set; }
        [Required]
        [Compare("Password", ErrorMessage = "le mot de passe et le mot de passe de confirmation ne correspondent pas !!")]
        public string ConfirmPassword { get; set; }

        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }
        [Required]
        public string Token { get; set; }
    }
}
