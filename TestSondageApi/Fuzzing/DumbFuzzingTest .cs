using AutoFixture;
using AutoFixture.Xunit2;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using SondageApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace TestSondageApi.Fuzzing
{
    public class DumbFuzzingTest
    {
        private readonly Fixture _fixture = new();
        private HttpClient _httpClient;
        public DumbFuzzingTest()
        {
            
            var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((context, conf) =>
                {
                    context.HostingEnvironment.EnvironmentName = "Test";
                    conf.AddJsonFile("appsettings.Test.json");
                });
            });

            _httpClient = application.CreateClient();
            _httpClient.DefaultRequestHeaders.Add("x-api-key", "abcd");

        }

        [Fact]
        public async Task GivenBadRoute_WhenGetRequest_Then404()
        {
            // Act
            var response = await _httpClient.GetAsync("/"+ _fixture.Create<string>());
            
            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GivenBadRoute_WhenPostRequest_Then404()
        {
            // Act
            var response = await _httpClient.PostAsync("/" + _fixture.Create<string>(), GetValidContent());
            
            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory]
        [InlineData("")]
        [InlineData("{}")]
        [InlineData("{fdajshngkjsf}")]
        [AutoData]
        public async Task GivenGoodRouteAndBadStringType_WhenPostRequest_Then415(string content)
        {
            // Act
            var response = await _httpClient.PostAsync("/Survey", new StringContent(content));
            
            // Assert
            Assert.Equal(HttpStatusCode.UnsupportedMediaType, response.StatusCode);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(-1)]
        [InlineData(int.MaxValue)]
        [AutoData]
        public async Task GivenGoodRouteAndBadintType_WhenPostRequest_Then400(int content)
        {
            // Act
            var response = await _httpClient.PostAsync("/Survey", JsonContent.Create(content));

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [InlineData("")]
        [InlineData("{}")]
        [InlineData("{fdajshngkjsf}")]
        public async Task GivenGoodRouteAndBadContent_WhenPostRequest_Then400(string content)
        {
            // Act
            var response = await _httpClient.PostAsync("/Survey", JsonContent.Create(content));

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        private HttpContent GetValidContent()
        {
            var surveyAnswer = new SurveyAnswer()
            {
                UserEmail = _fixture.Create<string>() + "@" + _fixture.Create<string>() + ".com",
                SurveyId = new Guid("386ed262-954f-4137-b931-0435cf0f3589"),
                QuestionAnswerPairList = new List<QuestionAnswerPair>()
                {
                    new QuestionAnswerPair(0,0),
                    new QuestionAnswerPair(1,0),
                    new QuestionAnswerPair(2,0),
                    new QuestionAnswerPair(3,0),
                }
            };

            return JsonContent.Create(surveyAnswer);
        }

    }
}
