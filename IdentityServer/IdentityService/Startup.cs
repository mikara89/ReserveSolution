using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using IdentityService.Data.Persistence;
using IdentityService.Messaging.Sender.Sender;
using IdentityService.Data.Models;
using Microsoft.AspNetCore.Identity;
using IdentityService.Messaging.Options;
using IdentityService.Data.Helpers;
using IdentityServer4.Services;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using IdentityService.Services;
using System;
using System.Net;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace IdentityService
{
    public class Startup
    {
        private readonly IWebHostEnvironment env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            this.env = env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            var appSettings = Configuration.GetSection(nameof(AppSettings)).Get<AppSettings>();
            services.AddDbContext<IdentityDBContext>(options =>
            {
                //options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                options.UseInMemoryDatabase("IdentityDB");
            });
            AddServiceAndSettings(services);
            services.AddDefaultIdentity<AppUser>(option =>
            {
                option.SignIn.RequireConfirmedEmail = true;
                option.SignIn.RequireConfirmedPhoneNumber = false;
            }).AddRoles<IdentityRole>()
              .AddDefaultTokenProviders()
              .AddEntityFrameworkStores<IdentityDBContext>();

            services.ConfigureApplicationCookie(config =>
            {
                config.Cookie.Name = "IdentityService.Cookie";
                config.LoginPath = "/Identity/Account/Login";
                config.LogoutPath = "/Identity/Account/Logout";
            });
            
            AddIdentityServer(services,appSettings);
            

            services.AddControllersWithViews();
            services.AddRazorPages();
        }

 
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
                              IWebHostEnvironment env,
                              ISeedUsersAndRols seedData)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //app.UseCors("AllowAll");
            app.UseStaticFiles();

            //if (env.IsDevelopment())
            //    app.UseHttpsRedirection();

            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseAuthorization();
            seedData.Start();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapRazorPages();
            });
        }

        private void AddIdentityServer(
            IServiceCollection services,
            AppSettings appSettings)
        {

            var issuer = Configuration["IDENTITY_AUTHORITY"];
            Console.WriteLine("IDENTITY_AUTHORITY: " + issuer);
            var GWUri = Configuration["GW_URI"];
            services.AddIdentityServer(c=>c.IssuerUri= issuer)
                //Add AspCore Identity
                .AddAspNetIdentity<AppUser>()
                //Mock data for testing
                .AddInMemoryApiResources(ID4Configuration.GetApis)
                .AddInMemoryClients(ID4Configuration.GetClients(GWUri))
                .AddInMemoryIdentityResources(ID4Configuration.GetIdentityResources())
                .AddDeveloperSigningCredential();
        }
        private void AddServiceAndSettings(IServiceCollection services)
        {
            services.Configure<AppSettings>(Configuration.GetSection(nameof(AppSettings)));
            services.Configure<RabbitMqConfiguration>(Configuration.GetSection("RabbitMq"));

            services.AddTransient<MailSendSender>();
            services.AddTransient<IEmailSender,MailSender>();
            services.AddTransient<ISeedUsersAndRols, SeedUsersAndRols>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IUserRepository, UserRepository>();
        }
    }
}
