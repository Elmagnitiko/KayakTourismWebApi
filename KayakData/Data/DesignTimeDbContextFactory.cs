using global::KayakData.DataNS;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
namespace KayakData 
{ 
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDBContext> 
    { 
        public ApplicationDBContext CreateDbContext(string[] args) 
        { 
            var configuration = new ConfigurationBuilder().SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../KayakTourismWebApi")).AddJsonFile("appsettings.json").Build(); 
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDBContext>(); 
            var connectionString = configuration.GetConnectionString("DefaultConnection"); 
            optionsBuilder.UseSqlite(connectionString); 
            return new ApplicationDBContext(optionsBuilder.Options); 
        } 
    } 
}

