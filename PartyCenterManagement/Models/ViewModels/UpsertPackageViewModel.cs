namespace PartyCenterManagement.Models.ViewModels
{
    public class UpsertPackageViewModel
    {
        public int PackageID { get; set; }

        public string Name { get; set; }
        public double Price { get; set; }
        public int MaxGuests { get; set; }
        public int MaxLength { get; set; }

        public List<int> SelectedServiceIds { get; set; } = new List<int>();

        public List<Service>? AllServices { get; set; }
    }
}
