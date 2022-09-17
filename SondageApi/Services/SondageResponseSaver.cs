namespace SondageApi.Services
{
    public class Responses : ISondageResponse
    {
        private ILogger<Responses> _logger;

        public Responses(ILogger<Responses> logger)
        {
            _logger = logger;
        }

        public async Task SaveResponseAsync(int questionIndex, int responseIndex)
        {
            throw new NotImplementedException();
        }
    }
}