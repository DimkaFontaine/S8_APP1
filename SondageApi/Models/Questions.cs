namespace SondageApi.Models;

public class Questions
{
    public short QuestionId { get; set; } = short.MinValue;
    public string QuestionName { get; set; } = string.Empty;
    public IEnumerable<Answer> Answers { get; set; } = Array.Empty<Answer>();
}