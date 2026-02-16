namespace PartyCenterManagement.Models
{
    public class Reservation
    {
        public int ReservationID { get; set; }
        public DateTime Date { get; set; }
        public int Length { get; set; }
        public int GuestCount { get; set; }

        public int PackageID { get; set; }
        public Package Package { get; set; }

        public string Note { get; set; }
        public string Status { get; set; }
        public string? UserID { get; set; }
        public UserProfile? User { get; set; }

        public string GFirstName { get; set; }
        public string GLastName { get; set; }
        public string GPhoneNumber { get; set; }

        public ICollection<ReservationService> ReservationServices { get; set; }
    }
}
