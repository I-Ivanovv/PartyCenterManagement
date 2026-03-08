using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PartyCenterManagement.Data;
using PartyCenterManagement.Models;

namespace PartyCenterManagement.Services
{
    public class PackageServices
    {
        ApplicationDbContext _db;

        public PackageServices(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<Package>> GetPackages()
        {
             var packages = _db.Packages
            .Include(p => p.PackageServices)
                .ThenInclude(ps => ps.Service)
            .ToListAsync();
            return await packages;
        }

        public async Task<List<Service>> GetServices()
        {
            var services = _db.Services.OrderBy(s => s.Price).ToListAsync();
            return await services;
        }
    }
}
