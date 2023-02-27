using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.DTOs.Request.AuthenticationRequest;
using API.DTOs.Response;
using API.Entities;
using API.Entities.User;
using API.Extensions;
using API.Interfaces;
using API.Services;
using API.Services.UserService;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    public class AccountController : BaseApiController
    {
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        public AccountController(IRoleService roleService,UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService, IMapper mapper, IUserService userService)
        {
            _signInManager = signInManager;
            _roleService = roleService;
            _userManager = userManager;
            _mapper = mapper;
            _userService = userService;
            _tokenService = tokenService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterRequest registerRequest)
        {
            if (await UserExist(registerRequest.Username))
                return BadRequest(new BaseResponseMessage(false, HttpStatusCode.BadRequest, "Username already taken."));

            var user = _mapper.Map<AppUser>(registerRequest);

            user.UserName = registerRequest.Username.ToLower();

            var result = await _userManager.CreateAsync(user, registerRequest.Password);

            if (!result.Succeeded) 
                return BadRequest(new BaseResponseMessage(false, HttpStatusCode.BadRequest, "Can not register user."));

            var role = await _roleService.GetRoleByRoleNameAsync("Customer");

            if(role != null)
                await _roleService.ApplyRoleForUserAsync(user, role);

            return Ok(new BaseResponseMessage(true, HttpStatusCode.OK, "Register success."));
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<ActionResult> ClientAuthenticate(AuthenticationRequest authenticationRequest)
        {
            var response = await _userService.Authenticate(authenticationRequest, ipAddress());
            setTokenCookie(response.RefreshToken);
            return Ok(response);
        }


        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<ActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if(string.IsNullOrEmpty(refreshToken))
                return Unauthorized(); 
            var response = await _userService.RefreshToken(refreshToken, ipAddress());
            setTokenCookie(response.RefreshToken);
            return Ok(response);
        }

        [Authorize]
        [HttpPost("revoke-token")]
        public async Task<ActionResult> RevokeToken(RevokeTokenRequest model)
        {
            var token = !string.IsNullOrEmpty(model.Token) ? model.Token : Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
                return BadRequest(new { message = "Token is required" });

            await _userService.RevokeToken(token, ipAddress());

            return Ok(new { message = "Token revoked" });
        }
       
        [HttpPut("change-password")]
        public async Task<ActionResult> ChangePassword(ResetPasswordRequest resetPasswordRequest)
        {
            var user = await _userManager.Users
                    .SingleOrDefaultAsync(u => u.Id == GetUserId());

            var  checkPasswordResult = await _signInManager.CheckPasswordSignInAsync(user, resetPasswordRequest.OldPassword, false);
            if (!checkPasswordResult.Succeeded)
                return BadRequest("Password not valid!");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var resetPasswordResult = await _userManager.ResetPasswordAsync(user, token, resetPasswordRequest.NewPassword);

            if (!resetPasswordResult.Succeeded) 
                return BadRequest("Can not change password!");

            return Ok();

        }


        #region private method

        private async Task<bool> UserExist(string username)
        {
            return await _userManager.Users.AnyAsync(u => u.UserName == username.ToLower());
        }

        private void setTokenCookie(string token)
        {
            // var cookieOptions = new CookieOptions
            // {
            //     HttpOnly = true,
            //     Expires = DateTime.UtcNow.AddDays(7),
            //     Secure = true
            // };
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7),
                Secure = true
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

        private string ipAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }

        #endregion

    }
}