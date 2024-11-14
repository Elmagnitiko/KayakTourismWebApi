using System.ComponentModel.DataAnnotations;

namespace KayakTourismWebApi.DTOs.ManageAccountNS
{
    public class ChangeEmailDto
    {
        [Required]
        [EmailAddress(ErrorMessage = "Incorrect email address.")]
        [Display(Name = "Email")]
        public string? Email { get; set; }
    }
}
