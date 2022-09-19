using SondageApi.Models;

namespace SondageApi.Services
{
    public interface ISurveyReader
    {
        IEnumerable<Survey> GetSurveys();
        bool Contains(SurveyAnswer answer);
        List<Guid> GetAllSurveyIds();
        bool AllQuestionAreAnswered(SurveyAnswer answer);
    }
}