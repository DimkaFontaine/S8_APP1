namespace SondageApi.Models;

public class ApiKeySettings
{
    public string Key { get; set; } = string.Empty;
    public const string Section = "ApiKey";
    public const string ApiKeyHeader = "X-API-KEY";
}