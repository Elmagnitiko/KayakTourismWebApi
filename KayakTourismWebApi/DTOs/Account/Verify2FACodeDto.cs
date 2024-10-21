using System.ComponentModel.DataAnnotations;

namespace KayakTourismWebApi.DTOs.Account
{
    public class Verify2FACodeDto
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Code { get; set; }
    }
}
