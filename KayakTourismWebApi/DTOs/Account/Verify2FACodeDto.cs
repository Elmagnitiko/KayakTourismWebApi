using System.ComponentModel.DataAnnotations;

namespace KayakTourismWebApi.DTOs.Account
{
    public class Verify2FACodeDto
    {
        [Required]
        public string? Code { get; set; }
    }
}
