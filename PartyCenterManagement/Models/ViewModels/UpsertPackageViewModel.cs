using System.ComponentModel.DataAnnotations;

namespace PartyCenterManagement.Models.ViewModels
{
    public class UpsertPackageViewModel
    {
        public int PackageID { get; set; }

        public string Name { get; set; }
        public double Price { get; set; }
        [Display(Name = "Max Guests")]
        public int MaxGuests { get; set; }
        [Display(Name = "Max Length")]
        public int MaxLength { get; set; }

        public List<int> SelectedServiceIds { get; set; } = new List<int>();

        public List<Service>? AllServices { get; set; }
    }
}
