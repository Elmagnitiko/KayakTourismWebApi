using System.ComponentModel.DataAnnotations;

namespace KayakTourismWebApi.DTOs.AccountNS
{
    public class RegisterDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

        [Required]
        [Phone]
        public string PhoneNumder { get; set; }
    }
}
