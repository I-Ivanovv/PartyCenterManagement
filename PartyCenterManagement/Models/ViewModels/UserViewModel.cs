using System.ComponentModel.DataAnnotations;

namespace PartyCenterManagement.Models.ViewModels
{
    public class UserViewModel
    {
        public string UserID { get; set; } = "";

        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Role")]
        public string? Role { get; set; }

        public List<string> RoleOptions { get; set; } = new List<string> { "Client", "Employee", "Admin" };
    }
}
