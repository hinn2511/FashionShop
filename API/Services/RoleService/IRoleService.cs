using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs.Params;
using API.Entities.User;
using API.Entities.UserModel;
using API.Helpers;

namespace API.Services
{
    public interface IRoleService
    {
        Task<List<AppPermission>> GetRolePermissionsAsync(AppRole role);

        Task<List<AppPermission>> GetPermissionsByIdsAsync(List<int> permissionIds);

        Task<PagedList<AppUser>> GetUsersAndRolesAsync(int pageNumber = 1,
                                                        int pageSize = 10,
                                                        OrderBy orderBy = OrderBy.Ascending,
                                                        string field = "",
                                                        string query = "",
                                                        int roleId = 0,
                                                        List<UserStatus> userStatuses = null);

        Task<bool> ApplyRoleForUserAsync(AppUser user, AppRole role);

        Task<bool> RemoveRoleForUserAsync(AppUser user);

        Task<List<Tuple<AppRole, int>>> GetRolesSummaryAsync(bool showNoRole);

        Task<Tuple<AppRole, int>> GetRoleSummary(AppRole role);

        Task<AppRole> GetRoleByRoleNameAsync(string roleName);

        Task<AppRole> GetRoleByRoleIdAsync(int roleId);

        Task RemoveAllUsersInRoleAsync(int roleId);

        // Task UpdateRolesPermissionAsync(int roleId, List<int> permissionIds);

        Task RemovePermissionsForUsersAsync(int roleId, List<AppPermission> permissions);

        Task ApplyPermissionsForUsersAsync(int roleId, List<AppPermission> permissions);

    }
}