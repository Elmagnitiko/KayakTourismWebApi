using KayakData.ModelsNS;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KayakData.DataNS
{
    public class ApplicationDBContext : IdentityDbContext<Customer>
    {
        public ApplicationDBContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
            
        }

        public DbSet<Event> Events { get; set; }
        public DbSet<EventCustomer> EventCustomers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<EventCustomer>()
            .HasKey(ec => new { ec.EventId, ec.CustomerId });

            modelBuilder.Entity<EventCustomer>()
                .HasOne(ec => ec.Event)
                .WithMany(e => e.EventCustomers)
                .HasForeignKey(ec => ec.EventId);

            modelBuilder.Entity<EventCustomer>()
                .HasOne(ec => ec.Customer)
                .WithMany(c => c.EventCustomers)
                .HasForeignKey(ec => ec.CustomerId);

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
