using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PartyCenterManagement.Models;
using PartyCenterManagement.Models.ViewModels;
using PartyCenterManagement.Services;
using System.Data;

namespace PartyCenterManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly PackageServices _packageServices;
        private readonly ReservationServices _reservationServices;
        private readonly UserProfileService _userProfileService;

        public AdminController(PackageServices packageServices, ReservationServices reservationServices,UserProfileService userProfileService)
        {
            _packageServices = packageServices;
            _reservationServices = reservationServices;
            _userProfileService = userProfileService;

        }
        public async Task<IActionResult> Dashboard(DateTime? startDate, DateTime? endDate)
        {
            var stats = await _reservationServices.GetDashboardStats(startDate, endDate);

            return View(stats);
        }

        public async Task<IActionResult> PackagesAndServices()
        {
            var packages = await _packageServices.GetPackages();
            var services = await _packageServices.GetServices();

            ViewBag.Packages = packages;
            ViewBag.Services = services;

            return View();
        }




        [HttpPost]
        public async Task<IActionResult> DeletePackage(int id)
        {
            await _packageServices.DeletePackage(id);
            return RedirectToAction("PackagesAndServices");
        }

       
        public async Task<IActionResult> DeleteService(int id)
        {
            await _packageServices.DeleteService(id);
            return RedirectToAction("PackagesAndServices");
        }

        [HttpGet]
        public async Task<IActionResult> UpsertPackage(int? id)
        {
            var services = await _packageServices.GetServices();

            if (id == null || id == 0)
            {
                return View(new UpsertPackageViewModel
                {
                    AllServices = services
                });
            }

            var packages = await _packageServices.GetPackages();
            var package = packages.FirstOrDefault(p => p.PackageID == id);

            if (package == null)
                return NotFound();

            var vm = new UpsertPackageViewModel
            {
                PackageID = package.PackageID,
                Name = package.Name,
                Price = package.Price,
                MaxGuests = package.MaxGuests,
                MaxLength = package.MaxLength,
                AllServices = services,
                SelectedServiceIds = package.PackageServices
                    .Select(ps => ps.ServiceID)
                    .ToList()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpsertPackage(UpsertPackageViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AllServices = await _packageServices.GetServices();
                return View(model);
            }

            if (model.PackageID == 0)
            {
                await _packageServices.CreatePackage(
                    model.Name,
                    model.Price,
                    model.MaxGuests,
                    model.MaxLength
                );

                var createdPackages = await _packageServices.GetPackages();
                var created = createdPackages.Last(); 

                foreach (var serviceId in model.SelectedServiceIds)
                {
                    await _packageServices.AddServiceToPackage(created.PackageID, serviceId);
                }
            }
            else
            {
                await _packageServices.UpdatePackage(
                    model.PackageID,
                    model.Name,
                    model.Price,
                    model.MaxGuests,
                    model.MaxLength
                );

                var packages = await _packageServices.GetPackages();
                var package = packages.First(p => p.PackageID == model.PackageID);

                var currentServiceIds = package.PackageServices.Select(ps => ps.ServiceID).ToList();

                foreach (var serviceId in currentServiceIds)
                {
                    if (!model.SelectedServiceIds.Contains(serviceId))
                    {
                        await _packageServices.RemoveServiceFromPackage(model.PackageID, serviceId);
                    }
                }

                foreach (var serviceId in model.SelectedServiceIds)
                {
                    if (!currentServiceIds.Contains(serviceId))
                    {
                        await _packageServices.AddServiceToPackage(model.PackageID, serviceId);
                    }
                }
            }

            return RedirectToAction("PackagesAndServices");
        }
        [HttpGet]
        public async Task<IActionResult> UpsertService(int? id)
        {
            if (id == null || id == 0)
            {
                return View(new UpsertServiceViewModel());
            }

            var services = await _packageServices.GetServices();
            var service = services.FirstOrDefault(s => s.ServiceID == id);

            if (service == null)
                return NotFound();

            var vm = new UpsertServiceViewModel
            {
                ServiceID = service.ServiceID,
                Serv = service.Serv,
                Price = service.Price
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpsertService(UpsertServiceViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.ServiceID == 0)
            {
                await _packageServices.CreateService(model.Serv, model.Price);
            }
            else
            {
                await _packageServices.UpdateService(
                    model.ServiceID,
                    model.Serv,
                    model.Price
                );
            }

            return RedirectToAction("PackagesAndServices");
        }

        public async Task<IActionResult> Users()
        {
            var users = await _userProfileService.GetAllIdentityUsersAsync();
            var profiles = await _userProfileService.GetAllUserProfilesAsync();

            var list = new List<UserViewModel>();

            foreach (var u in users)
            {
                var profile = profiles.FirstOrDefault(p => p.UserID == u.Id);
                var role = await _userProfileService.GetUserRoleAsync(u);

                list.Add(new UserViewModel
                {
                    UserID = u.Id,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    FirstName = profile?.FirstName,
                    LastName = profile?.LastName,
                    Role = role.FirstOrDefault()
                });
            }

            return View(list);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            await _userProfileService.DeleteUserAsync(id);

            return RedirectToAction("Users");
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userProfileService.GetIdentityUserAsync(id);
            if (user == null) return NotFound();

            var profile = await _userProfileService.GetUserProfileAsync(user);
            var role = await _userProfileService.GetUserRoleAsync(user);

            var vm = new UserViewModel
            {
                UserID = user.Id,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FirstName = profile?.FirstName,
                LastName = profile?.LastName,
                Role = role.FirstOrDefault() 
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(UserViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userProfileService.GetIdentityUserAsync(model.UserID);
            if (user == null) return NotFound();

            await _userProfileService.EditIdentityUserAsync(user, model.PhoneNumber, model.Role);

            var profile = await _userProfileService.GetUserProfileAsync(user);
            if (profile != null)
                await _userProfileService.EditUserProfileAsync(profile, model.FirstName, model.LastName);

            return RedirectToAction("Users");
        }
    }
}
