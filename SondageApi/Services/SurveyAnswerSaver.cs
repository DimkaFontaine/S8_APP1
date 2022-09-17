using SondageApi.Models;

namespace SondageApi.Services
{
    public class SurveyAnswerSaver : IAnswer
    {
        private ILogger<SurveyAnswerSaver> _logger;

        public SurveyAnswerSaver(ILogger<SurveyAnswerSaver> logger)
        {
            _logger = logger;
        }

        public async Task SaveAnswerAsync(SurveyAnswer answer)
        {
            throw new NotImplementedException();
        }
    }
}