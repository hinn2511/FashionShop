using System;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using API.Entities.User;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;

namespace API.Extensions
{
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddIdentityCore<AppUser>(opt =>
            {
                opt.Password.RequireNonAlphanumeric = false;
            })
                .AddDefaultTokenProviders()
                .AddRoles<AppRole>()
                .AddRoleManager<RoleManager<AppRole>>()
                .AddSignInManager<SignInManager<AppUser>>()
                .AddRoleValidator<RoleValidator<AppRole>>()
                .AddEntityFrameworkStores<DataContext>();

            services
                // (options =>
                // {
                //     // custom scheme defined in .AddPolicyScheme() below
                //     // options.DefaultScheme = "JWT_OR_COOKIE";
                //     // options.DefaultChallengeScheme = "JWT_OR_COOKIE";
                //     options.DefaultScheme = "Cookies";
                //     options.DefaultChallengeScheme = "Cookies";
                //     options.DefaultAuthenticateScheme = "Cookies";
                // })
                // .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)

                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddCookie(options =>
                {
                    options.LoginPath = "/login";
                    options.ExpireTimeSpan = TimeSpan.FromDays(1);
                })
                .AddJwtBearer("Bearer", options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetSection("AppSettings:Secret").Value)),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                    };
                })
                // .AddPolicyScheme("JWT_OR_COOKIE", "JWT_OR_COOKIE", options =>
                // {
                //     // runs on each request
                //     options.ForwardDefaultSelector = context =>
                //     {
                //         // filter by auth type
                //         string authorization = context.Request.Headers[HeaderNames.Authorization];
                //         if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
                //             return "Bearer";

                //         // otherwise always check for cookie auth
                //         return "Cookies";
                //     };
                // })
                ;

            services.AddAuthorization(opt =>
           {
               opt.AddPolicy("BusinessOnly", policy => policy.RequireAuthenticatedUser().RequireRole("Admin", "Manager"));
               opt.AddPolicy("AdminOnly", policy => policy.RequireAuthenticatedUser().RequireRole("Admin"));
               opt.AddPolicy("ManagerOnly", policy => policy.RequireAuthenticatedUser().RequireRole("Manager"));
               opt.AddPolicy("CustomerOnly", policy => policy.RequireAuthenticatedUser().RequireRole("Customer"));
           });

            return services;
        }
    }
}