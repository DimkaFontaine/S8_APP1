namespace SondageApi.Models
{
    public record Sondage(string SondageName,
                          IEnumerable<SondageQuestion> SondageQuestion);
}
