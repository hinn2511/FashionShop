using System.Collections.Generic;
using API.Entities.User;

namespace API.Entities.UserModel
{
    public class AppRole : BaseEntity
    {

        public string RoleName { get; set; }
        public ICollection<AppRolePermission> RolePermissions { get; set; }

        public ICollection<AppUser> Users { get; set; }
    }
}