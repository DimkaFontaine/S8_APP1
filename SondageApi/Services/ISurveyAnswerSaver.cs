using SondageApi.Models;

namespace SondageApi.Services
{
    public interface ISurveyAnswerSaver
    {
        Task SaveAnswerAsync(SurveyAnswer answer);
    }
}