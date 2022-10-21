using System;
using System.Threading.Tasks;
using API.Entities.User;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();
            if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return;
            var userId = resultContext.HttpContext.User.GetUserId();
            //var uow = resultContext.HttpContext.RequestServices.GetService<IUnitOfWork>();
            var userManager = resultContext.HttpContext.RequestServices.GetService<UserManager<AppUser>>();
            var user = await userManager.FindByIdAsync(userId.ToString());
            user.LastActive = DateTime.UtcNow;
            await userManager.UpdateAsync(user);
            //await uow.Complete();
        }
    }
}