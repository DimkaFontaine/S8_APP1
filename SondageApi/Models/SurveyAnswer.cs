namespace SondageApi.Models;

public class SurveyAnswer
{
    public string UserEmail { get; set; } = string.Empty;
    public Guid SurveyId { get; set; } = new Guid();
    public List<QuestionAnswerPair> QuestionAnswerPairList { get; set; } = new List<QuestionAnswerPair>();
}