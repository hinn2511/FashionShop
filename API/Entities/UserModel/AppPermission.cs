using System.Collections.Generic;
using API.Entities.UserModel;
using Microsoft.AspNetCore.Identity;

namespace API.Entities.User
{
    public class AppPermission : IdentityRole<int>
    {
        public ICollection<AppUserPermission> UserPermissions { get; set; }

        public ICollection<AppRolePermission> RolePermissions { get; set; }

        public string PermissionGroup { get; set; }
    }
}