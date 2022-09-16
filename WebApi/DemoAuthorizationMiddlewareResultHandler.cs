using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

public class DemoAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
{
    public DemoAuthorizationMiddlewareResultHandler()
    {
    }

    public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
    {
        try
        {
            // it will be get dynamically...
            string authority = "https://localhost:5001";

            var token = context.Request.Headers[HeaderNames.Authorization].FirstOrDefault()?.Split(" ").Last();
            if (string.IsNullOrWhiteSpace(token))
                return;
            ConfigurationManager<OpenIdConnectConfiguration> configurationManager =
                new ConfigurationManager<OpenIdConnectConfiguration>(
                    $"{authority}/.well-known/openid-configuration",
                    new OpenIdConnectConfigurationRetriever());

            OpenIdConnectConfiguration config = null;
            config = await configurationManager.GetConfigurationAsync();

            var tokenHandler = new JwtSecurityTokenHandler();

            tokenHandler.ValidateToken(
                token,
                new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidIssuer = authority,
                    IssuerSigningKeys = config.SigningKeys
                },
                out SecurityToken validatedToken); ;
            var jwtToken = (JwtSecurityToken)validatedToken;
            context.User.AddIdentity(new ClaimsIdentity(jwtToken.Claims));
        }
        catch
        {
            return;
        }
        await next.Invoke(context);
    }
}