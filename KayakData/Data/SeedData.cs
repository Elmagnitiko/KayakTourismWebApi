using KayakData.ModelsNS;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KayakData.DataNS
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider, UserManager<Customer> userManager)
        {
            await AddModeratorRights(userManager);
        }

        private static async Task AddModeratorRights(UserManager<Customer> userManager)
        {
            const string moderatorEmail = "moderator@moderator.local";
            const string moderatorPassword = "Moderator111!";

            var moderatorExists = await userManager.Users.AnyAsync(x => x.Email == moderatorEmail);

            if (!moderatorExists)
            {
                var moderator = new Customer
                {
                    Email = moderatorEmail,
                    UserName = moderatorEmail,
                    EmailConfirmed = true,
                    TwoFactorEnabled = false,
                };

                var createModeratorResuls = await userManager.CreateAsync(moderator, moderatorPassword);

                if (createModeratorResuls.Succeeded)
                {
                    await userManager.AddToRoleAsync(moderator, Constants.ModeratorRole);
                }

            }
        }

    }
}
