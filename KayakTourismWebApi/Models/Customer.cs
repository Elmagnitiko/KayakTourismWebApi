using Microsoft.AspNetCore.Identity;

namespace KayakTourismWebApi.ModelsNS
{
    public class Customer : IdentityUser
    {
        public List<EventCustomer> EventCustomers { get; set; } = new();
    }
}
