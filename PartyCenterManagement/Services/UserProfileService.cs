using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PartyCenterManagement.Data;
using PartyCenterManagement.Models;
using System.Security.Claims;

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

        public async Task<UserProfile> GetUserAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var userPr = _db.UserProfile.Where(x => x.User == user).FirstOrDefaultAsync();
            return await userPr;

        }
    }
}
