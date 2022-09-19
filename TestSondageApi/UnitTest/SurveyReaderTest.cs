using AutoFixture;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SondageApi.Models;
using SondageApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TestSondageApi.UnitTest
{
    public class SurveyReaderTest
    {
        private readonly Fixture _fixture = new();
        private readonly Mock<IFileWrapper> _fileWrapper = new();
        private SurveyReader _surveyReader;

        public SurveyReaderTest()
        {
            var logger = new Mock<ILogger<SurveyReader>>();
            var dataBaseFileUriOptions = Options.Create(
                new DataBaseFileUri()
                {
                    DataBaseWriteFileUri = string.Empty,
                });

            _surveyReader = new SurveyReader(logger.Object, dataBaseFileUriOptions, _fileWrapper.Object);
        }

        [Fact]
        public async Task WhenGettingSurveys_ThenSurveysAreReturn()
        {
            // Arrange 
            var expectedSurveys = SetupFileWrapper();

            // Act
            var surveys = _surveyReader.GetSurveys();

            // Assert
            Assert.NotEmpty(surveys);
            _fileWrapper.Verify(m => m.ReadAllText(It.IsAny<string>()), Times.Exactly(expectedSurveys.Count()));
            Assert.Equal(surveys.Count(), expectedSurveys.Count());
            Assert.Equal(expectedSurveys.FirstOrDefault().Id, surveys.FirstOrDefault().Id);

        }

        [Fact]
        public async Task GivenBadSurveyFile_WhenGettingSurveys_ThenNoSurvey()
        {
            // Arrange 
            var fileNameArray = _fixture.Create<string[]>();
            _fileWrapper.Setup(m => m.GetFiles(It.IsAny<string>())).Returns(fileNameArray);
            _fileWrapper.Setup(m => m.ReadAllText(It.IsAny<string>())).Returns(_fixture.Create<string>());

            // Act
            var surveys = _surveyReader.GetSurveys();

            // Assert
            Assert.Empty(surveys);
        }

        [Fact]
        public async Task GivenEmptySurveyFile_WhenGettingSurveys_ThenNoSurvey()
        {
            // Arrange 
            var fileNameArray = _fixture.Create<string[]>();
            _fileWrapper.Setup(m => m.GetFiles(It.IsAny<string>())).Returns(fileNameArray);
            _fileWrapper.Setup(m => m.ReadAllText(It.IsAny<string>())).Returns((string)null);

            // Act
            var surveys = _surveyReader.GetSurveys();

            // Assert
            Assert.Empty(surveys);
        }

        [Fact]
        public async Task GivenSurveyAnswer_WhenLookupContains_ThenItIsContain()
        {
            // Arrange 
            var expectedSurveys = SetupFileWrapper();
            var listQuestionAnswerPair = new List<QuestionAnswerPair>();

            foreach(var question in expectedSurveys.FirstOrDefault().Questions)
            {
                listQuestionAnswerPair
                    .Add(new QuestionAnswerPair(question.QuestionId, question.Answers.FirstOrDefault().Id));
            }

            var surveyAnswer = new SurveyAnswer()
            {
                SurveyId = expectedSurveys.FirstOrDefault().Id,
                UserEmail = _fixture.Create<MailAddress>().ToString(),
                QuestionAnswerPairList = listQuestionAnswerPair
            };

            // Act
            var contains = _surveyReader.Contains(surveyAnswer);

            // Assert
            Assert.True(contains);
        }

        [Fact]
        public async Task GivenSurveyAnswer_WhenLookupContains_ThenItIsNotContain()
        {
            // Arrange 
            SetupFileWrapper();
            var surveyAnswer = _fixture.Create<SurveyAnswer>();

            // Act
            var contains = _surveyReader.Contains(surveyAnswer);

            // Assert
            Assert.False(contains);
        }

        [Fact]
        public async Task GivenSurveyFile_WhenLookupAllSurveyIds_ThenSurveyIdsAreReturn()
        {
            // Arrange 
            var expectedSurveys = SetupFileWrapper();
            
            // Act
            var ids = _surveyReader.GetAllSurveyIds();

            // Assert
            Assert.True(ids.Contains(expectedSurveys.FirstOrDefault().Id));
            Assert.Equal(expectedSurveys.Count(), ids.Count());
        }

        [Fact]
        public async Task GivenSurveyAnswer_WhenAllQuestionAreAnswered_ThenIsTrue()
        {
            // Arrange 
            var expectedSurveys = SetupFileWrapper();
            var listQuestionAnswerPair = new List<QuestionAnswerPair>();

            foreach (var question in expectedSurveys.FirstOrDefault().Questions)
            {
                listQuestionAnswerPair
                    .Add(new QuestionAnswerPair(question.QuestionId, question.Answers.FirstOrDefault().Id));
            }

            var surveyAnswer = new SurveyAnswer()
            {
                SurveyId = expectedSurveys.FirstOrDefault().Id,
                UserEmail = _fixture.Create<MailAddress>().ToString(),
                QuestionAnswerPairList = listQuestionAnswerPair
            };

            // Act
            var isAnswered = _surveyReader.AllQuestionAreAnswered(surveyAnswer);

            // Assert
            Assert.True(isAnswered);
        }

        [Fact]
        public async Task GivenSurveyAnswer_WhenAllQuestionAreNotAnswered_ThenIsFasle()
        {
            // Arrange 
            var expectedSurveys = SetupFileWrapper();
            var listQuestionAnswerPair = new List<QuestionAnswerPair>();

            listQuestionAnswerPair
                .Add(new QuestionAnswerPair(
                    expectedSurveys.FirstOrDefault().Questions.FirstOrDefault().QuestionId,
                    expectedSurveys.FirstOrDefault().Questions.FirstOrDefault().Answers.FirstOrDefault().Id));


            var surveyAnswer = new SurveyAnswer()
            {
                SurveyId = expectedSurveys.FirstOrDefault().Id,
                UserEmail = _fixture.Create<MailAddress>().ToString(),
                QuestionAnswerPairList = listQuestionAnswerPair
            };

            // Act
            var isAnswered = _surveyReader.AllQuestionAreAnswered(surveyAnswer);

            // Assert
            Assert.False(isAnswered);
        }




        private List<Survey> SetupFileWrapper()
        {
            var survey1 = _fixture.Create<Survey>();
            var survey2 = _fixture.Create<Survey>();
            var survey3 = _fixture.Create<Survey>();
            var surveys = new List<Survey>() { survey1, survey2, survey3 };

            _fileWrapper.Setup(m => m.GetFiles(It.IsAny<string>())).Returns(surveys.Select(m => m.Name).ToArray());
            _fileWrapper.SetupSequence(m => m.ReadAllText(It.IsAny<string>()))
                .Returns(JsonSerializer.Serialize(survey1))
                .Returns(JsonSerializer.Serialize(survey2))
                .Returns(JsonSerializer.Serialize(survey3));

            return surveys;
        }

    }
}
