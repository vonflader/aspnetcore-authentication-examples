using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using WebAppApiKeyMiddleware.Authentication;

namespace WebAppApiKeyAuthHandler.Authentication;

public class ApiKeyAuthenticationSchemeHandler : AuthenticationHandler<ApiKeyAuthenticationSchemeOptions>
{
    public ApiKeyAuthenticationSchemeHandler(IOptionsMonitor<ApiKeyAuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName, out var apiKeyHeaderValues))
        {
            return Task.FromResult(AuthenticateResult.Fail("API Key missing"));
        }

        var extractedApiKey = apiKeyHeaderValues.FirstOrDefault();

        if (string.IsNullOrEmpty(extractedApiKey))
        {
            return Task.FromResult(AuthenticateResult.Fail("API Key missing"));
        }

        var valiApiKeys = Options.ApiKeys.Select(x => x.Key).ToList();

        if (valiApiKeys.Contains(extractedApiKey))
        {
            var apiKey = Options.ApiKeys.FirstOrDefault(x => x.Key == extractedApiKey);

            if (apiKey is null)
                return Task.FromResult(AuthenticateResult.Fail("Invalid API Key"));

            var claims = new[] { new Claim(ClaimTypes.Name, apiKey.Name) };
            apiKey.Roles.ForEach(role => claims.Append(new Claim(ClaimTypes.Role, role)));
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        return Task.FromResult(AuthenticateResult.Fail("Invalid API Key"));
    }
}
