using Microsoft.AspNetCore.Identity;

namespace KayakData.ModelsNS
{
    public class Customer : IdentityUser
    {
        public List<EventCustomer> EventCustomers { get; set; } = new();
    }
}
