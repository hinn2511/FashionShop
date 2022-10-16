using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.DTOs.Request.AuthenticationRequest;
using API.Entities;
using API.Entities.User;
using API.Interfaces;
using API.Services.UserService;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    
    public class AccountController : BaseApiController
    {
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService, IMapper mapper, IUserService userService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _mapper = mapper;
            _userService = userService;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await UserExist(registerDto.Username))
                return BadRequest("Username already taken");

            var user = _mapper.Map<AppUser>(registerDto);

            user.UserName = registerDto.Username.ToLower();

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded) 
                return BadRequest(result.Errors);

            var roleResult = await _userManager.AddToRoleAsync(user, "Customer");

            if(!roleResult.Succeeded) 
                return BadRequest(result.Errors);

            return Ok();
        }

        
        [HttpPost("authenticate")]
        public async Task<ActionResult> Authenticate(LoginDto model)
        {
            var response = await _userService.Authenticate(model, ipAddress());
            setTokenCookie(response.RefreshToken);
            return Ok(response);
        }

        [Authorize]
        [HttpPost("refresh-token")]
        public async Task<ActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var response = await _userService.RefreshToken(refreshToken, ipAddress());
            setTokenCookie(response.RefreshToken);
            return Ok(response);
        }

        [Authorize]
        [HttpPost("revoke-token")]
        public async Task<ActionResult> RevokeToken(RevokeTokenRequest model)
        {
            var token = model.Token != "" ? model.Token : Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
                return BadRequest(new { message = "Token is required" });

            await _userService.RevokeToken(token, ipAddress());
            return Ok(new { message = "Token revoked" });
        }

        [Authorize]
        [HttpGet("{id}/refresh-tokens")]
        public async Task<IActionResult> GetRefreshTokens(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            return Ok(user.RefreshTokens);
        }

        [HttpPut("change-password")]
        public async Task<ActionResult<UserDto>> ChangePassword(ResetPasswordDto resetPasswordDto)
        {
            var user = await _userManager.Users
                    .SingleOrDefaultAsync(u => u.UserName == resetPasswordDto.Username.ToLower());

            var  checkPasswordResult = await _signInManager.CheckPasswordSignInAsync(user, resetPasswordDto.OldPassword, false);
            if (!checkPasswordResult.Succeeded)
                return BadRequest("Password not valid!");

            var resetPasswordResult = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);

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
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
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