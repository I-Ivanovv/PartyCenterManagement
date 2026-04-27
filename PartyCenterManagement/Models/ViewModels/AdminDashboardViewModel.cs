namespace PartyCenterManagement.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalReservations { get; set; }
        public int ConfirmedReservations { get; set; } 
        public double TotalRevenue { get; set; }
        public string MostPopularPackage { get; set; }
        public int UpcomingReservations { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
