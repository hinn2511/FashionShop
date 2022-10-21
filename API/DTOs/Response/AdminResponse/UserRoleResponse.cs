namespace API.DTOs.Response.AdminResponse
{
    public class UserRoleResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Roles { get; set; }
    }
}