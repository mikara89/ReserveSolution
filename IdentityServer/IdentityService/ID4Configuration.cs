using IdentityModel;
using IdentityServer4.Models;
using IdentityService.Data.Helpers;
using IdentityService.Data.Models;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System.Collections.Generic;
using System.Security.Claims;
using static IdentityServer4.IdentityServerConstants;

namespace IdentityService
{
    public static class ID4Configuration
    {
        public static IEnumerable<IdentityResource> GetIdentityResources() =>
           new List<IdentityResource>
           {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
           };


        public static IEnumerable<ApiResource> GetApis =>
                new List<ApiResource> {
                new ApiResource("TeamAPI","TeamAPI", new string[]
                    { 
                        UserClaims.User, 
                        UserClaims.IsBanned,
                        UserClaims.SuperUser,
                        ClaimTypes.NameIdentifier
                    }
                )
                { Scopes={
                        new Scope("TeamApi","full" ),
                    } },
                };

        public static IEnumerable<Client> GetClients(AppSettings appSettings)
        {
            return new List<Client> {
                new Client
                {
                    ClientId = "team_api_swagger",
                    ClientName = "TeamApi",
                    ClientSecrets = {new Secret("My very secret secret".Sha256())}, // change me!

                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = false,
                    RequireConsent=false,
                    

                    RedirectUris = {$"{appSettings.GWTeamUrl}swagger/oauth2-redirect.html" }, 
                    AllowedCorsOrigins = {appSettings.GWTeamUrl[0..^1] },
                     //RedirectUris = {$"http://localhost:5111/swagger/oauth2-redirect.html" },
                    //AllowedCorsOrigins = {"http://localhost:5111" },
                    AllowedScopes = {
                        "TeamApi",
                        StandardScopes.Profile}
                }
            };
        }
            

       
    }
}
