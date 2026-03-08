using Microsoft.AspNetCore.Identity;
using PartyCenterManagement.Models;
using PartyCenterManagement.Services;

namespace PartyCenterManagement.Data
{
    public class IdentitySeed
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var userProfileService = scope.ServiceProvider.GetRequiredService<UserProfileService>();

            string[] roles = { "Admin","Employee","Client" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
               
            }
            await EnsureUserWithRole(userManager,userProfileService, "admin@party.local", "Admin99*", "Admin");
        }

        private static async Task EnsureUserWithRole(
            UserManager<IdentityUser> userManager,UserProfileService userProfileService,
            string email,
            string password,
            string role)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(user, password);

                if (!result.Succeeded)
                {
                    var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                    throw new Exception($"Cannot create user {email}: {errors}");
                }
                
            }
            if (await userProfileService.GetUserAsync(user) == null)
            {
                await userProfileService.CreateUserProfileAsync(user, "Admin", "User");
            }
            
            if (!await userManager.IsInRoleAsync(user, role))
                await userManager.AddToRoleAsync(user, role);
        }
            
    }
}
