using SondageApi.Models;

namespace SondageApi.Services
{
    public class SondageResponseSaver : IResponse
    {
        private ILogger<SondageResponseSaver> _logger;

        public SondageResponseSaver(ILogger<SondageResponseSaver> logger)
        {
            _logger = logger;
        }

        public async Task SaveResponseAsync(Response response)
        {
            throw new NotImplementedException();
        }
    }
}