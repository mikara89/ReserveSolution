using System;
using GateWayAPI.Web.Options;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace GateWayAPI.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;

            
            services.Configure<AppIdSwagger>(Configuration.GetSection(nameof(AppIdSwagger)));
            var appIdSwagger = Configuration.GetSection(nameof(AppIdSwagger)).Get<AppIdSwagger>();
            services.AddOcelot(Configuration);
            services.AddSwaggerForOcelot(Configuration);

            var issuer = Configuration["IDENTITY_AUTHORITY"];
            Console.WriteLine("IDENTITY_AUTHORITY: " + issuer);


            for (int i = 0; i < appIdSwagger.AuthKeys.Length; i++)
            {
                var key = appIdSwagger.AuthKeys[i];
                var apiName = appIdSwagger.ApiNames[i];
                services.AddAuthentication()
                    .AddIdentityServerAuthentication(key, o =>
                    {
                        o.ApiName = apiName;
                        o.RequireHttpsMetadata = false;//===>CHANGE IN PRODATION
                        o.SupportedTokens = SupportedTokens.Both;
                        o.Authority = issuer;
                    });
            }
                
           
            services.AddControllers();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async  void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            
            app.UseRouting();

            var appIdSwagger = Configuration.GetSection(nameof(AppIdSwagger)).Get<AppIdSwagger>();
            var url = Configuration["url"];
            app.UseSwaggerForOcelotUI(confg=> {
                confg.OAuthUsePkce();
                confg.OAuthClientId(appIdSwagger.ClientId);
                confg.OAuthAppName(appIdSwagger.AppName);
                confg.OAuth2RedirectUrl($"{url}swagger/oauth2-redirect.html");
                
            });

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
   

            await app.UseOcelot();
        }
    }
}
