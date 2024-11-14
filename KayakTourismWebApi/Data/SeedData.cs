using KayakTourismWebApi.ModelsNS;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KayakTourismWebApi.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider, UserManager<Customer> userManager)
        {
            //var userManager = services.GetRequiredService<UserManager<Customer>>();
            await AddModeratorRights(userManager);
        }

        private static async Task AddModeratorRights(UserManager<Customer> userManager)
        {
            var moderatorExists = await userManager.Users.AnyAsync(x => x.Email == "moderator@moderator.local");

            const string email = "moderator@moderator.local";
            const string password = "Moderator111!";

            if (!moderatorExists)
            {
                var moderator = new Customer
                {
                    Email = email,
                    UserName = email,
                    EmailConfirmed = true,
                    TwoFactorEnabled = false,
                };

                var createModeratorResuls = await userManager.CreateAsync(moderator, password);

                if (createModeratorResuls.Succeeded)
                {
                    await userManager.AddToRoleAsync(moderator, Constants.ModeratorRole);
                }

            }
        }

    }
}
