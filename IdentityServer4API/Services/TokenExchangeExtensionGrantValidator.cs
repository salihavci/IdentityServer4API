using IdentityModel;
using IdentityServer4.Validation;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer4API.Services
{
    public class TokenExchangeExtensionGrantValidator : IExtensionGrantValidator
    {
        private readonly ITokenValidator _tokenValidator;
        public TokenExchangeExtensionGrantValidator(ITokenValidator tokenValidator)
        {
            _tokenValidator = tokenValidator;
        }

        public string GrantType => "urn:ietf:params:oauth:grant-type:token-exchange";

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var requestRaw = context.Request.Raw.ToString();
            var token = context.Request.Raw.Get("subject_token");
            if(string.IsNullOrWhiteSpace(token))
            {
                context.Result = new GrantValidationResult(IdentityServer4.Models.TokenRequestErrors.InvalidRequest, "Token missing");
                return;
            }
            var tokenValidateResult = await _tokenValidator.ValidateAccessTokenAsync(token).ConfigureAwait(false);
            if(tokenValidateResult.IsError)
            {
                context.Result = new GrantValidationResult(IdentityServer4.Models.TokenRequestErrors.InvalidGrant, "Token invalid");
                return;
            }
            var subjectClaim = tokenValidateResult.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Subject);
            if(subjectClaim == null) 
            {
                context.Result = new GrantValidationResult(IdentityServer4.Models.TokenRequestErrors.InvalidGrant, "Token must contain subject value.");
                return;
            }
            context.Result = new GrantValidationResult(subjectClaim.Value, "access_token", tokenValidateResult.Claims);
            return;

        }
    }
}
