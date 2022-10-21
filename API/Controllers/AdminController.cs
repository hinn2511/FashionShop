using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.Params;
using API.DTOs.Request.AdminRequest;
using API.DTOs.Request.ConfigurationRequest;
using API.DTOs.Response.AdminResponse;
using API.Entities;
using API.Entities.User;
using API.Entities.WebPageModel;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class AdminController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AdminController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager,IUnitOfWork unitOfWork, IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet("users-roles")]
        public async Task<ActionResult> GetUsersWithRoles([FromQuery] UserRoleParams userRoleParams)
        {
            var systemUsers = _userManager.Users.AsQueryable();

            systemUsers = systemUsers.Include(x => x.UserRoles).ThenInclude(x => x.Role);

            if(userRoleParams.ExcludeRoles.Any())
                systemUsers = systemUsers.Where(x => !x.UserRoles.Select(x => x.Role.Name)
                                                .Any(name => userRoleParams.ExcludeRoles.Any(excludeName => name == excludeName)));

            //users = users.Where(x => x.Status != Status.Hidden && x.Status != Status.Deleted);

            if(!string.IsNullOrEmpty(userRoleParams.Query))
            {   
                var words = userRoleParams.Query.Replace("@", " ").RemoveSpecialCharacters().ToUpper().Split(" ").Distinct();
                foreach(var word in words)
                {
                    systemUsers = systemUsers.Where(x => x.UserName.ToUpper().Contains(word) 
                                                        || x.FirstName.ToUpper().Contains(word)
                                                        || x.LastName.ToUpper().Contains(word)
                                                        || x.Email.ToUpper().Contains(word)
                                                    );
                }
            }


            if (userRoleParams.OrderBy == OrderBy.Ascending) 
            {
                systemUsers = userRoleParams.Field switch
                {
                    "FirstName" => systemUsers.OrderBy(p => p.FirstName),
                    "LastName" => systemUsers.OrderBy(p => p.LastName),
                    "Username" => systemUsers.OrderBy(p => p.UserName),
                    _ => systemUsers.OrderBy(p => p.Id)
                };
            }
            else
            {
                systemUsers = userRoleParams.Field switch
                {
                    "FirstName" => systemUsers.OrderByDescending(p => p.FirstName),
                    "LastName" => systemUsers.OrderByDescending(p => p.LastName),
                    "Username" => systemUsers.OrderByDescending(p => p.UserName),
                    _ => systemUsers.OrderByDescending(p => p.Id)
                };
            }

            

            var users = await PagedList<AppUser>.CreateAsync(systemUsers.AsNoTracking(), userRoleParams.PageNumber, userRoleParams.PageSize);

            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

            var result = _mapper.Map<List<UserRoleResponse>>(users.ToList());

            return Ok(result);

        }

        [HttpPut("edit-user-roles/{username}")]
        public async Task<ActionResult> EditRoles(string username, [FromQuery] string roles)
        {
            var selectedRoles = roles.Split(",").ToArray();

            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
                return NotFound("User not found.");

            var userRoles = await _userManager.GetRolesAsync(user);

            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

            if (!result.Succeeded)
                return BadRequest("Can not add role for user.");

            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

            if (!result.Succeeded)
                return BadRequest("Can not remove role from user.");

            return Ok(await _userManager.GetRolesAsync(user));

        }

        [HttpPost("create-system-account")]
        public async Task<ActionResult> Register(CreateSystemAccountRequest createSystemAccountRequest)
        {
            if (await UserExist(createSystemAccountRequest.Username))
                return BadRequest("Username already taken");

            var user = _mapper.Map<AppUser>(createSystemAccountRequest);

            user.UserName = createSystemAccountRequest.Username.ToLower();

            var result = await _userManager.CreateAsync(user, createSystemAccountRequest.Password);

            if (!result.Succeeded) 
                return BadRequest(result.Errors);

            var roleResult = await _userManager.AddToRolesAsync(user, createSystemAccountRequest.Roles.Split(","));

            if(!roleResult.Succeeded) 
                return BadRequest(result.Errors);

            return Ok();
        }

         private async Task<bool> UserExist(string username)
        {
            return await _userManager.Users.AnyAsync(u => u.UserName == username.ToLower());
        }


        
    }
}