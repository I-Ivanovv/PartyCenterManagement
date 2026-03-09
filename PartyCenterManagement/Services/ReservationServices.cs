using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PartyCenterManagement.Data;
using PartyCenterManagement.Models;

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
    }
}