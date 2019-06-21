using IdentityServer4.Models;
using System.Collections.Generic;

namespace IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }

        public static IEnumerable<ApiResource> GetApis()
        {
            var apiResource = new ApiResource[]
            {
                new ApiResource("demo_api_swagger", "demo_api_swagger"),
            };
            var secret = new List<Secret> { new Secret("acf2ec6fb01a4b698ba240c2b10a0243") };
            apiResource[0].ApiSecrets = secret;
            return apiResource;
        }

        public static IEnumerable<Client> GetClients()
        {
            return new[]
            {
                new Client
                {
                    ClientId = "demo_api_swagger",
                    ClientName = "MVC PKCE Client",
                    AllowedGrantTypes = GrantTypes.Code,
                    //ClientSecrets = {new Secret("acf2ec6fb01a4b698ba240c2b10a0243".Sha256())},
                    RedirectUris = {"http://localhost:5001/oauth2-redirect.html"},
                    AllowedScopes = {"openid", "profile", "demo_api_swagger","api1"},

                    RequirePkce = false,
                    AllowPlainTextPkce = false,
                    RequireClientSecret = false,
                    AllowedCorsOrigins =
                    {
                    "http://localhost:5001",
                    "http://localhost:5000"
                    },
                    AllowAccessTokensViaBrowser = true,
                    AlwaysIncludeUserClaimsInIdToken = true
                }
            };
        }
    }
}