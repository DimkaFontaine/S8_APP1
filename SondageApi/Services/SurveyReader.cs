using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SondageApi.Models;
using System.Text.Json;
using System.Linq;
using Microsoft.VisualBasic;
using System.Collections.ObjectModel;
using JsonException = System.Text.Json.JsonException;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace SondageApi.Services
{
    public class SurveyReader : ISurveyReader
    {
        private readonly ILogger<SurveyReader> _logger;
        private readonly IFileWrapper _fileWrapper;
        private readonly DataBaseFileUri _dataBaseFileUri;
        private Lazy<List<Survey>> _surveys;
        public SurveyReader(ILogger<SurveyReader> logger, IOptions<DataBaseFileUri> dataBaseFileUri, IFileWrapper fileWrapper)
        {
            _dataBaseFileUri = dataBaseFileUri.Value;
            _logger = logger;
            _fileWrapper = fileWrapper;
            _surveys = new Lazy<List<Survey>>(() => GetSurveysInternal());

        }

        public IEnumerable<Survey> GetSurveys()
            => _surveys.Value;

        public bool Contains(SurveyAnswer answer) 
        {
            foreach (Survey survey in _surveys.Value) 
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

        public List<Guid> GetAllSurveyIds() 
            => _surveys.Value.Select(m => m.Id).ToList();

        public bool AllQuestionAreAnswered(SurveyAnswer answer) 
        {
            foreach (var _ in _surveys.Value.Where(survey => !(survey.Questions.Count() == answer.QuestionAnswerPairList.Count())).Select(survey => new { }))
            {
                return false;
            }
            return true;
        }

        private List<Survey> GetSurveysInternal()
        {
            try
            {
                var files = _fileWrapper.GetFiles(Directory.GetCurrentDirectory() + _dataBaseFileUri.DataBaseReadFileUri);
                var surveys = new List<Survey>();

                if (files is not null && files.Any())
                {
                    foreach (var file in files)
                    {
                        var jsonText = _fileWrapper.ReadAllText(file);
                        surveys.Add(JsonSerializer.Deserialize<Survey>(jsonText) ?? throw new ArgumentNullException(file));
                    }
                }
                return surveys;
            }
            catch (ArgumentNullException nullException)
            {
                _logger.LogError("utf8Json or returnType is null.");
                return Array.Empty<Survey>();
            }
            catch (JsonException jsonException)
            {
                _logger.LogError("The JSON is invalid, the returnType is not compatible with the JSON, or there is remaining data in the Stream.");
                return Array.Empty<Survey>();
            }
            catch (NotSupportedException notSupportedException) 
            {
                _logger.LogError("There is no compatible JsonConverter for returnType or its serializable members.");
                return Array.Empty<Survey>();
            }
            catch
            {
                _logger.LogError("An exception occured: Probably a wrong path to json.txt folder.");
                return new List<Survey>();
            }

        }
    }
}