using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PartyCenterManagement.Data;
using PartyCenterManagement.Models;
using PartyCenterManagement.Models.ViewModels;

namespace PartyCenterManagement.Services
{
    public class ReservationServices
    {
        private readonly ApplicationDbContext _db;

        public ReservationServices(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task CreateReservationGuest(DateTime dateTime,int length,int guestCount,int packageID,string note,string firstName,string lastName,string phoneNumber,
            List<Service> extraServices)
        {

            double packagePrice = await _db.Packages.Where(p => p.PackageID == packageID).Select(p => p.Price).FirstAsync();
            List<Service> packageServ = _db.PackageServices.Where(p => p.PackageID == packageID).Select(p => p.Service).ToList();
            double servicesPrice = extraServices.Sum(s => s.Price);
            packageServ.AddRange(extraServices);
            var reservation = new Reservation
            {
                Date = dateTime,
                Length = length,
                GuestCount = guestCount,
                PackageID = packageID,
                Note = note,
                Status = "Pending",
                Price = packagePrice + servicesPrice,
                Paid = false,
                GFirstName = firstName,
                GLastName = lastName,
                GPhoneNumber = phoneNumber
            };

            await _db.Reservations.AddAsync(reservation);
            await _db.SaveChangesAsync();

            var reservationServices = packageServ
                .Select(s => new ReservationService
                {
                    ReservationID = reservation.ReservationID,
                    ServiceID = s.ServiceID
                })
                .ToList();

            await _db.ReservationServices.AddRangeAsync(reservationServices);
            await _db.SaveChangesAsync();
        }

        public async Task CreateReservationUser(DateTime dateTime,int length, int guestCount,int packageID,string note,string userID,List<Service> extraServices)
        {
            double packagePrice = await _db.Packages.Where(p => p.PackageID == packageID).Select(p => p.Price).FirstAsync();
            List<Service> packageServ = _db.PackageServices.Where(p => p.PackageID == packageID).Select(p => p.Service).ToList();
            double servicesPrice = extraServices.Sum(s => s.Price);
            packageServ.AddRange(extraServices);
            var reservation = new Reservation
            {
                Date = dateTime,
                Length = length,
                GuestCount = guestCount,
                PackageID = packageID,
                Note = note,
                Status = "Pending",
                Price = packagePrice + servicesPrice,
                Paid = false,
                UserID = userID
            };

            await _db.Reservations.AddAsync(reservation);
            await _db.SaveChangesAsync();

            var reservationServices = packageServ
                .Select(s => new ReservationService
                {
                    ReservationID = reservation.ReservationID,
                    ServiceID = s.ServiceID
                })
                .ToList();

            await _db.ReservationServices.AddRangeAsync(reservationServices);
            await _db.SaveChangesAsync();
        }

        public async Task<List<Reservation>> GetUserReservatrionsAsync(IdentityUser user)
        {
            var reservations = _db.Reservations
            .Where(r => r.UserID == user.Id)
            .Include(r => r.Package)
            .Include(r => r.ReservationServices)
                .ThenInclude(rs => rs.Service)
            .OrderByDescending(r => r.Date)
            .ToListAsync();
            return await reservations;
        }


        public async Task CancelReservartionAsync(int id)
        {
            var reservation = await _db.Reservations.FindAsync(id);
            reservation.Status = "Cancelled";

            await _db.SaveChangesAsync();

        }

        public async Task<Reservation> GetReservationByIdAsync(int id)
        {
            return await _db.Reservations
                .Include(r => r.ReservationServices)
                .ThenInclude(rs => rs.Service)
                .FirstOrDefaultAsync(r => r.ReservationID == id);
        }

        public async Task UpdateReservationAsync(int reservationId, DateTime dateTime, int length, int guestCount, int packageID, string note, List<Service> extraServices)
        {
            var reservation = await _db.Reservations
                .Include(r => r.ReservationServices)
                .FirstOrDefaultAsync(r => r.ReservationID == reservationId);

            if (reservation == null)
                return;

            reservation.Date = dateTime;
            reservation.Length = length;
            reservation.GuestCount = guestCount;
            reservation.PackageID = packageID;
            reservation.Note = note;

            double packagePrice = await _db.Packages
                .Where(p => p.PackageID == packageID)
                .Select(p => p.Price)
                .FirstAsync();

            List<Service> packageServ = _db.PackageServices
                .Where(p => p.PackageID == packageID)
                .Select(p => p.Service)
                .ToList();

            double servicesPrice = extraServices.Sum(s => s.Price);

            packageServ.AddRange(extraServices);

            reservation.Price = packagePrice + servicesPrice;

            _db.ReservationServices.RemoveRange(reservation.ReservationServices);

            var reservationServices = packageServ
                .Select(s => new ReservationService
                {
                    ReservationID = reservation.ReservationID,
                    ServiceID = s.ServiceID
                })
                .ToList();

            await _db.ReservationServices.AddRangeAsync(reservationServices);

            await _db.SaveChangesAsync();
        }

        public async Task<AdminDashboardViewModel> GetDashboardStats(DateTime? start, DateTime? end)
        {
            var query = _db.Reservations
                .Include(r => r.Package)
                .AsQueryable();

            if (start != null)
            {
                start = start.Value.Date;
                query = query.Where(r => r.Date >= start);
            }

            if (end != null)
            {
                end = end.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(r => r.Date <= end);
            }

            var reservations = await query.ToListAsync();

            var totalReservations = reservations.Count;
            var revenue = reservations.Sum(r => r.Price);
            var mostPopular = reservations
                .GroupBy(r => r.Package.Name)
                .OrderByDescending(g => g.Count())
                .FirstOrDefault()?.Key ?? "N/A";
            var upcoming = reservations
                .Where(r => r.Status != "Cancelled")
                .Count(r => r.Date > DateTime.Now);

            return new AdminDashboardViewModel
            {
                TotalReservations = totalReservations,
                TotalRevenue = revenue,
                MostPopularPackage = mostPopular,
                UpcomingReservations = upcoming,
                StartDate = start,
                EndDate = end
            };
        }
        public async Task UpdateStatusAsync(int id, string status)
        {
            var reservation = await _db.Reservations.FindAsync(id);
            if (reservation != null)
            {
                reservation.Status = status;
                await _db.SaveChangesAsync();
            }
        }

        public async Task MarkAsPaidAsync(int id)
        {
            var reservation = await _db.Reservations.FindAsync(id);
            if (reservation != null)
            {
                reservation.Paid = true;
                await _db.SaveChangesAsync();
            }
        }

        public async Task<List<Reservation>> GetAllReservationsAsync(DateTime? date = null, bool upcomingOnly = false)
        {
            var query = _db.Reservations
                .Include(r => r.Package)
                .Include(r => r.User)
                .Include(r => r.ReservationServices)
                    .ThenInclude(rs => rs.Service)
                .AsQueryable();

            if (date.HasValue)
            {
                query = query.Where(r => r.Date.Date == date.Value.Date);
            }

            if (upcomingOnly)
            {
                query = query.Where(r => r.Date >= DateTime.Now && r.Status != "Cancelled");
            }

            return await query.OrderByDescending(r => r.Date).ToListAsync();
        }
    }
}