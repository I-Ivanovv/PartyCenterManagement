using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PartyCenterManagement.Models;
using PartyCenterManagement.Models.ViewModels;
using PartyCenterManagement.Services;

namespace PartyCenterManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly PackageServices _packageServices;
        private readonly ReservationServices _reservationServices;

        public AdminController(PackageServices packageServices, ReservationServices reservationServices)
        {
            _packageServices = packageServices;
            _reservationServices = reservationServices;

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

            // CREATE
            if (id == null || id == 0)
            {
                return View(new UpsertPackageViewModel
                {
                    AllServices = services
                });
            }

            // EDIT
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

        // POST: Edit Package
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpsertPackage(UpsertPackageViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AllServices = await _packageServices.GetServices();
                return View(model);
            }

            // CREATE
            if (model.PackageID == 0)
            {
                await _packageServices.CreatePackage(
                    model.Name,
                    model.Price,
                    model.MaxGuests,
                    model.MaxLength
                );

                // Optional: attach services after creation
                var createdPackages = await _packageServices.GetPackages();
                var created = createdPackages.Last(); // or find by name if needed

                foreach (var serviceId in model.SelectedServiceIds)
                {
                    await _packageServices.AddServiceToPackage(created.PackageID, serviceId);
                }
            }
            else
            {
                // UPDATE
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

                // remove unchecked
                foreach (var serviceId in currentServiceIds)
                {
                    if (!model.SelectedServiceIds.Contains(serviceId))
                    {
                        await _packageServices.RemoveServiceFromPackage(model.PackageID, serviceId);
                    }
                }

                // add new
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
    }
}
