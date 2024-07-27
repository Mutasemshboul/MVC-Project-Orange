using Microsoft.AspNetCore.Identity;

namespace MVC_Project_Orange.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string? Image { get; set; }
        public string? Address { get; set; }
        public bool isDeleted { get; set; }
    }
}
