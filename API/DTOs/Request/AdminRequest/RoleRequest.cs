using System.Collections.Generic;

namespace API.DTOs.Request.AdminRequest
{
    public class CreateRoleRequest
    {
        public string RoleName { get; set; }
        public List<int> PermissionIds { get; set; }
    }

    public class UpdateRolePermissionRequest
    {
        public List<int> PermissionIds { get; set; }
    }

    public class CreateSystemPermissionRequest
    {
        public string PermissionName { get; set; }
        public string PermissionGroup { get; set; }
    }

    


    
}