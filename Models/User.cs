using Microsoft.AspNetCore.Identity;

namespace Identity.Models
{
    public class ApplicationUser:IdentityUser<int>

    {
        public string Name { get; set; }
        public string Family { get; set; }
    } 
    public class ApplicationRole:IdentityRole<int>
    {
        
        
    }
}
