using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using System.Security.Cryptography.Xml;

namespace Mango.Services.Identity
{
    public static class SD
    {
        public const string Admin = "Admin";
        public const string Customer = "Customer";

        public static IEnumerable<IdentityResource> IdentityResources =>
            new List<IdentityResource>() {
                new IdentityResources.OpenId(),
                new IdentityResources.Email(),
                new IdentityResources.Profile()
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>() { new ApiScope("mango", "Mango Server")
                ,new ApiScope(name: "read", displayName: "Read your data")
                ,new ApiScope(name: "write", displayName: "Write your data")
                ,new ApiScope(name: "delete", displayName: "Delete your data")
            };

        public static IEnumerable<Client> Clients =>
            new List<Client>() {
                new Client(){
                    ClientId = "client",
                    ClientSecrets = { new Secret("secretkey".Sha256())},
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = {"read", "write", "profile" }  //<== "profile" is the system built-in scope does not have to be created above!
                },
                new Client(){
                    ClientId = "mango",
                    ClientSecrets = { new Secret("secretkey".Sha256())},
                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris = { "http://localhost:44395/signin-oidc" },
                    PostLogoutRedirectUris = { "http://localhost:44395/signout-callback-oidc" },
                    AllowedScopes = new List<string>(){ 
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.Profile,
                        "mango"
                    } //<== "profile" is the system built-in scope does not have to be created above!
                }};
    }
}
