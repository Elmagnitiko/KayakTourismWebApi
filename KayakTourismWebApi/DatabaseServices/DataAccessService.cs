using KayakTourismWebApi.DataNS;
using KayakTourismWebApi.InterfacesNS;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace KayakTourismWebApi.DatabaseServicesNS
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

            //logger.LogInformation("Configured EF.");
        }

        public static void RegisterDataAccess(this IServiceCollection services)
        {
            services.AddScoped<IEventRepository, EventRepository>();
        }

        public static void ConfigureJsonOptions(this IServiceCollection services) 
        {
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            }); 
        }
    }
}
