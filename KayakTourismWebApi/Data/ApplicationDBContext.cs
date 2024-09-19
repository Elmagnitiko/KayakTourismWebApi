using KayakTourismWebApi.ModelsNS;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KayakTourismWebApi.DataNS
{
    public class ApplicationDBContext : IdentityDbContext<Customer>
    {
        public ApplicationDBContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
            
        }

        public DbSet<Event> Events { get; set; }
    }
}
