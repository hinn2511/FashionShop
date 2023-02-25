using API.Entities.User;

namespace API.DTOs.Response.AdminResponse
{
    public class UserRoleResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public UserStatus Status { get; set; }
        public string Role { get; set; }
        public string StatusString { get; set; }
    }

    
}