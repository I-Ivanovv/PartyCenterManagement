using Microsoft.AspNetCore.Mvc;
using PartyCenterManagement.Models;
using PartyCenterManagement.Services;

namespace PartyCenterManagement.Controllers
{
    public class EmployeeController : Controller
    {
        ReservationServices _reservationService;
        public EmployeeController(ReservationServices reservationService)
        {
            _reservationService = reservationService;
        }
        public async Task<IActionResult> ManageReservations(DateTime? filterDate, bool upcomingOnly = false)
        {
            var reservations = await _reservationService.GetAllReservationsAsync(filterDate, upcomingOnly);
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
