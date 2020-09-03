using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Team.Api.Options;
using Team.Data.Persistence;

namespace Team.Api
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
            services.AddDbContext<TeamDBContext>(options =>
            {
                //options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                options.UseInMemoryDatabase("TeamDB");
            });
            services.AddAutoMapper(typeof(Startup));
            services.Configure<AppSettings>(Configuration.GetSection(nameof(AppSettings)));
            var appSettings = Configuration.GetSection(nameof(AppSettings)).Get<AppSettings>();
            services.AddSwaggerGen(confg =>
            {
                confg.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Team Api",
                    Description = "A simple API to add team",
                    Contact = new OpenApiContact
                    {
                        Name = "Slobodan",
                        Email = "smikaric@gmail.com"
                    }
                });
                var issuer = Configuration["AUTHORITY_ISSUER"];
                //oauth2 for log in 
                confg.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri($"{appSettings.AUTHORITY_ISSUER}connect/authorize"),
                            TokenUrl = new Uri($"{appSettings.AUTHORITY_ISSUER}connect/token"),
                            Scopes = new Dictionary<string, string>
                            {
                                {"TeamApi","full" }
                            }
                        }
                    }
                });
                //API key for sending mails
                confg.AddSecurityDefinition(ApiKeyAuthenticationOptions.DefaultScheme, new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Name = "X-API-KEY", //header with api key
                });
                confg.OperationFilter<AuthenticationRequirementsOperationFilter>();
            });

            services.AddAuthorization(cfg =>
            {
                ////Claim only for SuperUser
                //cfg.AddPolicy(ClaimPolicy.SuperUserClaimPolicy, p => p.RequireClaim(UserClaims.SuperUser, "True"));

                //cfg.AddPolicy(ClaimPolicy.TeamClaimPolicy, p => p
                //.RequireAssertion(context =>
                ////Claim for any users
                //context.User.HasClaim(c => c.Type == UserClaims.TeamApi) &&
                ////Claim for not banned users
                //context.User.HasClaim(c => c.Type == UserClaims.IsBanned && c.Value == "False") ||
                ////Claim for SuperUser
                //context.User.HasClaim(c => c.Type == UserClaims.SuperUser))
                //);
            });

            services.AddCors(cofing =>
            {
                cofing.AddPolicy("AllowAll", p =>
                p
                .AllowAnyMethod()
                .AllowAnyOrigin()
                .AllowAnyHeader());
            });

            var issuer = Configuration["IDENTITY_AUTHORITY"];
            Console.WriteLine("IDENTITY_AUTHORITY: " + issuer);
            // configure jwt authentication
            services.AddAuthentication("Bearer")
                .AddApiKeySupport(option => { })
                .AddIdentityServerAuthentication("Bearer", options =>
                {
                    // required audience of access tokens
                    options.ApiName = appSettings.AppName;
                    options.RequireHttpsMetadata = false;//===>CHANGE IN PRODATION
                    // auth server base endpoint (this will be used to search for disco doc)
                    options.Authority = issuer;
                }); ;

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseSwagger();

            app.UseCors("AllowAll");

            var appSettings = Configuration.GetSection(nameof(AppSettings)).Get<AppSettings>();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Team Api");
                c.OAuthClientId(appSettings.ClientId);
                c.OAuthAppName(appSettings.AppName);
                c.OAuth2RedirectUrl($"{appSettings.RedirectSwagger}oauth2-redirect.html"); 
                //c.OAuth2RedirectUrl($"http://localhost:5111/swagger/oauth2-redirect.html"); 
                c.OAuthUsePkce();
            });

            app.UseAuthorization();
            app.UseAuthentication();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
