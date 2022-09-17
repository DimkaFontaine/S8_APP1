using Microsoft.Extensions.Options;
using SondageApi.Models;
using System.Net;

namespace SondageApi.Middleware;

public class ApiKeyMiddleware
{
    private readonly ILogger<ApiKeyMiddleware> _logger;
    private readonly RequestDelegate _next;
    private ApiKeySettings _apiKeySettings;

    public ApiKeyMiddleware(
        ILogger<ApiKeyMiddleware> logger, 
        RequestDelegate next, 
        IOptions<ApiKeySettings> apiKeySettings)
    {
        _logger = logger;
        _next = next;
        _apiKeySettings = apiKeySettings.Value;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        if(!httpContext.Request.Headers.TryGetValue(ApiKeySettings.ApiKeyHeader, out var headerApiKey))
        {
            _logger.LogInformation("No API key in header request");
            
            httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await httpContext.Response.WriteAsync("No API key in header request");
            return;
        }

        if(_apiKeySettings.Key != headerApiKey)
        {
            _logger.LogInformation("Unauthorized API key");

            httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await httpContext.Response.WriteAsync("Unauthorized API key");
            return;
        }

        await _next(httpContext);
    }
}