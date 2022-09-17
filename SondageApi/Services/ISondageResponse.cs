namespace SondageApi.Services
{
    public interface ISondageResponse
    {
        Task SaveResponseAsync(int questionIndex, int responseIndex);
    }
}