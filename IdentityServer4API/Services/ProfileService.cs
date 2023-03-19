using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServer4API.Services
{
    public class ProfileService<TUser> : IProfileService where TUser : class
    {
        protected readonly IUserClaimsPrincipalFactory<TUser> ClaimsFactory;
        protected readonly ILogger<ProfileService<TUser>> Logger;
        private readonly UserManager<TUser> _userManager;

        public ProfileService(UserManager<TUser> userManager, IUserClaimsPrincipalFactory<TUser> claimsFactory)
        {
            _userManager = userManager;
            ClaimsFactory = claimsFactory;
        }

        public ProfileService(IUserClaimsPrincipalFactory<TUser> claimsFactory, ILogger<ProfileService<TUser>> logger, UserManager<TUser> userManager)
        {
            ClaimsFactory = claimsFactory;
            Logger = logger;
            _userManager = userManager;
        }

        public virtual async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var sub = context.Subject?.GetSubjectId();
            if (sub == null) throw new Exception("No sub claim present");

            await GetProfileDataAsync(context, sub);
        }

        protected virtual async Task GetProfileDataAsync(ProfileDataRequestContext context, string subjectId)
        {
            var user = await FindUserAsync(subjectId);
            if (user != null)
            {
                await GetProfileDataAsync(context, user);
            }
        }

        protected virtual async Task GetProfileDataAsync(ProfileDataRequestContext context, TUser user)
        {
            var principal = await GetUserClaimsAsync(user);
            context.AddRequestedClaims(principal.Claims);
            var userInfo = user as AppUser;
            var additionalClaims = new List<Claim>()
                {
                    new Claim(JwtClaimTypes.Name, userInfo.UserName, ClaimValueTypes.String),
                    new Claim("city",userInfo.City, ClaimValueTypes.String)
                };
            context.IssuedClaims.AddRange(additionalClaims);
        }

        protected virtual async Task<ClaimsPrincipal> GetUserClaimsAsync(TUser user)
        {
            var principal = await ClaimsFactory.CreateAsync(user);
            if (principal == null) throw new Exception("ClaimsFactory failed to create a principal");

            return principal;
        }

        public virtual async Task IsActiveAsync(IsActiveContext context)
        {
            var sub = context.Subject?.GetSubjectId();
            if (sub == null) throw new Exception("No subject Id claim present");

            await IsActiveAsync(context, sub);
        }

        protected virtual async Task IsActiveAsync(IsActiveContext context, string subjectId)
        {
            var user = await FindUserAsync(subjectId);
            if (user != null)
            {
                await IsActiveAsync(context, user);
            }
            else
            {
                context.IsActive = false;
            }
        }

        protected virtual async Task IsActiveAsync(IsActiveContext context, TUser user)
        {
            context.IsActive = await IsUserActiveAsync(user);
        }

        public virtual Task<bool> IsUserActiveAsync(TUser user)
        {
            return Task.FromResult(true);
        }

        protected virtual async Task<TUser> FindUserAsync(string subjectId)
        {
            var user = await _userManager.FindByIdAsync(subjectId);
            if (user == null)
            {
                Logger?.LogWarning("No user found matching subject Id: {subjectId}", subjectId);
            }

            return user;
        }
    }
}
