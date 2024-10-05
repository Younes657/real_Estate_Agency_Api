using System.ComponentModel.DataAnnotations;

namespace AgenceImmobiliareApi.Models.DTOs
{
    public class ForgetPasswordDTO
    {
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }
    }
}
