using AutoFixture;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using SondageApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TestSondageApi.Fuzzing
{
    public class SmartFuzzingTest
    {
        private Fixture _fixture = new();
        private HttpClient _httpClient;

        public SmartFuzzingTest()
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
        public async Task GivenBadRoute_WhenRequest_Then404()
        {
            // Act
            var response = await _httpClient.GetAsync("/Survey/"+ _fixture.Create<string>());

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GivenBadApiKey_WhenRequest_Then401()
        {
            // Arrange
            _httpClient.DefaultRequestHeaders.Remove("x-api-key");
            _httpClient.DefaultRequestHeaders.Add("x-api-key", "aaaa");

            // Act
            var response = await _httpClient.GetAsync("/Survey");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GivenGoodRoute_WhenPutRequest_Then405()
        {
            // Act
            var response = await _httpClient.PutAsync("/Survey", GetValidContent());

            // Assert
            Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
        }

        [Fact]
        public async Task GivenGoodRoute_WhenDeleteRequest_Then405()
        {
            // Act
            var response = await _httpClient.DeleteAsync("/Survey");

            // Assert
            Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
        }

        [Fact]
        public async Task GivenContentWithAdditionalElement_WhenPostRequest_Then415()
        {
            // Arrange
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
                    new QuestionAnswerPair(4,0),
                    new QuestionAnswerPair(5,0),
                    new QuestionAnswerPair(6,0),
                }
            };

            // Act
            var response = await _httpClient.PostAsync("/Survey", JsonContent.Create(surveyAnswer));

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GivenContentWithMissingElement_WhenPostRequest_Then400()
        {
            // Arrange
            var surveyAnswer = new SurveyAnswer()
            {
                UserEmail = _fixture.Create<string>() + "@" + _fixture.Create<string>() + ".com",
                SurveyId = new Guid("386ed262-954f-4137-b931-0435cf0f3589"),
            };

            // Act
            var response = await _httpClient.PostAsync("/Survey", JsonContent.Create(surveyAnswer));

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GivenContentWrongTypeElement_WhenPostRequest_Then400()
        {
            // Arrange
            var surveyAnswer = new SurveyAnswer()
            {
                UserEmail = _fixture.Create<string>(),
                SurveyId = new Guid("386ed262-954f-4137-b931-0435cf0f3589"),
                QuestionAnswerPairList = new List<QuestionAnswerPair>()
                {
                    new QuestionAnswerPair(0,0),
                    new QuestionAnswerPair(1,0),
                    new QuestionAnswerPair(2,0),
                    new QuestionAnswerPair(3,0),
                }
            };
            // Act
            var response = await _httpClient.PostAsync("/Survey", JsonContent.Create(surveyAnswer));

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GivenInvalidContent_WhenPostRequest_Then400()
        {
            // Arrange
            var surveyAnswer = _fixture.Create<SurveyAnswer>();

            // Act
            var response = await _httpClient.PostAsync("/Survey", JsonContent.Create(surveyAnswer));

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
