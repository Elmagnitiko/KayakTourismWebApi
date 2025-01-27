using System.ComponentModel.DataAnnotations;

namespace KayakData.DTOs.AccountNS
{
    public class Verify2FACodeDto
    {
        [Required]
        public string? Code { get; set; }
    }
}
