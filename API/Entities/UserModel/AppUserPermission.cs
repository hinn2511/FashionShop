using Microsoft.AspNetCore.Identity;

namespace API.Entities.User
{
    public class AppUserPermission : IdentityUserRole<int>
    {
        public AppUser User { get; set; }
        public AppPermission Role { get; set; }
    }
}