using System.ComponentModel.DataAnnotations;

namespace KayakTourismWebApi.DTOs.Account
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
