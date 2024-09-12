using KayakTourismWebApi.ModelsNS;
using Microsoft.EntityFrameworkCore;

namespace KayakTourismWebApi.DataNS
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
            
        }

        public DbSet<Event> Events { get; set; }
    }
}
