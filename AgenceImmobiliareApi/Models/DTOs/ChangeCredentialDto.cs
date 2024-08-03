namespace AgenceImmobiliareApi.Models.DTOs
{
    public class ChangeCredentialDto
    {
        public string NewEmail { get; set; }
        public string NewPassword { get; set; }
        public string OldPassword { get; set; }
        public string UserName { get; set; }

    }
}
