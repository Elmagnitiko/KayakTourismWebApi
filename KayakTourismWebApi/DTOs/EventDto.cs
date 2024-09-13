namespace KayakTourismWebApi.DTOsNS
{
    public class EventDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime EventStarts { get; set; }
        public DateTime EventEnds { get; set; }
    }
}
