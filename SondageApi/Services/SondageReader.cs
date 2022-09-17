using SondageApi.Models;

namespace SondageApi.Services
{
    public class SondageReader : ISondageReader
    {
        private ILogger<SondageReader> _logger;

        public SondageReader(ILogger<SondageReader> logger)
        {
            _logger = logger;
        }

        public IEnumerable<SondageQuestion> GetSondageQuestions()
        {
            throw new NotImplementedException();
        }
    }
}