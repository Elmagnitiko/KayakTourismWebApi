using System.ComponentModel.DataAnnotations;

namespace KayakTourismWebApi.DTOs.Account
{
    public class VerifyCodeDto
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Code { get; set; }
    }
}
