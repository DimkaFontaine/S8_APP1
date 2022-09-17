namespace SondageApi.Models
{
    public class Survey 
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public IEnumerable<Questions> Questions { get; set; } = Enumerable.Empty<Questions>();
    }
}
