namespace SondageApi.Models;

public class QuestionAnswerPair 
{
    public Guid SurveyId { get; set; }
    public short QuestionId { get; set; }
    public short AnswerId { get; set; }

    public QuestionAnswerPair(Guid surveyId, short questionId, short answerId) 
    {
        SurveyId = surveyId;
        QuestionId = questionId;
        AnswerId = answerId;
    }
}
