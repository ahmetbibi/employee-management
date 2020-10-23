using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeManagement
{
    public class Startup
    {
        private IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextPool<AppDbContext>(options => options.UseSqlServer(_config.GetConnectionString("EmployeeDBConnection")));
            
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    options.Password.RequiredUniqueChars = 3;
                    options.Password.RequiredLength = 10;
                }).AddEntityFrameworkStores<AppDbContext>();

            //services.Configure<IdentityOptions>(options =>
            //{
            //    options.Password.RequiredUniqueChars = 3;
            //    options.Password.RequiredLength = 10;
            //});

            services.AddMvc(options => {
                var policy = new AuthorizationPolicyBuilder()
                                .RequireAuthenticatedUser()
                                .Build();

                options.Filters.Add(new AuthorizeFilter(policy));
            }).AddXmlSerializerFormatters();
            // Setting the current repository to use
            //services.AddSingleton<IEmployeeRepository, MockEmployeeRepository>();
            services.AddScoped<IEmployeeRepository, SQLEmployeeRepository>(); // To use every http request to a different controller, we use AddScoped pattern
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            } 
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
            }
               
            //app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });

            //app.UseMvcWithDefaultRoute();

            //app.UseMvc();

            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});
        }
    }
}
