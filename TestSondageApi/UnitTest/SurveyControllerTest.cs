using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SondageApi.Controllers;
using SondageApi.Models;
using SondageApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace TestSondageApi.UnitTest
{
    public class SurveyControllerTest
    {
        private readonly Fixture _fixture = new();
        private readonly Mock<ISurveyReader> _mockSurveyReader = new();
        private readonly Mock<ISurveyAnswerSaver> _mockSurveyAnswerSaver = new();
        private SurveyController _surveyController;

        public SurveyControllerTest()
        {
            var logger = new Mock<ILogger<SurveyController>>();

            _surveyController = new SurveyController(logger.Object, _mockSurveyReader.Object, _mockSurveyAnswerSaver.Object);
        }

        [Fact]
        public async Task WhenGettingSurveys_ThenSurveysAreReturn()
        {
            // Arrange
            var expectedSurveys = _fixture.Create<Survey[]>();

            _mockSurveyReader.Setup(m => m.GetSurveys()).Returns(expectedSurveys);

            // Act
            var surveys =_surveyController.GetSurveys();

            // Assert
            Assert.Equal(expectedSurveys, surveys);
        }

        [Fact]
        public async Task GivenSurveyAnswer_WhenSubmitSurvey_ThenServeyIsSaved()
        {
            // Arrange
            var surveyAnswer = new SurveyAnswer()
            {
                SurveyId = _fixture.Create<Guid>(),
                UserEmail = _fixture.Create<MailAddress>().ToString(),
                QuestionAnswerPairList = _fixture.Create<List<QuestionAnswerPair>>()
            };

            SetupMockHappyPath(surveyAnswer);

            // Act
            var actionResult = await _surveyController.SubmitSurveyAsync(surveyAnswer);

            // Assert
            _mockSurveyAnswerSaver.Verify(m => m.SaveAnswerAsync(It.IsAny<SurveyAnswer>()), Times.Once);
            Assert.IsType<OkResult>(actionResult);
            Assert.True(((OkResult)actionResult).StatusCode == 200);
        }

        [Fact]
        public async Task GivenSurveyAnswer_WhenInvalidServeyId_ThenGetBadRequest()
        {
            // Arrange
            var surveyAnswer = new SurveyAnswer()
            {
                SurveyId = _fixture.Create<Guid>(),
                UserEmail = _fixture.Create<MailAddress>().ToString(),
                QuestionAnswerPairList = _fixture.Create<List<QuestionAnswerPair>>()
            };

            _mockSurveyReader
                .Setup(m => m.GetAllSurveyIds())
                .Returns(_fixture.Create<List<Guid>>());
            _mockSurveyReader
                .Setup(m => m.Contains(It.IsAny<SurveyAnswer>()))
                .Returns(true);
            _mockSurveyReader
                .Setup(m => m.AllQuestionAreAnswered(It.IsAny<SurveyAnswer>()))
                .Returns(true);
            _mockSurveyAnswerSaver
                .Setup(m => m.SaveAnswerAsync(It.IsAny<SurveyAnswer>()));

            // Act
            var actionResult = await _surveyController.SubmitSurveyAsync(surveyAnswer);

            // Assert
            _mockSurveyAnswerSaver.Verify(m => m.SaveAnswerAsync(It.IsAny<SurveyAnswer>()), Times.Never);
            Assert.IsType<BadRequestObjectResult>(actionResult);
            Assert.True(((BadRequestObjectResult)actionResult).StatusCode == 400);
        }

        [Fact]
        public async Task GivenSurveyAnswer_WhenInvalidEmail_ThenGetBadRequest()
        {
            // Arrange
            var surveyAnswer = new SurveyAnswer()
            {
                SurveyId = _fixture.Create<Guid>(),
                UserEmail = _fixture.Create<string>(),
                QuestionAnswerPairList = _fixture.Create<List<QuestionAnswerPair>>()
            };

            SetupMockHappyPath(surveyAnswer);

            // Act
            var actionResult = await _surveyController.SubmitSurveyAsync(surveyAnswer);

            // Assert
            _mockSurveyAnswerSaver.Verify(m => m.SaveAnswerAsync(It.IsAny<SurveyAnswer>()), Times.Never);
            Assert.IsType<BadRequestObjectResult>(actionResult);
            Assert.True(((BadRequestObjectResult)actionResult).StatusCode == 400);
        }

        [Fact]
        public async Task GivenSurveyAnswer_WhenEmptyEmail_ThenGetBadRequest()
        {
            // Arrange
            var surveyAnswer = new SurveyAnswer()
            {
                SurveyId = _fixture.Create<Guid>(),
                UserEmail = "",
                QuestionAnswerPairList = _fixture.Create<List<QuestionAnswerPair>>()
            };

            SetupMockHappyPath(surveyAnswer);

            // Act
            var actionResult = await _surveyController.SubmitSurveyAsync(surveyAnswer);

            // Assert
            _mockSurveyAnswerSaver.Verify(m => m.SaveAnswerAsync(It.IsAny<SurveyAnswer>()), Times.Never);
            Assert.IsType<BadRequestObjectResult>(actionResult);
            Assert.True(((BadRequestObjectResult)actionResult).StatusCode == 400);
        }

        [Fact]
        public async Task GivenSurveyAnswer_WhenEmptyQuestionAnswer_ThenGetBadRequest()
        {
            // Arrange
            var surveyAnswer = new SurveyAnswer()
            {
                SurveyId = _fixture.Create<Guid>(),
                UserEmail = _fixture.Create<MailAddress>().ToString(),
                QuestionAnswerPairList = new List<QuestionAnswerPair>()
            };

            SetupMockHappyPath(surveyAnswer);

            // Act
            var actionResult = await _surveyController.SubmitSurveyAsync(surveyAnswer);

            // Assert
            _mockSurveyAnswerSaver.Verify(m => m.SaveAnswerAsync(It.IsAny<SurveyAnswer>()), Times.Never);
            Assert.IsType<BadRequestObjectResult>(actionResult);
            Assert.True(((BadRequestObjectResult)actionResult).StatusCode == 400);
        }

        [Fact]
        public async Task GivenSurveyAnswer_WhenInvalidQuestionAnswer_ThenGetBadRequest()
        {
            // Arrange
            var surveyAnswer = new SurveyAnswer()
            {
                SurveyId = _fixture.Create<Guid>(),
                UserEmail = _fixture.Create<MailAddress>().ToString(),
                QuestionAnswerPairList = _fixture.Create<List<QuestionAnswerPair>>()
            };

            _mockSurveyReader
                .Setup(m => m.GetAllSurveyIds())
                .Returns(new List<Guid>() { surveyAnswer.SurveyId });
            _mockSurveyReader
                .Setup(m => m.Contains(It.IsAny<SurveyAnswer>()))
                .Returns(false);
            _mockSurveyReader
                .Setup(m => m.AllQuestionAreAnswered(It.IsAny<SurveyAnswer>()))
                .Returns(true);
            _mockSurveyAnswerSaver
                .Setup(m => m.SaveAnswerAsync(It.IsAny<SurveyAnswer>()));

            // Act
            var actionResult = await _surveyController.SubmitSurveyAsync(surveyAnswer);

            // Assert
            _mockSurveyAnswerSaver.Verify(m => m.SaveAnswerAsync(It.IsAny<SurveyAnswer>()), Times.Never);
            Assert.IsType<BadRequestObjectResult>(actionResult);
            Assert.True(((BadRequestObjectResult)actionResult).StatusCode == 400);
        }

        [Fact]
        public async Task GivenSurveyAnswer_WhenMissingAnswerInQuestionAnswer_ThenGetBadRequest()
        {
            // Arrange
            var surveyAnswer = new SurveyAnswer()
            {
                SurveyId = _fixture.Create<Guid>(),
                UserEmail = _fixture.Create<MailAddress>().ToString(),
                QuestionAnswerPairList = _fixture.Create<List<QuestionAnswerPair>>()
            };

            _mockSurveyReader
                .Setup(m => m.GetAllSurveyIds())
                .Returns(new List<Guid>() { surveyAnswer.SurveyId });
            _mockSurveyReader
                .Setup(m => m.Contains(It.IsAny<SurveyAnswer>()))
                .Returns(true);
            _mockSurveyReader
                .Setup(m => m.AllQuestionAreAnswered(It.IsAny<SurveyAnswer>()))
                .Returns(false);
            _mockSurveyAnswerSaver
                .Setup(m => m.SaveAnswerAsync(It.IsAny<SurveyAnswer>()));

            // Act
            var actionResult = await _surveyController.SubmitSurveyAsync(surveyAnswer);

            // Assert
            _mockSurveyAnswerSaver.Verify(m => m.SaveAnswerAsync(It.IsAny<SurveyAnswer>()), Times.Never);
            Assert.IsType<BadRequestObjectResult>(actionResult);
            Assert.True(((BadRequestObjectResult)actionResult).StatusCode == 400);
        }

        [Fact]
        public async Task GivenSurveyAnswer_WhenAlredyExistingUser_ThenGetBadRequest()
        {
            // Arrange
            var surveyAnswer = new SurveyAnswer()
            {
                SurveyId = _fixture.Create<Guid>(),
                UserEmail = _fixture.Create<MailAddress>().ToString(),
                QuestionAnswerPairList = _fixture.Create<List<QuestionAnswerPair>>()
            };

            _mockSurveyReader
                .Setup(m => m.GetAllSurveyIds())
                .Returns(new List<Guid>() { surveyAnswer.SurveyId });
            _mockSurveyReader
                .Setup(m => m.Contains(It.IsAny<SurveyAnswer>()))
                .Returns(true);
            _mockSurveyReader
                .Setup(m => m.AllQuestionAreAnswered(It.IsAny<SurveyAnswer>()))
                .Returns(true);
            _mockSurveyAnswerSaver
                .Setup(m => m.SaveAnswerAsync(It.IsAny<SurveyAnswer>())).Throws(new UnauthorizedAccessException());

            // Act
            var actionResult = await _surveyController.SubmitSurveyAsync(surveyAnswer);

            // Assert
            _mockSurveyAnswerSaver.Verify(m => m.SaveAnswerAsync(It.IsAny<SurveyAnswer>()), Times.Once);
            Assert.IsType<BadRequestObjectResult>(actionResult);
            Assert.True(((BadRequestObjectResult)actionResult).StatusCode == 400);
        }

        [Fact]
        public async Task GivenSurveyAnswer_WhenSaveError_ThenGetInternalServerError()
        {
            // Arrange
            var surveyAnswer = new SurveyAnswer()
            {
                SurveyId = _fixture.Create<Guid>(),
                UserEmail = _fixture.Create<MailAddress>().ToString(),
                QuestionAnswerPairList = _fixture.Create<List<QuestionAnswerPair>>()
            };

            _mockSurveyReader
                .Setup(m => m.GetAllSurveyIds())
                .Returns(new List<Guid>() { surveyAnswer.SurveyId });
            _mockSurveyReader
                .Setup(m => m.Contains(It.IsAny<SurveyAnswer>()))
                .Returns(true);
            _mockSurveyReader
                .Setup(m => m.AllQuestionAreAnswered(It.IsAny<SurveyAnswer>()))
                .Returns(true);
            _mockSurveyAnswerSaver
                .Setup(m => m.SaveAnswerAsync(It.IsAny<SurveyAnswer>())).Throws(new Exception());

            // Act
            var actionResult = await _surveyController.SubmitSurveyAsync(surveyAnswer);

            // Assert
            _mockSurveyAnswerSaver.Verify(m => m.SaveAnswerAsync(It.IsAny<SurveyAnswer>()), Times.Once);
            Assert.IsType<StatusCodeResult>(actionResult);
            Assert.Equal(500, ((StatusCodeResult)actionResult).StatusCode);
        }

        private void SetupMockHappyPath(SurveyAnswer surveyAnswer)
        {
            _mockSurveyReader
                .Setup(m => m.GetAllSurveyIds())
                .Returns(new List<Guid>() { surveyAnswer.SurveyId });
            _mockSurveyReader
                .Setup(m => m.Contains(It.IsAny<SurveyAnswer>()))
                .Returns(true);
            _mockSurveyReader
                .Setup(m => m.AllQuestionAreAnswered(It.IsAny<SurveyAnswer>()))
                .Returns(true);
            _mockSurveyAnswerSaver
                .Setup(m => m.SaveAnswerAsync(It.IsAny<SurveyAnswer>()));
        }
    }
}
