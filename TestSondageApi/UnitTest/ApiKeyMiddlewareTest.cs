using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SondageApi.Middleware;
using SondageApi.Models;
using SondageApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSondageApi.UnitTest
{
    public class ApiKeyMiddlewareTest
    {
        
        private readonly Fixture _fixture = new();
        private ApiKeyMiddleware _apiKeyMiddleware;

        private string _apiKey = string.Empty;

        public ApiKeyMiddlewareTest()
        {
            _apiKey = _fixture.Create<string>();
        }

        [Fact]
        public async Task GivenHttpRequest_WhenGoodApiKey_ThenHttpRequestIsTreated()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<ApiKeyMiddleware>>();
            var apiKeySettingsOptions = Options.Create(
                new ApiKeySettings()
                {
                    Key = _apiKey
                });
            var nextTriggered = false;
            RequestDelegate next = (HttpContext httpContext) => 
                {
                    nextTriggered = true;
                    return Task.CompletedTask;
                };
            var context = new DefaultHttpContext();
            context.Request.Headers.Add(ApiKeySettings.ApiKeyHeader, _apiKey);

            _apiKeyMiddleware = new ApiKeyMiddleware(loggerMock.Object, next, apiKeySettingsOptions);

            // Act
            await _apiKeyMiddleware.InvokeAsync(context);

            // Assert
            Assert.True(nextTriggered);
        }

        [Fact]
        public async Task GivenHttpRequest_WhenBadApiKey_ThenHttpRequestIsBlock()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<ApiKeyMiddleware>>();
            var apiKeySettingsOptions = Options.Create(
                new ApiKeySettings()
                {
                    Key = _apiKey
                });
            var nextTriggered = false;
            RequestDelegate next = (HttpContext httpContext) =>
            {
                nextTriggered = true;
                return Task.CompletedTask;
            };
            var context = new DefaultHttpContext();
            context.Request.Headers.Add(ApiKeySettings.ApiKeyHeader, _fixture.Create<string>());

            _apiKeyMiddleware = new ApiKeyMiddleware(loggerMock.Object, next, apiKeySettingsOptions);

            // Act
            await _apiKeyMiddleware.InvokeAsync(context);

            // Assert
            Assert.False(nextTriggered);
        }

        [Fact]
        public async Task GivenHttpRequest_WhenNoApiKey_ThenHttpRequestIsBlock()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<ApiKeyMiddleware>>();
            var apiKeySettingsOptions = Options.Create(
                new ApiKeySettings()
                {
                    Key = _apiKey
                });
            var nextTriggered = false;
            RequestDelegate next = (HttpContext httpContext) =>
            {
                nextTriggered = true;
                return Task.CompletedTask;
            };
            var context = new DefaultHttpContext();

            _apiKeyMiddleware = new ApiKeyMiddleware(loggerMock.Object, next, apiKeySettingsOptions);

            // Act
            await _apiKeyMiddleware.InvokeAsync(context);

            // Assert
            Assert.False(nextTriggered);
        }
    }
}
