using KayakTourismWebApi.InterfacesNS;

namespace KayakTourismWebApi.DatabaseServicesNS
{
    public static class DataAccessService
    {
        public static void RegisterDataAccess(this IServiceCollection services)
        {
            services.AddScoped<IEventRepository, EventRepository>();
        }
    }
}
