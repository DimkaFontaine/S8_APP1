using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SondageApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.Json;

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
            List<Guid> sondagesGuids = new List<Guid>();
            foreach (Survey sondage in _surveys) 
            {
                sondagesGuids.Add(sondage.Id);
            }
            return sondagesGuids;
        }

        public List<QuestionAnswerPair> GetAllSurveyQuestionAnswerPairs() 
        {
            if (_surveys is null)
            {
                GetSurveys();
            }
            List<QuestionAnswerPair> questionResponsePairs = new List<QuestionAnswerPair>();
            foreach (Survey survey in _surveys) 
            {
                foreach (Questions question in survey.Questions) 
                {
                    foreach (Answer answer in question.Answers) 
                    {
                        questionResponsePairs.Add(new QuestionAnswerPair(question.QuestionId, answer.Id));
                    }
                }
            }
            return questionResponsePairs;
        }

        public bool AllQuestionAreAnswered() 
        {
            if (_surveys is null)
            {
                GetSurveys();
            }
            List<QuestionAnswerPair> questionAnswerPair = GetAllSurveyQuestionAnswerPairs();
            int nQuestion = 0;
            foreach (Survey survey in _surveys) 
            {
                nQuestion = nQuestion + survey.Questions.Count();
            }
            return nQuestion == questionAnswerPair.Count();
        }
    }
}