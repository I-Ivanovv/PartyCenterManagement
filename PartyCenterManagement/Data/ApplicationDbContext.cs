using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PartyCenterManagement.Models;

namespace PartyCenterManagement.Data
{
    public class ApplicationDbContext
    : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<UserProfile> UserProfile => Set<UserProfile>();
        public DbSet<Package> Packages => Set<Package>(); 
        public DbSet<Service> Services => Set<Service>();
        public DbSet<Reservation> Reservations => Set<Reservation>();
        public DbSet<PackageService> PackageServices => Set<PackageService>();
        public DbSet<ReservationService> ReservationServices => Set<ReservationService>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserProfile>()
                .HasOne(p => p.User)
                .WithOne()
                .HasForeignKey<UserProfile>(p => p.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<PackageService>()
                .HasKey(ps => new { ps.PackageID, ps.ServiceID });

            builder.Entity<PackageService>()
                .HasOne(ps => ps.Package)
                .WithMany(p => p.PackageServices)
                .HasForeignKey(ps => ps.PackageID);

            builder.Entity<PackageService>()
                .HasOne(ps => ps.Service)
                .WithMany(s => s.PackageServices)
                .HasForeignKey(ps => ps.ServiceID);

            builder.Entity<Reservation>()
                .HasOne(r => r.Package)
                .WithMany()
                .HasForeignKey(r => r.PackageID)
                .OnDelete(DeleteBehavior.Restrict);


            builder.Entity<Reservation>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserID)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<ReservationService>()
                .HasKey(rs => new { rs.ReservationID, rs.ServiceID });

            builder.Entity<ReservationService>()
                .HasOne(rs => rs.Reservation)
                .WithMany(r => r.ReservationServices)
                .HasForeignKey(rs => rs.ReservationID);

            builder.Entity<ReservationService>()
                .HasOne(rs => rs.Service)
                .WithMany(s => s.ReservationServices)
                .HasForeignKey(rs => rs.ServiceID);

            builder.Entity<Service>().HasData(
                 new Service { ServiceID = 1, Serv = "Entertainer" },
                 new Service { ServiceID = 2, Serv = "Cake" },
                 new Service { ServiceID = 3, Serv = "Decorations" },
                 new Service { ServiceID = 4, Serv = "Catering" },
                 new Service { ServiceID = 5, Serv = "Costumes" }
                 );

            builder.Entity<Package>().HasData(
                new Package { PackageID = 1, Name = "Standard", Price = 150, MaxGuests = 40, MaxLength = 2 },
                new Package { PackageID = 2, Name = "Premium", Price = 250, MaxGuests = 60, MaxLength = 4 },
                new Package { PackageID = 3, Name = "Ultra", Price = 350 }
            );
        }
    }
}
