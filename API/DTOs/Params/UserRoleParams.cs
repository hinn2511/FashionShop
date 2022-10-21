using System.Collections.Generic;

namespace API.DTOs.Params
{
    public class UserRoleParams : PaginationParams
    {
        public List<string> ExcludeRoles { get; set; }
    }
}