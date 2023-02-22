using IdentityServer4.Models;
using IdentityServer4;
using System.Collections.Generic;
using System;

namespace IdentityServer4API
{
    public static class Config
    {
        public static IEnumerable<ApiResource> ApiResources => new ApiResource[]
        {
            new ApiResource("catalog_resource"){Scopes = {"catalog_fullpermission"} },
            new ApiResource("photostock_resource"){Scopes = {"photostock_fullpermission"} },
            new ApiResource("basket_resource"){Scopes = {"basket_fullpermission"} },
            new ApiResource("discount_resource"){Scopes = {"discount_fullpermission"} },
            new ApiResource("order_resource"){Scopes = {"order_fullpermission"} },
            new ApiResource("payment_resource"){Scopes = {"payment_fullpermission"} },
            new ApiResource(IdentityServerConstants.LocalApi.ScopeName)
        };
        public static IEnumerable<IdentityResource> IdentityResources => new IdentityResource[]
        {
            new IdentityResources.Email(),
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResource() { Name = "roles", DisplayName = "Roles", Description = "Kullanıcı rolleri", UserClaims = new []{"role"} }
        };
        public static IEnumerable<ApiScope> ApiScopes => new ApiScope[]
        {
        new ApiScope("catalog_fullpermission","Catalog API'si için full erişim"),
        new ApiScope("photostock_fullpermission","Photostock API'si için full erişim"),
        new ApiScope("basket_fullpermission","Basket(Sepet) API'si için full erişim"),
        new ApiScope("discount_fullpermission","Discount(İndirim) API'si için full erişim"),
        new ApiScope("order_fullpermission","Order(Sipariş) API'si için full erişim"),
        new ApiScope("payment_fullpermission","Payment(Ödeme) API'si için full erişim"),
        new ApiScope(IdentityServerConstants.LocalApi.ScopeName)
        };
        public static IEnumerable<Client> Clients => new Client[]
        {
            new Client()
            {
                ClientId = "WebMvcClient",
                ClientSecrets = {new Secret("secret".Sha256()) },
                ClientName = "Asp.Net Core MVC",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                AllowedScopes = {
                    "catalog_fullpermission",
                    "photostock_fullpermission",
                    IdentityServerConstants.LocalApi.ScopeName
                }
            },
            new Client()
            {
                ClientId = "WebMvcClientForUser",
                ClientSecrets = {new Secret("secret".Sha256()) },
                ClientName = "Asp.Net Core MVC",
                AllowOfflineAccess = true,
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                AllowedScopes = {
                    "basket_fullpermission",
                    "discount_fullpermission",
                    "order_fullpermission",
                    "payment_fullpermission",
                    IdentityServerConstants.StandardScopes.Email,
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.OfflineAccess,
                    "roles",
                    IdentityServerConstants.LocalApi.ScopeName
                },
                AccessTokenLifetime = 3600,
                RefreshTokenExpiration = TokenExpiration.Absolute,
                AbsoluteRefreshTokenLifetime = (int)(DateTime.Now.AddDays(60) - DateTime.Now).TotalSeconds,
                RefreshTokenUsage = TokenUsage.ReUse
            }
        };

    }
}
