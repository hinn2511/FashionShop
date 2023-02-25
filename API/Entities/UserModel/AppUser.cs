using System;
using System.Collections.Generic;
using API.Entities.UserModel;
using Microsoft.AspNetCore.Identity;

namespace API.Entities.User
{
    public class AppUser : IdentityUser<int>
    {
        public DateTime DateOfBirth { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime LastActive { get; set; } = DateTime.UtcNow;
        public ICollection<AppUserPermission> UserPermissions { get; set; }
        public List<RefreshToken> RefreshTokens { get; set; }
        public int? RoleId { get; set; }
        public AppRole Role { get; set; }

    }

    public enum UserStatus
    {
        Active,
        Deactivated
    }
}