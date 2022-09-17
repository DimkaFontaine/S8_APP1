namespace SondageApi.Models;

public class Response
{
    public string? UserEmail;
    public Guid? SondageId;
    public Dictionary<short, short>? QuestionResponsePair;
}