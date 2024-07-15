using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AgenceImmobiliareApi.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiry { get; set; }
    }
}
