using System.ComponentModel.DataAnnotations;

namespace PartyCenterManagement.Models.ViewModels
{
    public class UpsertServiceViewModel
    {
        public int ServiceID { get; set; }
        [Display(Name = "Service")]
        public string Serv { get; set; }
        public double Price { get; set; }
    }
}
