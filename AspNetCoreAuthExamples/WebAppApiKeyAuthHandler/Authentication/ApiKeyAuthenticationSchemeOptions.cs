using Microsoft.AspNetCore.Authentication;
using WebAppApiKeyAuthHandler.Authentication;

namespace WebAppApiKeyMiddleware.Authentication;

public class ApiKeyAuthenticationSchemeOptions : AuthenticationSchemeOptions
{
    public List<ApiKeyModel> ApiKeys { get; set; } = new();
}
