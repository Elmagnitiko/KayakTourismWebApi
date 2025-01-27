namespace KayakData.DTOs.EventNS
{
    public class EventDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime EventStarts { get; set; }
        public DateTime EventEnds { get; set; }
    }
}
