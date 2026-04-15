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

                // Security check for clients editing close to the date
                if ((reservation.Date - DateTime.Now).TotalDays <= 5 && User.IsInRole("Client"))
                {
                    TempData["Error"] = "Reservations cannot be edited within 5 days.";
                    return RedirectToAction("MyReservations");
                }
                if (reservation.Status == "Cancelled" && User.IsInRole("Client"))
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

            // 1. Validation Logic
            if (selectedPackage == null)
                ModelState.AddModelError("", "Package not found.");

            // Employee fix: only clients are restricted from picking old dates
            bool isClient = User.IsInRole("Client");
            if (model.Date == null || (isClient && model.Date < DateTime.Today))
                ModelState.AddModelError("Date", "Date must be today or later.");

            if (model.Time == null)
                ModelState.AddModelError("Time", "Please choose a time.");

            if (selectedPackage != null)
            {
                if (model.GuestCount > selectedPackage.MaxGuests)
                    ModelState.AddModelError("GuestCount", $"Maximum guests: {selectedPackage.MaxGuests}");

                if (model.Length > selectedPackage.MaxLength)
                    ModelState.AddModelError("Length", $"Maximum length: {selectedPackage.MaxLength}");
            }

            // 2. Overlap Check (Only blocks if existing status is "Confirmed")
            if (ModelState.IsValid && model.Date.HasValue && model.Time.HasValue)
            {
                DateTime newStart = model.Date.Value.Date + model.Time.Value;
                DateTime newEnd = newStart.AddHours((double)model.Length + 1); // +1 hour for cleanup

                var allReservations = await _reservationServices.GetAllReservationsAsync();

                bool isOverlapping = allReservations.Any(r =>
                    r.Status == "Confirmed" && // Only check against Confirmed slots
                    r.ReservationID != model.ReservationID && // Don't collide with self
                    r.Date.Date == model.Date.Value.Date &&
                    newStart < r.Date.AddHours((double)r.Length + 1) &&
                    newEnd > r.Date
                );

                if (isOverlapping)
                    ModelState.AddModelError("Time", "This time slot is unavailable (includes 1h cleanup time).");
            }

            if (!ModelState.IsValid)
            {
                model.Packages = packages;
                model.Services = services;
                model.IsGuest = (await _userManager.GetUserAsync(User)) == null;
                return View(model);
            }

            // 3. Persistence Logic
            var extraServices = services
                .Where(s => model.ServiceIds != null && model.ServiceIds.Contains(s.ServiceID))
                .ToList();

            var user = await _userManager.GetUserAsync(User);
            DateTime reservationDateTime = model.Date.Value.Date + model.Time.Value;

            if (model.ReservationID != null)
            {
                // Check edit permission again for safety
                var reservation = await _reservationServices.GetReservationByIdAsync(model.ReservationID.Value);
                if (isClient && (reservation.Date - DateTime.Now).TotalDays <= 5)
                {
                    TempData["Error"] = "Cannot update within 5 days of the event.";
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
                        reservationDateTime, model.Length, model.GuestCount,
                        model.PackageID, model.Note, user.Id, extraServices);
                }
                else
                {
                    await _reservationServices.CreateReservationGuest(
                        reservationDateTime, model.Length, model.GuestCount,
                        model.PackageID, model.Note, model.FirstName,
                        model.LastName, model.PhoneNumber, extraServices);
                }
            }

            return User.IsInRole("Client")
                ? RedirectToAction("Index", "Home")
                : RedirectToAction("ManageReservations", "Employee");
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

        // This allows you to visit /Admin/ReservationCalendar
        public IActionResult ReservationCalendar()
        {
            // MVC will automatically look for Views/Admin/ReservationCalendar.cshtml
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetCalendarEvents()
        {
            var reservations = await _reservationServices.GetAllReservationsAsync();
            var activeRes = reservations.Where(r => r.Status != "Cancelled" && r.Status != "Declined").ToList();

            var eventList = new List<object>();

            // 1. ADD SUMMARIES (For Month View)
            var summaries = activeRes.GroupBy(r => r.Date.Date).Select(g => new {
                title = $"{g.Count(r => r.Status == "Confirmed")} Approved | {g.Count(r => r.Status == "Pending")} Pending",
                start = g.Key.ToString("yyyy-MM-dd"),
                allDay = true, // Key for Month View look
                className = "month-summary-event",
                backgroundColor = g.Any(r => r.Status == "Pending") ? "#ffc107" : "#28a745",
                borderColor = "transparent",
                textColor = g.Any(r => r.Status == "Pending") ? "#000" : "#fff"
            });
            eventList.AddRange(summaries);

            // 2. ADD INDIVIDUAL NAMES (For Week View)
            var individuals = activeRes.Select(r => new {
                title = r.User != null ? $"{r.User.FirstName}" : $"{r.GFirstName}",
                start = r.Date.ToString("yyyy-MM-ddTHH:mm:ss"),
                end = r.Date.AddHours(r.Length).ToString("yyyy-MM-ddTHH:mm:ss"),
                allDay = false,
                className = "week-detail-event",
                backgroundColor = r.Status == "Pending" ? "#ffc107" : "#28a745",
                borderColor = "transparent",
                textColor = r.Status == "Pending" ? "#000" : "#fff"
            });
            eventList.AddRange(individuals);

            return Json(eventList);
        }
    

        [HttpGet]
        public async Task<IActionResult> GetReservationsByDay(DateTime date)
        {
            var reservations = await _reservationServices.GetAllReservationsAsync(date, date);

            var result = reservations.Select(r => new {
                time = r.Date.ToString("HH:mm"),
                status = r.Status,
                client = r.User != null ? $"{r.User.FirstName} {r.User.LastName}" : $"{r.GFirstName} {r.GLastName}",
                // Ensure this property name matches what's in the JS (case-sensitive)
                length = r.Length
            });

            return Json(result);
        }
    }


}