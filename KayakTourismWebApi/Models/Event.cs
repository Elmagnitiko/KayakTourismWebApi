using System.ComponentModel.DataAnnotations.Schema;

namespace KayakTourismWebApi.ModelsNS
{
    //[Table("Events")]
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public DateTime EventStarts { get; set; }
        public DateTime EventEnds { get; set; }
        public string IsRegistrationOpened { get; set; } = "true";
    }
}
