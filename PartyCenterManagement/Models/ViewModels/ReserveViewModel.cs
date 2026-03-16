using PartyCenterManagement.Models;

namespace PartyCenterManagement.ViewModels
{
    public class ReserveViewModel
    {
        public List<Package> Packages { get; set; } = new();
        public List<Service> Services { get; set; } = new();
        public int? ReservationID { get; set; }

        public bool IsGuest { get; set; }

        public int PackageID { get; set; }

        public DateTime? Date { get; set; }

        public TimeSpan? Time { get; set; }

        public int GuestCount { get; set; }

        public int Length { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Note { get; set; }

        public List<int>? ServiceIds { get; set; }
    }
}