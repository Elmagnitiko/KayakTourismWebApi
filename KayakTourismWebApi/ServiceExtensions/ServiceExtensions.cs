using KayakTourismWebApi.DataNS;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace KayakTourismWebApi.ServiceExtensionsNS
{
    public static class ServiceExtensions
    {
        public static void ConfigureEntityFramework(this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionStringBuilder = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ApplicationDBContext>(options =>
            {
                options.UseSqlite(connectionStringBuilder);
            });

            //logger.LogInformation("Configured EF.");
        }

    }
}
