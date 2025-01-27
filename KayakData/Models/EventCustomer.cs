
namespace KayakData.ModelsNS
{
    public class EventCustomer
    {
        public int EventId { get; set; }
        public Event Event { get; set; }

        public string CustomerId { get; set; }
        public Customer Customer { get; set; }
    }
}
