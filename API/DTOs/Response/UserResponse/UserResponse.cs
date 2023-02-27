using System;
using API.Entities.User;

namespace API.DTOs.Response.UserResponse
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public DateTime LastSeen { get; set; }
        public double TotalAmount { get; set; }
        public double TotalOrder { get; set; }
        public string Role { get; set; }
        public UserStatus Status { get; set; }
        public string StatusString { get; set; }
    }
}