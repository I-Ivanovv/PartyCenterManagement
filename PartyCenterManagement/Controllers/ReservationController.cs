using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PartyCenterManagement.Models;
using PartyCenterManagement.Services;
using PartyCenterManagement.ViewModels;

namespace PartyCenterManagement.Controllers
{
    public class ReservationController : Controller
    {
        private readonly ReservationServices _reservationServices;
        private readonly PackageServices _packageServices;
        private readonly UserManager<IdentityUser> _userManager;

        public ReservationController(
            ReservationServices reservationServices,
            PackageServices packageServices,
            UserManager<IdentityUser> userManager)
        {
            _reservationServices = reservationServices;
            _packageServices = packageServices;
            _userManager = userManager;
        }

        public async Task<IActionResult> Reserve(int? packageId, int? id)
        {
            var model = new ReserveViewModel
            {
                Packages = await _packageServices.GetPackages(),
                Services = await _packageServices.GetServices(),
                IsGuest = (await _userManager.GetUserAsync(User)) == null
            };

            if (id != null)
            {
                var reservation = await _reservationServices.GetReservationByIdAsync(id.Value);

                if (reservation == null)
                    return NotFound();

                if ((reservation.Date - DateTime.Now).TotalDays <= 5)
                {
                    TempData["Error"] = "Reservations cannot be edited within 5 days.";
                    return RedirectToAction("MyReservations");
                }
                if (reservation.Status == "Cancelled")
                {
                    TempData["Error"] = "Cancelled reservations cannot be edited.";
                    return RedirectToAction("MyReservations");
                }

                model.ReservationID = reservation.ReservationID;
                model.PackageID = reservation.PackageID;
                model.Date = reservation.Date.Date;
                model.Time = reservation.Date.TimeOfDay;
                model.Length = reservation.Length;
                model.GuestCount = reservation.GuestCount;
                model.Note = reservation.Note;

                model.ServiceIds = reservation.ReservationServices
                    .Select(s => s.ServiceID)
                    .ToList();
            }
            else
            {
                model.PackageID = packageId ?? 0;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reserve(ReserveViewModel model)
        {
            var packages = await _packageServices.GetPackages();
            var services = await _packageServices.GetServices();

            var selectedPackage = packages.FirstOrDefault(p => p.PackageID == model.PackageID);

            if (selectedPackage == null)
                ModelState.AddModelError("", "Package not found.");

            if (model.Date == null || model.Date < DateTime.Today)
                ModelState.AddModelError("Date", "Date must be today or later.");

            if (model.Time == null)
                ModelState.AddModelError("Time", "Please choose a time.");

            if (model.GuestCount > selectedPackage.MaxGuests)
                ModelState.AddModelError("GuestCount", $"Maximum guests: {selectedPackage.MaxGuests}");

            if (model.Length > selectedPackage.MaxLength)
                ModelState.AddModelError("Length", $"Maximum length: {selectedPackage.MaxLength}");

            if (!ModelState.IsValid)
            {
                model.Packages = packages;
                model.Services = services;
                model.IsGuest = (await _userManager.GetUserAsync(User)) == null;
                return View(model);
            }

            var extraServices = services
                .Where(s => model.ServiceIds != null && model.ServiceIds.Contains(s.ServiceID))
                .ToList();

            var user = await _userManager.GetUserAsync(User);

            DateTime reservationDateTime = model.Date.Value.Date + model.Time.Value;

            if (model.ReservationID != null)
            {
                var reservation = await _reservationServices.GetReservationByIdAsync(model.ReservationID.Value);

                if ((reservation.Date - DateTime.Now).TotalDays <= 5)
                {
                    TempData["Error"] = "Reservations cannot be edited within 5 days.";
                    return RedirectToAction("MyReservations");
                }
                if (reservation.Status == "Cancelled")
                {
                    TempData["Error"] = "Cancelled reservations cannot be edited.";
                    return RedirectToAction("MyReservations");
                }

                await _reservationServices.UpdateReservationAsync(
                    model.ReservationID.Value,
                    reservationDateTime,
                    model.Length,
                    model.GuestCount,
                    model.PackageID,
                    model.Note,
                    extraServices);
            }
            else
            {
                if (user != null)
                {
                    await _reservationServices.CreateReservationUser(
                        reservationDateTime,
                        model.Length,
                        model.GuestCount,
                        model.PackageID,
                        model.Note,
                        user.Id,
                        extraServices);
                }
                else
                {
                    await _reservationServices.CreateReservationGuest(
                        reservationDateTime,
                        model.Length,
                        model.GuestCount,
                        model.PackageID,
                        model.Note,
                        model.FirstName,
                        model.LastName,
                        model.PhoneNumber,
                        extraServices);
                }
            }

            return RedirectToAction("Index", "Home");
        }


    [Authorize] // Only logged-in users can view    
    public async Task<IActionResult> MyReservations()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return RedirectToAction("Login", "Account");

            var userRes = await _reservationServices.GetUserReservatrionsAsync(user);

        return View(userRes);
    }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CancelReservation(int id)
        {
            await _reservationServices.CancelReservartionAsync(id);

            return RedirectToAction("MyReservations");
        }
    }
}