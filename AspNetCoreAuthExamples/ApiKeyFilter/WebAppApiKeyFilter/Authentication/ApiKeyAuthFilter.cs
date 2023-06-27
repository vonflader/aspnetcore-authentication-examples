using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebAppApiKeyFilter.Authentication
{
    public class ApiKeyAuthFilter : IAuthorizationFilter
    {
        private readonly IConfiguration _configuration;

        public ApiKeyAuthFilter(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if(!context.HttpContext.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName, out var extractedApiKey))
            {
                context.Result = new UnauthorizedObjectResult("Missing API key");
                return;
            }

            var apiKeys = _configuration.GetSection(AuthConstants.ApiKeySectionName)
                                        .Get<List<ApiKeyModel>>() ?? 
                                        new List<ApiKeyModel>();
            
            var validApiKeys = apiKeys.Select(x => x.Key).ToList();

            if (validApiKeys.Contains(extractedApiKey!))
                return;

            context.Result = new UnauthorizedObjectResult("Invalid API key");
        }
    }
}
