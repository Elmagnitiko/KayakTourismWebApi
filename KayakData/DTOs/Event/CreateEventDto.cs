using System.ComponentModel.DataAnnotations;

namespace KayakData.DTOs.EventNS
{
    public class CreateEventDto
    {
        [Required]
        [MinLength(4, ErrorMessage = "Event name must be over 4 characters long.")]
        [MaxLength(42, ErrorMessage = "Event name cannot be over 42 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MinLength(4, ErrorMessage = "Event description must be over 42 characters long.")]
        [MaxLength(333, ErrorMessage = "Event name cannot be over 333 characters.")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.1, 1000)]
        public decimal Price { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime EventStarts { get; set; }

        [Required]
        public DateTime EventEnds { get; set; }
    }
}
