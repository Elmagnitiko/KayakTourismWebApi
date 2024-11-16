using System.ComponentModel.DataAnnotations;

namespace KayakData.DTOs.AccountNS
{
    public class RegisterDto
    {
        [Required]
        [EmailAddress(ErrorMessage = "Incorrect email address.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Phone]
        [RegularExpression(@"^\+?375(17|25|29|33|44)\d{7}$", ErrorMessage = "Incorrect phone number.")]
        public string PhoneNumder { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 10)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
