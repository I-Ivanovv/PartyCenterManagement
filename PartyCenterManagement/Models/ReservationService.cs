namespace PartyCenterManagement.Models
{
    public class ReservationService
    {
        public int ReservationID { get; set; }
        public Reservation Reservation { get; set; }

        public int ServiceID { get; set; }
        public Service Service { get; set; }
    }
}
