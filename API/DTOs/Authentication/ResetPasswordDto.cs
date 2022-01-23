using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class ResetPasswordDto
    {
        public string Username { get; set; }
        public string OldPassword { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }
}