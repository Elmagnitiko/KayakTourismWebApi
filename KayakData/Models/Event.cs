using System.ComponentModel.DataAnnotations.Schema;

namespace KayakData.ModelsNS
{
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public DateTime EventStarts { get; set; }
        public DateTime EventEnds { get; set; }

        [Column(TypeName = "INTEGER")]
        public int IsRegistrationOpenedInt { get; set; } = 1;

        [NotMapped]
        public bool IsRegistrationOpened  // or better use this appeoach https://metanit.com/sharp/efcore/2.5.php
        {
            get => IsRegistrationOpenedInt == 1;
            set => IsRegistrationOpenedInt = value ? 1 : 0;
        }
        public List<EventCustomer> EventCustomers { get; set; } = new();
    }
}
