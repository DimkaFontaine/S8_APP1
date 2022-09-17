namespace SondageApi.Services
{
    public interface IResponse
    {
        Task SaveResponseAsync(int questionIndex, int responseIndex);
    }
}