using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace WeCareAuthApp.API
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                 new IdentityResources.Profile()
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
                {
                    new ApiScope("apiscope1", "My API")
                };

        public static IEnumerable<Client> Clients =>
            new Client[]
                {
                    new Client()
                    {
                        ClientName = "testclient",                                                                                                                         
                        ClientId=   "testclientid",
                        AllowedGrantTypes= GrantTypes.ResourceOwnerPassword,
                        RedirectUris =
                        {
                            "https://localhost:5000/signin-oidc?code=AUTHORIZATION_CODE"
                        },
                        RequirePkce = false,
                        AllowedScopes =
                        {
                            IdentityServerConstants.StandardScopes.OpenId,
                            IdentityServerConstants.StandardScopes.Profile,
                            "apiscope1"
                        },
                        ClientSecrets =
                        {
                            new Secret("secret".Sha256())
                        }
                    }
                };
    }
}