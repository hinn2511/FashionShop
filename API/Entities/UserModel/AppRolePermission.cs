using API.Entities.User;

namespace API.Entities.UserModel
{
    public class AppRolePermission : BaseEntity
    {
        public AppRolePermission()
        {
        }

        public AppRolePermission(int permissionId, int roleId)
        {
            PermissionId = permissionId;
            RoleId = roleId;
        }

        public AppPermission AppPermission { get; set; }
        public int PermissionId { get; set; }
        public AppRole AppRole { get; set; }
        public int RoleId { get; set; }
    }
}