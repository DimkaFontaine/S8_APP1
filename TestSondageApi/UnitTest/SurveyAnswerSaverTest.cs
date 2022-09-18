using AutoFixture;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SondageApi.Models;
using SondageApi.Services;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace TestSondageApi.UnitTest
{
    public class SurveyAnswerSaverTest
    {
        private readonly Fixture _fixture = new();
        private readonly Mock<IFileWrapper> _fileWrapper = new();
        private SurveyAnswerSaver _surveyAnswerSaver;

        private const string Path = "\\DatabaseTest\\";

        public SurveyAnswerSaverTest()
        {
            var loggerMock = new Mock<ILogger<SurveyAnswerSaver>>();
            var dataBaseFileUriOptions = Options.Create(
                new DataBaseFileUri()
                {
                    DataBaseWriteFileUri = Path
                });

            _surveyAnswerSaver = new SurveyAnswerSaver(loggerMock.Object, dataBaseFileUriOptions, _fileWrapper.Object);
        }

        [Fact]
        public async Task GivenNewAnswer_WhenFirstServeyAnswer_ThenAnswerIsSave()
        {
            // Arrange 
            _fileWrapper.Setup(m => m.Exists(It.IsAny<string>())).Returns(false);
            _fileWrapper.Setup(m => m.Create(It.IsAny<string>()));
            _fileWrapper.Setup(m => m.WriteAllText(It.IsAny<string>(), It.IsAny<string>()));

            var surveyAnswer = _fixture.Create<SurveyAnswer>();

            // Act
            await _surveyAnswerSaver.SaveAnswerAsync(surveyAnswer);

            // Assert
            _fileWrapper.Verify(
                m =>
                m.WriteAllText(
                    It.Is<string>(x => x.Contains(surveyAnswer.SurveyId.ToString())),
                    It.Is<string>(x => x.Contains(surveyAnswer.UserEmail.ToString()))),
                Times.Once);
        }

        [Fact]
        public async Task GivenNewAnswer_WhenExistingServeyAnswer_ThenAnswerIsSave()
        {
            // Arrange 
            var oldAnswer = _fixture.Create<Dictionary<string, List<QuestionAnswerPair>>>();
            var oldAnswerJson = JsonSerializer.Serialize(oldAnswer);

            _fileWrapper.Setup(m => m.Exists(It.IsAny<string>())).Returns(true);
            _fileWrapper.Setup(m => m.ReadAllTextAsync(It.IsAny<string>())).ReturnsAsync(oldAnswerJson);
            _fileWrapper.Setup(m => m.WriteAllText(It.IsAny<string>(), It.IsAny<string>()));

            var surveyAnswer = _fixture.Create<SurveyAnswer>();

            // Act
            await _surveyAnswerSaver.SaveAnswerAsync(surveyAnswer);

            // Assert
            _fileWrapper.Verify(
                m =>
                m.WriteAllText(
                    It.Is<string>(x => x.Contains(surveyAnswer.SurveyId.ToString())),
                    It.Is<string>(x => x.Contains(surveyAnswer.UserEmail.ToString()) &&
                        x.Contains(oldAnswer.Keys.FirstOrDefault()))),
                Times.Once);
        }

        [Fact]
        public async Task GivenNewAnswer_WhenDataBaseCorrupted_ThenDoesNotSaveAnswer()
        {
            // Arrange 
            _fileWrapper.Setup(m => m.Exists(It.IsAny<string>())).Returns(true);
            _fileWrapper.Setup(m => m.ReadAllTextAsync(It.IsAny<string>())).ReturnsAsync(_fixture.Create<string>());

            var surveyAnswer = _fixture.Create<SurveyAnswer>();

            // Act
            var act = () => _surveyAnswerSaver.SaveAnswerAsync(surveyAnswer);

            // Assert
            Assert.ThrowsAsync<Exception>(act);
        }

        [Fact]
        public async Task GivenNewAnswer_WhenAlreadyUsedEmail_ThenDoesNotSaveAnswer()
        {
            // Arrange 
            var surveyAnswer = _fixture.Create<SurveyAnswer>();
            var oldAnswer = _fixture.Create<Dictionary<string, List<QuestionAnswerPair>>>();

            oldAnswer.Add(surveyAnswer.UserEmail, surveyAnswer.QuestionAnswerPairList);

            var oldAnswerJson = JsonSerializer.Serialize(oldAnswer);

            _fileWrapper.Setup(m => m.Exists(It.IsAny<string>())).Returns(true);
            _fileWrapper.Setup(m => m.ReadAllTextAsync(It.IsAny<string>())).ReturnsAsync(oldAnswerJson);
            _fileWrapper.Setup(m => m.WriteAllText(It.IsAny<string>(), It.IsAny<string>()));


            // Act
            var act = () => _surveyAnswerSaver.SaveAnswerAsync(surveyAnswer);

            // Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(act);
        }
    }
}