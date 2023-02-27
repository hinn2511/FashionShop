
using System.Collections.Generic;

namespace API.DTOs.Response.AdminResponse
{
    public class SystemRoleResponse
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        public int TotalUser { get; set; }
    }

    public class SystemPermissionResponse
    {   public int Id { get; set; }
        public string PermissionName { get; set; }
        public string PermissionGroup { get; set; }
        public bool IsAllowed { get; set; }
    }

}