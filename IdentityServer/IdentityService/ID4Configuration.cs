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



        public static Scope TeamApi = 
            new Scope(nameof(TeamApi),"full" );
        public static Scope TeamPlayerApi =
            new Scope(nameof(TeamPlayerApi), "full");
        public static Scope TeamClientApi =
            new Scope(nameof(TeamClientApi), "full");


        public static IEnumerable<ApiResource> GetApis =>
                new List<ApiResource> {
                new ApiResource("TeamAPI","TeamAPI", new string[]
                        { 
                            UserClaims.SuperUser,
                        })
                    { 
                        Scopes = new List<Scope>{TeamApi}
                    },
                    new ApiResource("TeamPlayerAPI","TeamPlayerAPI", new string[]
                        {
                            UserClaims.SuperUser,
                        })
                    { Scopes=new List<Scope>{TeamPlayerApi}
                    },
                    new ApiResource("TeamClientAPI","TeamClientAPI", new string[]
                        {
                            UserClaims.SuperUser,
                        })
                    { Scopes={
                            TeamClientApi,
                        }
                    }, new ApiResource("GWAPI","GWAPI", new string[]
                        {
                            UserClaims.SuperUser,
                        })
                };

       

        public static IEnumerable<Client> GetClients(AppSettings appSettings)
        {
            return new List<Client> {
                new Client
                {
                    ClientId = "gw_api_swagger",
                    ClientName = "GWApi",
                    ClientSecrets = {new Secret("My very secret secret".Sha256())}, // change me!

                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = false,
                    RequireConsent=false,


                    RedirectUris = {$"{appSettings.GWTeamUrl}swagger/oauth2-redirect.html" },
                    AllowedCorsOrigins = {appSettings.GWTeamUrl[0..^1] },
                    AllowedScopes = {
                        TeamApi.Name,
                        TeamPlayerApi.Name,
                        TeamClientApi.Name,
                        StandardScopes.Profile}
                },
             
                new Client
                {
                    ClientId = "gw_api_postman_pw",
                    ClientSecrets = {new Secret("My very secret secret".Sha256())}, // change me!
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AllowedScopes = {
                        TeamApi.Name,
                        TeamPlayerApi.Name,
                        TeamClientApi.Name,
                        StandardScopes.OpenId,
                        StandardScopes.Profile}
                }
            };

        }
    }
}
