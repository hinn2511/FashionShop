namespace API.DTOs.Request.AuthenticationRequest
{
    public class ResetPasswordRequest
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}