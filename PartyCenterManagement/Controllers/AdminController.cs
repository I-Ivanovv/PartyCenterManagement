using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PartyCenterManagement.Models;
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
        public async Task<IActionResult> UpdatePackage(int packageID, double price, int maxGuests, int maxLength)
        {
            await _packageServices.UpdatePackage(packageID, price, maxGuests, maxLength);
            return RedirectToAction("Packages");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateService(int serviceID, string serv, double price)
        {
            await _packageServices.UpdateService(serviceID, serv, price);
            return RedirectToAction("Packages");
        }

        [HttpPost]
        public async Task<IActionResult> CreatePackage(string name, double price, int maxGuests, int maxLength)
        {
            await _packageServices.CreatePackage(name, price, maxGuests, maxLength);
            return RedirectToAction("Packages");
        }

        [HttpPost]
        public async Task<IActionResult> DeletePackage(int id)
        {
            await _packageServices.DeletePackage(id);
            return RedirectToAction("Packages");
        }

        [HttpPost]
        public async Task<IActionResult> CreateService(string name, double price)
        {
            await _packageServices.CreateService(name, price);
            return RedirectToAction("Packages");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteService(int id)
        {
            await _packageServices.DeleteService(id);
            return RedirectToAction("Packages");
        }

        [HttpPost]
        public async Task<IActionResult> AddServiceToPackage(int packageId, int serviceId)
        {
            await _packageServices.AddServiceToPackage(packageId, serviceId);
            return RedirectToAction("Packages");
        }
        [HttpPost]
        public async Task<IActionResult> RemoveServiceFromPackage(int packageId, int serviceId)
        {
            await _packageServices.RemoveServiceFromPackage(packageId, serviceId);
            return RedirectToAction("Packages");
        }
    }
}
