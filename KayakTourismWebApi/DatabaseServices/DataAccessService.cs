using KayakTourismWebApi.DataNS;
using KayakTourismWebApi.InterfacesNS;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace KayakTourismWebApi.DatabaseServices
{
    public static class DataAccessService
    {
        public static void ConfigureEntityFramework(this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionStringBuilder = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ApplicationDBContext>(options =>
            {
                options.UseSqlite(connectionStringBuilder);
            });
        }

        public static void RegisterDataAccess(this IServiceCollection services)
        {
            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<IEventSubscription, EventSubscriptionRepository>();
        }

    }
}
