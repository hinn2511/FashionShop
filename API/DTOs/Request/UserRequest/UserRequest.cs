using API.DTOs.Request.AccountRequest;
using API.DTOs.Request.AuthenticationRequest;

namespace API.DTOs.Request.UserRequest
{
    public class SetUsersRoleRequest : BaseBulkRequest
    {
    }

     public class RemoveUsersRoleRequest : BaseBulkRequest
    {
    }

    public class ActivateUsersRequest : BaseBulkRequest
    {
    }

    public class DeactivateUsersRequest : BaseBulkRequest
    {
    }

    public class DeleteUsersRequest : BaseBulkRequest
    {
    }

    public class CreateUserRequest : RegisterRequest
    {
        public string RoleId { get; set; }
    }

    public class UpdateUserRequest : UpdateAccountRequest
    {
    }

    public class UpdateUserPasswordRequest
    {
        public string Password { get; set; }
    }
}