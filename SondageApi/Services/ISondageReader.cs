using SondageApi.Models;

namespace SondageApi.Services
{
    public interface ISondageReader
    {
        IEnumerable<Sondage> GetSondages();
    }
}