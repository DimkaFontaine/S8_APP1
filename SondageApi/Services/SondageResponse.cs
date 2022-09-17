namespace SondageApi.Services
{
    public class SondageResponse : ISondageResponse
    {
        private ILogger<SondageResponse> _logger;

        public SondageResponse(ILogger<SondageResponse> logger)
        {
            _logger = logger;
        }

        public async Task SaveResponseAsync(int questionIndex, int responseIndex)
        {
            throw new NotImplementedException();
        }
    }
}