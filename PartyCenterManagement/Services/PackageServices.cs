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

        public async Task UpdatePackage(int id, string name, double price, int guests, int length)
        {
            var package = await _db.Packages.FindAsync(id);

            if (package == null) return;

            package.Name = name;
            package.Price = price;
            package.MaxGuests = guests;
            package.MaxLength = length;

            await _db.SaveChangesAsync();
        }

        public async Task UpdateService(int id, string name, double price)
        {
            var service = await _db.Services.FindAsync(id);
            if (service == null) return;

            service.Serv = name;
            service.Price = price;

            await _db.SaveChangesAsync();
        }

        public async Task CreatePackage(string name, double price, int maxGuests, int maxLength)
        {
            var package = new Package
            {
                Name = name,
                Price = price,
                MaxGuests = maxGuests,
                MaxLength = maxLength
            };

            _db.Packages.Add(package);
            await _db.SaveChangesAsync();
        }

        public async Task DeletePackage(int id)
        {
            var package = await _db.Packages.FindAsync(id);

            _db.Packages.Remove(package);
            await _db.SaveChangesAsync();
        }

        public async Task CreateService(string name, double price)
        {
            var service = new Service
            {
                Serv = name,
                Price = price
            };

            _db.Services.Add(service);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteService(int id)
        {
            var service = await _db.Services.FindAsync(id);

            _db.Services.Remove(service);
            await _db.SaveChangesAsync();
        }

        public async Task AddServiceToPackage(int packageId, int serviceId)
        {
            bool exists = await _db.PackageServices
                .AnyAsync(ps => ps.PackageID == packageId && ps.ServiceID == serviceId);

            if (!exists)
            {
                _db.PackageServices.Add(new PackageService
                {
                    PackageID = packageId,
                    ServiceID = serviceId
                });

                await _db.SaveChangesAsync();
            }
        }

        public async Task RemoveServiceFromPackage(int packageId, int serviceId)
        {
            var ps = await _db.PackageServices
                .FirstOrDefaultAsync(x => x.PackageID == packageId && x.ServiceID == serviceId);

            if (ps != null)
            {
                _db.PackageServices.Remove(ps);
                await _db.SaveChangesAsync();
            }
        }
    }
}
