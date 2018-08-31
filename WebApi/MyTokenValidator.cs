using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace WebApi
{
    public class MyTokenValidator : ISecurityTokenValidator
    {
        bool ISecurityTokenValidator.CanValidateToken => true;

        int ISecurityTokenValidator.MaximumTokenSizeInBytes { get; set; }

        bool ISecurityTokenValidator.CanReadToken(string securityToken)
        {
            return true;
        }

        //验证token
        ClaimsPrincipal ISecurityTokenValidator.ValidateToken(string securityToken, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            validatedToken = null;
            //判断token是否正确
            if (securityToken != "abcdefg")
                return null;

            //给Identity赋值
            var identity = new ClaimsIdentity(JwtBearerDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim("name", "wyt"));
            identity.AddClaim(new Claim(ClaimsIdentity.DefaultRoleClaimType, "admin"));

            var principle = new ClaimsPrincipal(identity);
            return principle;
        }
    }
}
