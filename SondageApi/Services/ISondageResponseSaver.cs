using SondageApi.Models;

namespace SondageApi.Services
{
    public interface IResponse
    {
        Task SaveResponseAsync(Response response);
    }
}