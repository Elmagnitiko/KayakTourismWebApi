using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;

namespace KayakTourismWebApi.DTOs.AccountNS
{
    public class LoginWith2FACodeDto
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "Authenticator code")]
        public string TwoFactorCode { get; set; }

        [Required]
        public bool RememberMe { get; set; }

        [Required]
        [Display(Name = "Remember this machine")]
        public bool RememberMachine { get; set; }
    }
}
