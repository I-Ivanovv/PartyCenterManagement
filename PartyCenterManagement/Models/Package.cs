using System.ComponentModel.DataAnnotations;

namespace PartyCenterManagement.Models
{
    public class Package
    {
        public int PackageID { get; set; }

        public string Name { get; set; }
        public int Price { get; set; }
        public int? MaxGuests { get; set; }
        public int? MaxLength { get; set; }

        public ICollection<PackageService> PackageServices { get; set; }
    }
}
