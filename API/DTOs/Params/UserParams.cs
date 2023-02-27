using System.Collections.Generic;
using API.Entities.User;

namespace API.DTOs.Params
{
    public class UserParams : PaginationParams
    {
        public int RoleId { get; set; }
        public List<UserStatus> UserStatus { get; set; } = new List<UserStatus>() {};
    }

   
}