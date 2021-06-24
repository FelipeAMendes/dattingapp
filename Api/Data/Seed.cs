using Api.Entities;
using Api.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Api.Data
{
    public class Seed
    {
        private const string password = "password@123";

        public static async Task SeedUsers(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            if (await userManager.Users.AnyAsync()) return;

            var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");

            var users = JsonSerializer.Deserialize<IEnumerable<AppUser>>(userData);
            if (users is null) return;

            var roles = new List<AppRole>
            {
                new AppRole { Name = RoleNames.Admin.ToString() },
                new AppRole { Name = RoleNames.Moderator.ToString() },
                new AppRole { Name = RoleNames.Member.ToString() }
            };

            foreach (var role in roles)
            {
                await roleManager.CreateAsync(role);
            }

            foreach (var user in users)
            {
                user.UserName = user.UserName.ToLower();
                await userManager.CreateAsync(user, password);
                await userManager.AddToRoleAsync(user, RoleNames.Member.ToString());
            }

            var admin = new AppUser { UserName = "admin" };
            await userManager.CreateAsync(admin, password);
            await userManager.AddToRolesAsync(admin, new List<string> { RoleNames.Admin.ToString(), RoleNames.Moderator.ToString() });
        }
    }
}
