using System;
using API.Extensions;
using API.Helpers.Authorization;
using API.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace API
{
    public class Startup
    {
        private readonly IConfiguration _config;
        public Startup(IConfiguration config)
        {
            _config = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationServices(_config);
            services.AddControllers();
            services.AddCors(options =>
            {
                options.AddPolicy("OpenCORSPolicy",
                policy =>
                {
                    policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200")
                        .SetPreflightMaxAge(TimeSpan.FromSeconds(2520));
                });
                
            }); 
            services.AddIdentityServices(_config);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseMiddleware<PreflightRequestMiddleware>();

            // app.UseMiddleware<ExceptionMiddleware>();

            app.UseMiddleware<ErrorHandlerMiddleware>();
            
            app.UseMiddleware<JwtMiddleware>();

            app.UseRouting();

            app.UseCors("OpenCORSPolicy");

            app.UseHttpsRedirection();

            app.UseHttpsRedirection(); 

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
