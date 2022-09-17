using SondageApi.Models;

namespace SondageApi.Services
{
    public interface IAnswer
    {
        Task SaveAnswerAsync(SurveyAnswer answer);
    }
}