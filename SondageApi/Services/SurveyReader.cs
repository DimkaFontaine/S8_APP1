using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SondageApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.Json;
using System.Linq;

namespace SondageApi.Services
{
    public class SurveyReader : ISurveyReader
    {
        private ILogger<SurveyReader> _logger;
        private DataBaseFileUri _dataBaseFileUri;
        private List<Survey>? _surveys;
        public SurveyReader(ILogger<SurveyReader> logger, IOptions<DataBaseFileUri> dataBaseFileUri)
        {
            _dataBaseFileUri = dataBaseFileUri.Value;
            _logger = logger;
        }

        public bool Contains(SurveyAnswer answer) 
        {
            if (_surveys is null)
            {
                GetSurveys();
            }
            foreach (Survey survey in _surveys) 
            {
                int qNum = 0;
                bool surveyIdCheck = survey.Id == answer.SurveyId;
                foreach (QuestionAnswerPair qaPair in answer.QuestionAnswerPairList)
                {
                    bool surveyQuestionCheck = survey.Questions.Any(q => q.QuestionId == qaPair.QuestionId);
                    bool surveyAnswerCheck = survey.Questions.ElementAt(qNum).Answers.Any(a => a.Id == qaPair.AnswerId);
                    if (surveyIdCheck && surveyQuestionCheck && surveyAnswerCheck)
                    {
                        return true;
                    }
                    qNum++;
                }
            }
            return false;
        }

        public IEnumerable<Survey> GetSurveys()
        {
            if (_surveys is not null)
            {
                return _surveys;
            }

            try
            {
                string[] files = Directory.GetFiles(Directory.GetCurrentDirectory() + _dataBaseFileUri.DataBaseReadFileUri);
                _surveys = new List<Survey>();

                if (files is not null && files.Any())
                {
                    foreach (string file in files)
                    {
                        string jsonText = File.ReadAllText(file);
                        _surveys.Add(JsonSerializer.Deserialize<Survey>(jsonText) ?? throw new ArgumentNullException(file));
                    }
                }
                return _surveys;
            }
            catch (ArgumentNullException nullEx)
            {
                _logger.LogError("Could not Deserialize " + nullEx.ToString() + "to json object.");
                return Array.Empty<Survey>();
            }
            catch
            {
                _logger.LogError("An exception occured: Probably a wrong path to json.txt folder.");
                return Array.Empty<Survey>();
            }

        }

        public List<Guid> GetAllSurveyIds() 
        {
            if(_surveys is null)
            {
                GetSurveys();
            }
            return (_surveys.Select(sondage => sondage.Id)).ToList();
        }

        public List<QuestionAnswerPair> GetAllSurveyQuestionAnswerPairs() 
        {
            if (_surveys is null)
            {
                GetSurveys();
            }
            return (_surveys.SelectMany(survey => survey.Questions.SelectMany(question => question.Answers.Select(answer => new QuestionAnswerPair(question.QuestionId, answer.Id))))).ToList();
        }

        public bool AllQuestionAreAnswered(SurveyAnswer answer) 
        {
            foreach (var _ in _surveys.Where(survey => !(survey.Questions.Count() == answer.QuestionAnswerPairList.Count())).Select(survey => new { }))
            {
                return false;
            }
            return true;
        }
    }
}