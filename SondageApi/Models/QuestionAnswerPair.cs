namespace SondageApi.Models;

public class QuestionAnswerPair 
{
    public short QuestionId { get; set; }
    public short AnswerId { get; set; }

    public QuestionAnswerPair(short questionId, short answerId) 
    {
        QuestionId = questionId;
        AnswerId = answerId;
    }
}
