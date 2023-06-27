namespace WebAppApiKeyMiddleware.Authentication;

public class ApiKeyModel
{
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
}
