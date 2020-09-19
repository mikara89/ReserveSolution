using System;
using System.Collections.Generic;
using System.Reflection;
using AutoMapper;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Player.Api.Filters;
using Player.Api.Options;
using Player.Data.Persistence;
using Player.Domains.Models;
using Player.Messanger.Sender;
using Player.Messanger.Sender.Options;
using Player.Service;
using Player.Service.Commands;
using Player.Service.Hendlers;
using Player.Service.Queries;
using Player.Service.Repository;

namespace Player.Api
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
            services.AddDbContext<PlayerDBContext>(options =>
            {
                //options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                options.UseInMemoryDatabase("PlayerDB");
            });

            services.AddTransient<SeedInvitations>();

            services.AddAutoMapper(typeof(Startup));

            #region EventEmitter
            services.AddTransient<IPlayerEventEmitter, PlayerEventEmitter>();
            #endregion

            #region Repositories
            services.AddTransient<IPlayerRepository, PlayerRepository>();
            services.AddTransient<IInvetationRepository, InvetationRepository>();
            #endregion

            #region MediatoR
            services.AddMediatR(typeof(Startup));
            services.AddTransient<IRequestHandler<GetAllPlayersQuery, List<PlayerDto>>, GetAllPlayersHandler>();
            services.AddTransient<IRequestHandler<GetPlayerByIdQuery, PlayerDto>, GetPlayerByIdHandler>();
            services.AddTransient<IRequestHandler<PlayerCreateCommand, PlayerDto>, PlayerCreateHandler>();
            services.AddTransient<IRequestHandler<PlayerUpdateCommand, PlayerDto>, PlayerUpdateHandler>();
            services.AddTransient<IRequestHandler<PlayerDeleteCommand, PlayerDto>, PlayerDeleteHandler>();
            #endregion

            #region SettingsParamiters
            services.Configure<RabbitMqSettings>(Configuration.GetSection(nameof(RabbitMqSettings)));
            services.Configure<AppSettings>(Configuration.GetSection(nameof(AppSettings)));
            #endregion

            var appSettings = Configuration.GetSection(nameof(AppSettings)).Get<AppSettings>();
            var rabbitMqSettings = Configuration.GetSection(nameof(RabbitMqSettings)).Get<RabbitMqSettings>();

            services.AddSwaggerGen(confg =>
            {
                confg.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Team Player Api",
                    Description = "A simple API to add player",
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
                                {"PlayerApi","full" }
                            }
                        }
                    }
                });
                confg.OperationFilter<AuthenticationRequirementsOperationFilter>();
            });

            var issuer = Configuration["IDENTITY_AUTHORITY"];
            Console.WriteLine("IDENTITY_AUTHORITY: " + issuer);
            // configure token authentication
            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication("Bearer", options =>
                {
                    // required audience of access tokens
                    options.ApiName = appSettings.AppName;
                    options.RequireHttpsMetadata = false;//===>CHANGE IN PRODATION
                    // auth server base endpoint (this will be used to search for disco doc)
                    options.Authority = issuer;
                }); ;


            services.AddCors(cofing =>
            {
                cofing.AddPolicy("AllowAll", p =>
                p
                .AllowAnyMethod()
                .AllowAnyOrigin()
                .AllowAnyHeader());
            });

            services.AddControllers(
                options =>
                {
                    options.Filters.Add<ValidationFilter>();
                })
                .AddFluentValidation(opt =>
                {
                    opt.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
                })
                ;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, SeedInvitations seed)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseSwagger();

            app.UseCors("AllowAll");

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Player Api");
            });

            app.UseAuthorization();
            app.UseAuthentication();
            seed.Seed();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
