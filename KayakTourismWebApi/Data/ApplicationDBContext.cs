using KayakTourismWebApi.ModelsNS;
using Microsoft.AspNetCore.Identity;
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var roles = new[]
            {
                new IdentityRole
                {
                    Name = Constants.ModeratorRole,
                    NormalizedName = Constants.NormalizedModeratorRole,
                },
                new IdentityRole
                {
                    Name = Constants.CustomerRole,
                    NormalizedName = Constants.NormalizedCustomerRole,
                }
            };

            modelBuilder.Entity<IdentityRole>()
                .HasData(roles);
        }
    }
}
