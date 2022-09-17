namespace SondageApi.Models
{
    public record Sondage(Guid Id,string Name, IEnumerable<Questions> Questions);
}
