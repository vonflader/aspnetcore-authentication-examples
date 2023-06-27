using WebAppApiKeyAuthHandler.Authentication;
using WebAppApiKeyMiddleware.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var apiKeys = builder.Configuration.GetSection(AuthConstants.ApiKeySectionName)
                                   .Get<List<ApiKeyModel>>() ??
                                   new List<ApiKeyModel>();

builder.Services.AddAuthentication(AuthConstants.ApiKeyScheme)
    .AddScheme<ApiKeyAuthenticationSchemeOptions, ApiKeyAuthenticationSchemeHandler>(
    AuthConstants.ApiKeyScheme, options => options.ApiKeys = apiKeys);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
