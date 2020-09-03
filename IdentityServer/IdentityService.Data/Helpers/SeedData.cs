using IdentityService.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityService.Data.Helpers
{
    public class SeedUsersAndRols : ISeedUsersAndRols
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<AppUser> userManager;
        private readonly ILogger<SeedUsersAndRols> logger;

        public SeedUsersAndRols(
            RoleManager<IdentityRole> roleManager,
            UserManager<AppUser> userManager,
            ILogger<SeedUsersAndRols> logger)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.logger = logger;
        }

        public void Start()
        {
            AddRoles(roleManager, logger);
            AddAdmin(userManager, logger);
            SetAdmin(userManager, roleManager, logger);
        }
        private void AddRoles(RoleManager<IdentityRole> roleManager, ILogger<object> logger)
        {
            var roles = new List<IdentityRole> {
                new IdentityRole
                {
                     Name="User"
                },
                new IdentityRole
                {
                     Name="Admin"
                },
            };
            roles.ForEach(r =>
            {
                var result = roleManager.CreateAsync(r).Result;
                if (result.Succeeded)
                    logger.LogInformation($"Role {r.Name} added");
                else
                {
                    logger.LogWarning($"Role {r.Name} not added");
                    result.Errors.ToList().ForEach(e =>
                        logger.LogWarning("error:" + e.Description)
                    );

                }
            });

        }

        private void AddAdmin(UserManager<AppUser> appUserManager, ILogger<object> logger)
        {
            var pass = "mikara@89Team";
            var users = new List<AppUser> {
                new AppUser
                {
                     Id="f7882153-2077-436d-94b0-822bc6b89dc4",
                     Email="smikaric@team.com",
                     UserName="smikaric@team.com",
                     IsActive=true,
                     EmailConfirmed=true,
                },
            };
            users.ForEach(u =>
            {
                var result = appUserManager.CreateAsync(u, pass).Result;
                if (result.Succeeded)
                    logger.LogInformation($"Role {u.Email} added");
                else
                {
                    logger.LogWarning($"Role {u.Email} not added");
                    result.Errors.ToList().ForEach(e =>
                        logger.LogWarning("error:" + e.Description)
                    );
                }
            });
        }
        private void SetAdmin(UserManager<AppUser> appUserManager, RoleManager<IdentityRole> roleManager, ILogger<object> logger)
        {
            var user = appUserManager.FindByNameAsync("smikaric@team.com").Result;
            if (user != null)
            {
                var roleAdmin = roleManager.FindByNameAsync(UserRole.Admin).Result;
                if (roleAdmin != null)
                {
                    var result = appUserManager.AddToRoleAsync(user, roleAdmin.Name).Result;


                    if (!appUserManager.GetClaimsAsync(user).Result.Any(x => x.Type == UserClaims.SuperUser))
                        appUserManager.AddClaimAsync(user, new System.Security.Claims.Claim(UserClaims.SuperUser, "True")).Wait();

                    if (!appUserManager.GetClaimsAsync(user).Result.Any(x => x.Type == UserClaims.User))
                        appUserManager.AddClaimAsync(user, new System.Security.Claims.Claim(UserClaims.User, "True")).Wait();

                    if (result.Succeeded)
                        logger.LogInformation("Role Admin assignet to " + user.UserName);
                    else
                    {
                        logger.LogWarning($"Role Admin faild asssignt to " + user.UserName);
                        result.Errors.ToList().ForEach(e =>
                            logger.LogWarning("error:" + e.Description)
                        );
                    }
                }

            }
        }
    }
}
