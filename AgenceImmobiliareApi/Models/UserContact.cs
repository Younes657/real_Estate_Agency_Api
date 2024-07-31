using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.ComponentModel.DataAnnotations;

namespace AgenceImmobiliareApi.Models
{
    public class UserContact
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Email { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Message { get; set; }
        [Required]
        public string Sujet {  get; set; }
        [Required]
        public bool Seen {  get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }

    }
}
