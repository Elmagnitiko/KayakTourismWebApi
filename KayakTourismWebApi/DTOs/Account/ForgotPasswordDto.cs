using System.ComponentModel.DataAnnotations;

namespace KayakTourismWebApi.DTOs.AccountNS
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
