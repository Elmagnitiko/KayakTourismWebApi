using System.ComponentModel.DataAnnotations;

namespace KayakData.DTOs.AccountNS
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
