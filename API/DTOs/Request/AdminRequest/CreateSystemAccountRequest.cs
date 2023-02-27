using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Request.AdminRequest
{
    public class CreateSystemAccountRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [StringLength(32, MinimumLength = 8)]
        public string Password { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string RoleName { get; set; }
    }
}