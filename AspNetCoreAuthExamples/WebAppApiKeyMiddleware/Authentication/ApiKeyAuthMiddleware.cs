using Microsoft.AspNetCore.Mvc;

namespace WebAppApiKeyMiddleware.Authentication;

public class ApiKeyAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public ApiKeyAuthMiddleware(RequestDelegate next,
        IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.HttpContext.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName, out var extractedApiKey))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Missing API key");
            return;
        }

        var apiKeys = _configuration.GetSection(AuthConstants.ApiKeySectionName)
                                    .Get<List<ApiKeyModel>>() ??
                                    new List<ApiKeyModel>();

        var validApiKeys = apiKeys.Select(x => x.Key).ToList();

        if (validApiKeys.Contains(extractedApiKey!))
        {
            await _next(context);
            return;
        }

        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsync("Invalid API key");
    }
}
