using SondageApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SondageApi.Services
{
    public interface ISurveyReader
    {
        IEnumerable<Survey> GetSurveys();
        bool Contains(SurveyAnswer answer);
        List<Guid> GetAllSurveyIds();
        List<QuestionAnswerPair> GetAllSurveyQuestionAnswerPairs();
        bool AllQuestionAreAnswered(SurveyAnswer answer);
    }
}