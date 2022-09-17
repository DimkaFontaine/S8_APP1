namespace SondageApi.Services
{
    public class SondageResponseSaver : IResponse
    {
        private ILogger<SondageResponseSaver> _logger;

        public SondageResponseSaver(ILogger<SondageResponseSaver> logger)
        {
            _logger = logger;
        }

        public async Task SaveResponseAsync(int questionIndex, int responseIndex)
        {
            throw new NotImplementedException();
        }
    }
}