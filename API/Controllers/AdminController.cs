using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using API.DTOs.Params;
using API.DTOs.Request.AdminRequest;
using API.DTOs.Request.ConfigurationRequest;
using API.DTOs.Response;
using API.DTOs.Response.AdminResponse;
using API.Entities;
using API.Entities.User;
using API.Entities.UserModel;
using API.Entities.WebPageModel;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{

    public class AdminController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppPermission> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRoleService _roleService;

        public AdminController(UserManager<AppUser> userManager, RoleManager<AppPermission> roleManager, IUnitOfWork unitOfWork, IMapper mapper, IRoleService roleService)
        {
            _roleService = roleService;
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        #region GET method


        #endregion

        #region system account
        [Authorize(Roles = "CreateAccount")]
        [HttpPost("create-account")]
        public async Task<ActionResult> CreateSystemAccount(CreateSystemAccountRequest createSystemAccountRequest)
        {
            if (await UserExist(createSystemAccountRequest.Username))
                return BadRequest(new BaseResponseMessage(false, HttpStatusCode.BadRequest, "Account already exist."));

            if (string.IsNullOrEmpty(createSystemAccountRequest.RoleName))
                return BadRequest(new BaseResponseMessage(false, HttpStatusCode.BadRequest, "Role is not valid."));

            var role = await _unitOfWork.AppRoleRepository.GetFirstBy(x => x.RoleName == createSystemAccountRequest.RoleName);

            if (role == null)
                return BadRequest(new BaseResponseMessage(false, HttpStatusCode.BadRequest, "Role is not valid."));

            var user = _mapper.Map<AppUser>(createSystemAccountRequest);

            user.UserName = createSystemAccountRequest.Username.ToLower();

            user.RoleId = role.Id;

            var result = await _userManager.CreateAsync(user, createSystemAccountRequest.Password);

            if (!result.Succeeded)
                return BadRequest(new BaseResponseMessage(false, HttpStatusCode.BadRequest, result.Errors.ToString()));

            await _roleService.ApplyRoleForUserAsync(user, role);

            return Ok(new BaseResponseMessage(true, HttpStatusCode.NoContent, "Account created successfully."));
        }

        #endregion

        #region system role
        [Authorize(Roles = "ViewRoles")]
        [HttpGet("view-roles")]
        public async Task<ActionResult> ViewSystemRoles([FromQuery] bool showNoRole)
        {
            var roles = await _roleService.GetRolesSummaryAsync(showNoRole);

            var result = _mapper.Map<List<SystemRoleResponse>>(roles);

            return Ok(result);
        }

        [Authorize(Roles = "ViewRoleDetail")]
        [HttpGet("view-role-detail/{roleId}")]
        public async Task<ActionResult> ViewSystemRoleDetail(int roleId)
        {
            var role = await _roleService.GetRoleByRoleIdAsync(roleId);
            if (role == null)
                return NotFound(new BaseResponseMessage(false, HttpStatusCode.NotFound, "Role not exist"));

            var roles = _roleService.GetRoleSummary(role);

            var result = _mapper.Map<SystemRoleResponse>(roles);

            return Ok(result);
        }

        [Authorize(Roles = "ViewRolePermissions")]
        [HttpGet("view-role-permissions/{roleId}")]
        public async Task<ActionResult> ViewRolePermissions(int roleId)
        {
            var role = await _roleService.GetRoleByRoleIdAsync(roleId);
            if (role == null)
                return NotFound(new BaseResponseMessage(false, HttpStatusCode.NotFound, "Role not exist"));

            var permissions = await _roleManager.Roles.ToListAsync();

            var rolePermissions = await _roleService.GetRolePermissionsAsync(role);

            var result = _mapper.Map<List<SystemPermissionResponse>>(permissions);

            foreach (var item in result)
            {
                if (rolePermissions.Any(x => x.Id == item.Id))
                    item.IsAllowed = true;
                else
                    item.IsAllowed = false;
            }

            return Ok(result);
        }


        [Authorize(Roles = "CreateRole")]
        [HttpPost("create-role")]
        public async Task<ActionResult> CreateSystemRole(CreateRoleRequest createSystemRoleRequest)
        {
            if (await _roleService.GetRoleByRoleNameAsync(createSystemRoleRequest.RoleName) != null)
                return BadRequest(new BaseResponseMessage(false, HttpStatusCode.BadRequest, "Role already exist"));

            var role = new AppRole();

            role.RoleName = createSystemRoleRequest.RoleName;

            role.RolePermissions = new List<AppRolePermission>();

            var permissions = await _roleService.GetPermissionsByIdsAsync(createSystemRoleRequest.PermissionIds.Distinct().ToList());

            foreach (var permission in permissions)
            {
                var appRolePermission = new AppRolePermission();
                appRolePermission.AppRole = role;
                appRolePermission.AppPermission = permission;
                role.RolePermissions.Add(appRolePermission);
            }

            _unitOfWork.AppRoleRepository.Insert(role);

            if (await _unitOfWork.Complete())
                return Ok(new BaseResponseMessage(true, HttpStatusCode.OK, "Role created successfully"));

            return BadRequest(new BaseResponseMessage(true, HttpStatusCode.BadRequest, "Error when created role"));
        }

        [Authorize(Roles = "DeleteRole")]
        [HttpDelete("delete-role/{roleId}")]
        public async Task<ActionResult> DeleteSystemRole(int roleId)
        {
            var role = await _roleService.GetRoleByRoleIdAsync(roleId);
            if (role == null)
                return BadRequest("Role not exist");

            await _roleService.RemoveAllUsersInRoleAsync(role.Id);

            _unitOfWork.AppRoleRepository.Delete(role.Id);

            if (await _unitOfWork.Complete())
                return Ok(new BaseResponseMessage(true, HttpStatusCode.OK, $"Success fully delete role {role.RoleName}"));

            return BadRequest(new BaseResponseMessage(false, HttpStatusCode.BadRequest, $"Error when delete role"));
        }

        [Authorize(Roles = "UpdateRolePermissions")]
        [HttpPut("update-role-permissions/{roleId}")]
        public async Task<ActionResult> UpdateSystemRolePermissions(int roleId, UpdateRolePermissionRequest request)
        {
            var role = await _roleService.GetRoleByRoleIdAsync(roleId);
            if (role == null)
                return BadRequest("Role not exist");

            var rolePermissions = await _unitOfWork.AppRolePermissionRepository.GetAllBy(x => x.RoleId == roleId);

            var rolePermissionIds = rolePermissions.Select(x => x.PermissionId).ToList();
            
            if (request.PermissionIds == null || !request.PermissionIds.Any())
            {
                await RemoveAllPermissionsForRole(roleId, role, rolePermissionIds);
            }
            else
            {
                var permissionIds = request.PermissionIds.Distinct();

                var set = new HashSet<int>(rolePermissionIds);

                if (set.SetEquals(permissionIds))
                    return Ok(new BaseResponseMessage(true, HttpStatusCode.OK, $"Success fully update permission for role {role.RoleName}"));

                await AddPermissionsForRole(roleId, role, permissionIds, rolePermissionIds);

                await RemovePermissionsForRole(roleId, role, permissionIds, rolePermissionIds);
            }

            await _unitOfWork.Complete();
            
            return Ok(new BaseResponseMessage(true, HttpStatusCode.OK, $"Success fully update permission for role {role.RoleName}"));
            
        }
        #endregion

        #region system permission
        [Authorize(Roles = "ViewPermissions")]
        [HttpGet("view-permissions")]
        public async Task<ActionResult> ViewSystemPermissions()
        {
            var permissions = await _roleManager.Roles.OrderBy(x => x.PermissionGroup).ThenBy(x => x.Name).ToListAsync();
            var result = _mapper.Map<List<SystemPermissionResponse>>(permissions);
            return Ok(result);
        }


        [Authorize(Roles = "CreatePermission")]
        [HttpPost("create-permission")]
        public async Task<ActionResult> CreateSystemPermission(CreateSystemPermissionRequest createSystemPermissionRequest)
        {

            var permission = await _roleManager.FindByNameAsync(createSystemPermissionRequest.PermissionName);
            if (permission != null)
                return BadRequest("Permission already exist");

            var newPermission = new AppPermission();
            newPermission.Name = createSystemPermissionRequest.PermissionName;
            newPermission.PermissionGroup = createSystemPermissionRequest.PermissionGroup;

            var result = await _roleManager.CreateAsync(newPermission);

            if (result.Succeeded)
                return Ok(new BaseResponseMessage(true, HttpStatusCode.OK, $"Create successfully permission {createSystemPermissionRequest.PermissionName}"));

            return BadRequest(new BaseResponseMessage(false, HttpStatusCode.BadRequest, $"Error when create permission {createSystemPermissionRequest.PermissionName}"));

        }


        #endregion

        #region private method
        private async Task<bool> UserExist(string username)
        {
            return await _userManager.Users.AnyAsync(u => u.UserName == username.ToLower());
        }

        private async Task RemoveAllPermissionsForRole(int roleId, AppRole role, List<int> rolePermissionIds)
        {
            if(rolePermissionIds == null || !rolePermissionIds.Any())
                return;
            var deletedPermissions = await _roleManager.Roles.Where(x => rolePermissionIds.Contains(x.Id)).ToListAsync();
            if (deletedPermissions != null && deletedPermissions.Any())
            {
                _unitOfWork.AppRolePermissionRepository.Delete(x => x.RoleId == role.Id && deletedPermissions.Select(x => x.Id).Contains(x.PermissionId));
                await _roleService.RemovePermissionsForUsersAsync(roleId, deletedPermissions);
            }
        }

        private async Task RemovePermissionsForRole(int roleId, AppRole role, IEnumerable<int> permissionIds, List<int> rolePermissionIds)
        {
            var deletedPermissionIds = rolePermissionIds.Except(permissionIds);
            var deletedPermissions = await _roleManager.Roles.Where(x => deletedPermissionIds.Contains(x.Id)).ToListAsync();
            if (deletedPermissions != null && deletedPermissions.Any())
            {
                _unitOfWork.AppRolePermissionRepository.Delete(x => x.RoleId == role.Id && deletedPermissions.Select(x => x.Id).Contains(x.PermissionId));
                await _roleService.RemovePermissionsForUsersAsync(roleId, deletedPermissions);
            }
        }

        private async Task AddPermissionsForRole(int roleId, AppRole role, IEnumerable<int> permissionIds, List<int> rolePermissionIds)
        {
            var newPermissionIds = permissionIds.Except(rolePermissionIds);
            var newPermissions = await _roleManager.Roles.Where(x => newPermissionIds.Contains(x.Id)).ToListAsync();

            if (newPermissions != null && newPermissions.Any())
            {
                foreach (var permission in newPermissions)
                {
                    var newRolePermission = new AppRolePermission();
                    newRolePermission.PermissionId = permission.Id;
                    newRolePermission.RoleId = role.Id;
                    _unitOfWork.AppRolePermissionRepository.Insert(newRolePermission);
                }
                await _roleService.ApplyPermissionsForUsersAsync(roleId, newPermissions);
            }
        }

        #endregion


    }
}