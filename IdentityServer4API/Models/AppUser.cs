using Microsoft.AspNetCore.Identity;

namespace IdentityServer4API.Models
{
    public class AppUser : IdentityUser
    {
        public string City { get; set; }
    }
}