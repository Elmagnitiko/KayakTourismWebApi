using System.ComponentModel.DataAnnotations;

namespace KayakTourismWebApi.DTOs.AccountNS
{
    public class VerifyCodeDto
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Code { get; set; }
    }
}
