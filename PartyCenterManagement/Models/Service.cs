namespace PartyCenterManagement.Models
{
    public class Service
    {
        public int ServiceID { get; set; }
        public string Serv { get; set; }

        public ICollection<PackageService> PackageServices { get; set; }
        public ICollection<ReservationService> ReservationServices { get; set; }
    }
}
