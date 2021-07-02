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
        public static Scope PlayerApi =
            new Scope(nameof(PlayerApi), "full");
        public static Scope ClientApi =
            new Scope(nameof(ClientApi), "full");


        public static IEnumerable<ApiResource> GetApis =>
                new List<ApiResource> {
                new ApiResource("TeamAPI","TeamAPI", new string[]
                        { 
                            UserClaims.SuperUser,
                        })
                    { 
                        Scopes = new List<Scope>{TeamApi}
                    },
                    new ApiResource("PlayerAPI","PlayerAPI", new string[]
                        {
                            UserClaims.SuperUser,
                        })
                    { Scopes=new List<Scope>{PlayerApi}
                    },
                    new ApiResource("ClientAPI","ClientAPI", new string[]
                        {
                            UserClaims.SuperUser,
                        })
                    { Scopes={
                            ClientApi,
                        }
                    }, new ApiResource("GWAPI","GWAPI", new string[]
                        {
                            UserClaims.SuperUser,
                        })
                };

       

        public static IEnumerable<Client> GetClients(string uri)
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


                    RedirectUris = {$"{uri}swagger/oauth2-redirect.html" },
                    AllowedCorsOrigins = {uri[0..^1] },
                    AllowedScopes = {
                        TeamApi.Name,
                        PlayerApi.Name,
                        ClientApi.Name,
                        StandardScopes.Profile}
                },
             
                new Client
                {
                    ClientId = "gw_api_postman_pw",
                    ClientSecrets = {new Secret("My very secret secret".Sha256())}, // change me!
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AllowedScopes = {
                        TeamApi.Name,
                        PlayerApi.Name,
                        ClientApi.Name,
                        StandardScopes.OpenId,
                        StandardScopes.Profile}
                }
            };

        }
    }
}
