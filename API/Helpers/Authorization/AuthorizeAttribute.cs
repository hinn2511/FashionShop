using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace API.Helpers.Authorization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var allowedRoles = new List<string>();

            var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
            if (allowAnonymous)
                return;

            var allowRoles = context.ActionDescriptor.EndpointMetadata.OfType<RoleAttribute>();
            if (allowRoles.Any()) 
            {
                foreach(var allowRole in allowRoles)
                    allowedRoles.Add(allowRole.NormalizeRoleName);
            }

            var task = (Task<AppUser>) context.HttpContext.Items["User"];
            
            var user = task?.Result;                

            if (user != null)
            {
                if (!allowRoles.Any())
                    return;

                var dbContext = context.HttpContext.RequestServices.GetRequiredService<DataContext>();
                
                var userRoleIds = dbContext.UserRoles.Where(x => x.UserId == user.Id).Select(x => x.RoleId);

                var userRoles = dbContext.Roles.Where(x => userRoleIds.Contains(x.Id));

                foreach(var userRole in userRoles) {
                    if (allowedRoles.Contains(userRole.NormalizedName)) 
                        return;
                }

                context.Result = new JsonResult(new { message = "Forbidden" }) { StatusCode = StatusCodes.Status403Forbidden };
            }
            else
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
        }
    }
}