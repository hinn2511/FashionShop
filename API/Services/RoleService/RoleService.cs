


using System.Collections.Generic;
using System.Threading.Tasks;
using API.Entities.User;
using API.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using API.Helpers;
using API.DTOs.Params;
using Microsoft.EntityFrameworkCore;
using API.Extensions;
using API.Entities.UserModel;
using API.Data;
using System;

namespace API.Services
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly RoleManager<AppPermission> _roleManager;
        private readonly DataContext _context;
        private readonly UserManager<AppUser> _userManager;
        public RoleService(DataContext context, UserManager<AppUser> userManager, RoleManager<AppPermission> roleManager, IUnitOfWork unitOfWork)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<AppPermission>> GetPermissionsByIdsAsync(List<int> permissionIds)
        {
            var result = new List<AppPermission>();

            await permissionIds.ForEachAsync(async i =>
            {
                var permission = await _roleManager.FindByIdAsync(i.ToString());
                result.Add(permission);
            });

            return result;
        }

        public async Task<AppRole> GetRoleByRoleIdAsync(int roleId)
        {
            return await _unitOfWork.AppRoleRepository.GetFirstBy(x => x.Id == roleId);
        }

        public async Task<AppRole> GetRoleByRoleNameAsync(string roleName)
        {
            return await _unitOfWork.AppRoleRepository.GetFirstBy(x => x.RoleName == roleName);
        }

        public async Task<List<AppPermission>> GetRolePermissionsAsync(AppRole role)
        {
            var rolePermissions = await _unitOfWork.AppRolePermissionRepository.GetAllBy(x => x.RoleId == role.Id);

            var permissionIds = rolePermissions.Select(x => x.PermissionId);

            var result = new List<AppPermission>();

            await permissionIds.ForEachAsync(async i =>
            {
                var permission = await _roleManager.FindByIdAsync(i.ToString());
                result.Add(permission);
            });

            return result;

        }

        public async Task<PagedList<AppUser>> GetUsersAndRolesAsync(int pageNumber, int pageSize, OrderBy orderBy, string field, string query,
            int roleId, List<UserStatus> userStatuses)
        {
            var systemUsers = _userManager.Users.AsQueryable();

            if (roleId > 0)
                systemUsers = systemUsers.Where(x => x.RoleId == roleId);

            if (roleId == 0)
                systemUsers = systemUsers.Where(x => x.RoleId == 0 || x.RoleId == null);

            if (userStatuses != null)
            {
                if (userStatuses.Count == 1)
                {
                    if (userStatuses.Contains(UserStatus.Deactivated))
                        systemUsers = systemUsers.Where(x => x.LockoutEnabled && x.LockoutEnd > DateTimeOffset.UtcNow);
                    else
                        systemUsers = systemUsers.Where(x => !x.LockoutEnabled || (x.LockoutEnabled && x.LockoutEnd < DateTimeOffset.UtcNow));
                }
            }

            if (!string.IsNullOrEmpty(query))
            {
                var words = query.Replace("@", " ").RemoveSpecialCharacters().ToUpper().Split(" ").Distinct();
                foreach (var word in words)
                {
                    systemUsers = systemUsers.Where(x => x.UserName.ToUpper().Contains(word)
                                                        || x.FirstName.ToUpper().Contains(word)
                                                        || x.LastName.ToUpper().Contains(word)
                                                        || x.Email.ToUpper().Contains(word)
                                                    );
                }
            }


            if (orderBy == OrderBy.Ascending)
            {
                systemUsers = field switch
                {
                    "firstName" => systemUsers.OrderBy(p => p.FirstName),
                    "lastName" => systemUsers.OrderBy(p => p.LastName),
                    "username" => systemUsers.OrderBy(p => p.UserName),
                    "id" => systemUsers.OrderBy(p => p.Id),
                    _ => systemUsers.OrderBy(p => p.Created)
                };
            }
            else
            {
                systemUsers = field switch
                {
                    "firstName" => systemUsers.OrderByDescending(p => p.FirstName),
                    "lastName" => systemUsers.OrderByDescending(p => p.LastName),
                    "username" => systemUsers.OrderByDescending(p => p.UserName),
                    "id" => systemUsers.OrderBy(p => p.Id),
                    _ => systemUsers.OrderByDescending(p => p.Created)
                };
            }


            systemUsers = systemUsers.Include(x => x.Role);

            return await PagedList<AppUser>.CreateAsync(systemUsers.AsNoTracking(), pageNumber, pageSize);
        }
        public async Task<List<Tuple<AppRole, int>>> GetRolesSummaryAsync(bool showNoRole)
        {
            var result = new List<Tuple<AppRole, int>>();

            var roleIds = await _context.Users.Select(x => x.RoleId).ToListAsync();

            var roles = await _context.AppRoles.ToListAsync();

            foreach(var role in roles)
            {
                result.Add(Tuple.Create(role, roleIds.Where(x => x == role.Id).Count()));
            }

            if (showNoRole)
            {
                var noRole = new AppRole();
                noRole.RoleName = "No role available";
                var noRoleCount = await _context.Users.Where(x => x.RoleId == 0 || x.RoleId == null).CountAsync();
                result.Add(Tuple.Create(noRole, noRoleCount));
            }
            return result;
        }

        public async Task<Tuple<AppRole, int>> GetRoleSummary(AppRole role)
        {
            var count = await _context.Users.Where(x => x.RoleId == role.Id).CountAsync();
            return Tuple.Create(role, count);
        }

        public async Task<bool> ApplyRoleForUserAsync(AppUser user, AppRole role)
        {
            var permissions = await GetRolePermissionsAsync(role);

            var userExistedPermissions = await GetUserPermissionsAsync(user);

            if (userExistedPermissions == null || !userExistedPermissions.Any())
            {
                var addResult = await _userManager.AddToRolesAsync(user, permissions.Select(x => x.Name));
                return addResult.Succeeded;
            }
            else
            {
                var newPermission = permissions.Select(x => x.Name).Except(userExistedPermissions);

                var removePermission = userExistedPermissions.Except(permissions.Select(x => x.Name));

                var addToRoleResult = await _userManager.AddToRolesAsync(user, newPermission);

                if (!addToRoleResult.Succeeded)
                    return false;

                var removeFromRoleResult = await _userManager.RemoveFromRolesAsync(user, removePermission);

                if (!removeFromRoleResult.Succeeded)
                    return false;

                user.RoleId = role.Id;

                var updateUserResult = await _userManager.UpdateAsync(user);

                if (!updateUserResult.Succeeded)
                    return false;
            }
            return true;
        }

        public async Task<bool> RemoveRoleForUserAsync(AppUser user)
        {
            var userExistedPermissions = await GetUserPermissionsAsync(user);

            var removePermissionResult = await _userManager.RemoveFromRolesAsync(user, userExistedPermissions);

            if (!removePermissionResult.Succeeded)
                return false;

            user.RoleId = null;

            var updateUserResult = await _userManager.UpdateAsync(user);

            if (!updateUserResult.Succeeded)
                return false;

            return true;
        }

        public async Task RemoveAllUsersInRoleAsync(int roleId)
        {
            var role = await GetRoleByRoleIdAsync(roleId);

            var rolePermissions = await _unitOfWork.AppRolePermissionRepository.GetAllAndIncludeAsync(x => x.RoleId == role.Id, "AppPermission", true);

            var permissions = rolePermissions.Select(x => x.AppPermission).ToList();

            var usersHaveRole = await _context.Users.Where(x => x.RoleId == role.Id).ToListAsync();

            await usersHaveRole.ForEachAsync(async u =>
            {
                await _userManager.RemoveFromRolesAsync(u, permissions.Select(x => x.Name));
                u.RoleId = null;
                await _userManager.UpdateAsync(u);
            });
        }

        public async Task RemovePermissionsForUsersAsync(int roleId, List<AppPermission> permissions)
        {

            var usersInRole = await _context.Users.Where(x => x.RoleId == roleId).ToListAsync();
            if (usersInRole != null && usersInRole.Any())
            {
                if (permissions != null && permissions.Any())
                {
                    await usersInRole.ForEachAsync(async u =>
                    {
                        await permissions.ForEachAsync(async p =>
                        {
                            await _userManager.RemoveFromRoleAsync(u, p.Name);
                        });
                    });
                }

            }
        }

        public async Task ApplyPermissionsForUsersAsync(int roleId, List<AppPermission> permissions)
        {

            var usersInRole = await _context.Users.Where(x => x.RoleId == roleId).ToListAsync();
            if (usersInRole != null && usersInRole.Any())
            {
                if (permissions != null && permissions.Any())
                {
                    await usersInRole.ForEachAsync(async u =>
                   {
                       await permissions.ForEachAsync(async p =>
                       {
                           await _userManager.AddToRoleAsync(u, p.Name);
                       });
                   });
                }

            }
        }



        #region private method


        private async Task<List<string>> GetUserPermissionsAsync(AppUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            return roles.ToList();
        }




        #endregion
    }
}