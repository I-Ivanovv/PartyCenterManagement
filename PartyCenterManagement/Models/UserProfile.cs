using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace PartyCenterManagement.Models
{
    public class UserProfile
    {
        [Key]
        public string UserID { get; set; } = "";
        public IdentityUser? User { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

    }
}
