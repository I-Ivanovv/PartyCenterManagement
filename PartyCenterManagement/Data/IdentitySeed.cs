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
            await EnsureUserWithRole(userManager, userProfileService, "employee@party.local", "Employee99*", "Employee");
            await EnsureUserWithRole(userManager, userProfileService, "user@party.local", "User99*", "Client");
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
            if (await userProfileService.GetUserProfileAsync(user) == null)
            {
                if (role == "Admin")
                {
                    await userProfileService.CreateUserProfileAsync(user, "Admin", "User");
                }
                else if (role == "Employee")
                {
                    await userProfileService.CreateUserProfileAsync(user, "Employee", "User");
                }
                else if (role == "Client")
                {
                    await userProfileService.CreateUserProfileAsync(user, "Client", "User");
                }
            }
            
            if (!await userManager.IsInRoleAsync(user, role))
                await userManager.AddToRoleAsync(user, role);
        }
            
    }
}
