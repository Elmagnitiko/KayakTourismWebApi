using KayakData.DataNS;
using KayakData.ModelsNS;
using Microsoft.AspNetCore.Identity;

namespace KayakTourismWebApi.ServiceExtensionsNS
{
    public static class ApplicationBuilderExtensions 
    { 
        public static async Task SeedDataAsync(this IApplicationBuilder app) 
        { 
            using (var scope = app.ApplicationServices.CreateScope()) 
            { 
                var services = scope.ServiceProvider; 
                var userManager = services.GetRequiredService<UserManager<Customer>>(); 
                await SeedData.InitializeAsync(services, userManager); 
            } 
        } 
    }
}
