using IdentityServer4.Validation;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer4API.Services
{
    public class TokenExchangeExtensionGrantValidator : IExtensionGrantValidator
    {
        public string GrantType => "urn:ietf:params:oauth:grant-type:token-exchange";
        private readonly ITokenValidator _tokenValidator;

        public TokenExchangeExtensionGrantValidator(ITokenValidator tokenValidator)
        {
            _tokenValidator = tokenValidator;
        }

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var requestRaw = context.Request.Raw.ToString();
            var token = context.Request.Raw.Get("subject_token");
            if (string.IsNullOrWhiteSpace(token))
            {
                context.Result = new GrantValidationResult(IdentityServer4.Models.TokenRequestErrors.InvalidRequest, "token missing.");
                return;
            }
            var tokenValidateResult = await _tokenValidator.ValidateAccessTokenAsync(token).ConfigureAwait(false);
            if(tokenValidateResult.IsError)
            {
                context.Result = new GrantValidationResult(IdentityServer4.Models.TokenRequestErrors.InvalidGrant, "token invalid.");
                return;
            }
            var subjectClaim = tokenValidateResult.Claims.FirstOrDefault(x => x.Type == "sub");
            if(subjectClaim == null) 
            {
                context.Result = new GrantValidationResult(IdentityServer4.Models.TokenRequestErrors.InvalidGrant, "token must contain subject id.");
                return;
            }
            context.Result = new GrantValidationResult(subjectClaim.Value, OpenIdConnectParameterNames.AccessToken, tokenValidateResult.Claims);
            return;
        }
    }
}
