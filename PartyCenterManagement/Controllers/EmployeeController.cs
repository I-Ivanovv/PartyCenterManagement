using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PartyCenterManagement.Models;
using PartyCenterManagement.Services;

namespace PartyCenterManagement.Controllers
{
    [Authorize(Roles = "Admin,Employee")]
    public class EmployeeController : Controller
    {
        ReservationServices _reservationService;
        public EmployeeController(ReservationServices reservationService)
        {
            _reservationService = reservationService;
        }
        public async Task<IActionResult> ManageReservations(DateTime? startDate, DateTime? endDate, bool upcomingOnly = false)
        {
            var reservations = await _reservationService.GetAllReservationsAsync(startDate, endDate, upcomingOnly);

            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;

            return View(reservations);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            await _reservationService.UpdateStatusAsync(id, status);
            return RedirectToAction(nameof(ManageReservations));
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsPaid(int id)
        {
            await _reservationService.MarkAsPaidAsync(id);
            return RedirectToAction(nameof(ManageReservations));
        }
    }
}
