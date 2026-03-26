using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PartyCenterManagement.Data;
using PartyCenterManagement.Models;

namespace PartyCenterManagement.Services
{
    public class UserProfileService
    {
        ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public UserProfileService(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
        public async Task<IdentityUser> GetIdentityUserAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
           
        }
        public async Task CreateUserProfileAsync(IdentityUser user, string first, string last)
        {
            UserProfile userPr = new UserProfile
            {
                User = user,
                FirstName = first,
                LastName = last
            };
            _db.UserProfile.Add(userPr);
            await _db.SaveChangesAsync();
        }

        public async Task<UserProfile> GetUserProfileAsync(IdentityUser user)
        {
            var userPr = _db.UserProfile.Where(x => x.User == user).FirstOrDefaultAsync();
            return await userPr;

        }

        public async Task EditUserProfileAsync(UserProfile userPr, string first, string last)
        {
            userPr.FirstName = first;
            userPr.LastName = last;
            await _db.SaveChangesAsync();
        }

        public async Task<List<UserProfile>> GetAllUserProfilesAsync()
        {
            var profiles = await _db.UserProfile.ToListAsync();
            return profiles;
        }
        public async Task<List<IdentityUser>> GetAllIdentityUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            return users;
        }
        public async Task<IList<string>> GetUserRoleAsync(IdentityUser user)
        {
            return await _userManager.GetRolesAsync(user);
        }
        
        public async Task DeleteUserAsync(string id)
        {
            var user = await GetIdentityUserAsync(id);
            var userPr = await GetUserProfileAsync(user);
            if (user != null)
            {
                _db.UserProfile.Remove(userPr);
                await _userManager.DeleteAsync(user);
                await _db.SaveChangesAsync();
            }
        }
        
        public async Task EditIdentityUserAsync(IdentityUser user, string phoneNumber, string role)
        {
            user.PhoneNumber = phoneNumber;
            await _userManager.UpdateAsync(user);

            var currentRoles = await GetUserRoleAsync(user);
            if (!currentRoles.Contains(role))
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRoleAsync(user, role);
            }
        }
    }
}
