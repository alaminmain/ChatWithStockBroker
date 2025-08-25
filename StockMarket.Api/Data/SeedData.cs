
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StockMarket.Api.Models;
using System.Threading.Tasks;

namespace StockMarket.Api.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

                // User 1
                if (await userManager.FindByEmailAsync("user1@example.com") == null)
                {
                    var user = new User { UserName = "user1@example.com", Email = "user1@example.com" };
                    await userManager.CreateAsync(user, "Password123!");
                }

                // User 2
                if (await userManager.FindByEmailAsync("user2@example.com") == null)
                {
                    var user = new User { UserName = "user2@example.com", Email = "user2@example.com" };
                    await userManager.CreateAsync(user, "Password123!");
                }

                // User 3
                if (await userManager.FindByEmailAsync("user3@example.com") == null)
                {
                    var user = new User { UserName = "user3@example.com", Email = "user3@example.com" };
                    await userManager.CreateAsync(user, "Password123!");
                }

                // User 4
                if (await userManager.FindByEmailAsync("user4@example.com") == null)
                {
                    var user = new User { UserName = "user4@example.com", Email = "user4@example.com" };
                    await userManager.CreateAsync(user, "Password123!");
                }
            }
        }
    }
}
