using Microsoft.AspNetCore.Mvc;
using SondageApi.Models;
using SondageApi.Services;

namespace SondageApi.Controllers;

[ApiController]
[Route("[controller]")]
public class SurveyController : ControllerBase
{
    private readonly ILogger<SurveyController> _logger;
    private readonly ISurveyReader _surveyReader;
    private readonly IAnswer _surveyAnswer;

    public SurveyController(
        ILogger<SurveyController> logger,
        ISurveyReader surveyReader,
        IAnswer surveyAnswer)
    {
        _logger = logger;
        _surveyReader = surveyReader;
        _surveyAnswer = surveyAnswer;
    }

    [HttpGet(Name = "GetSurveys")]
    public IEnumerable<Survey> GetSurveys()
    {
        return _surveyReader.GetSurveys();
    }

    [HttpPost(Name = "SubmitSurvey")]
    public async Task<IActionResult> SubmitSurveyAsync(SurveyAnswer answer)
    {
        if (ValidateSurveyId(answer))
        {
            return BadRequest();
        }

        if (ValidateUserEmail(answer))
        {
            return BadRequest();
        }

        if (ValidateAllQuestionsAnswers(answer))
        {
            return BadRequest();
        }

        await _surveyAnswer.SaveAnswerAsync(answer);
        return Ok();
    }

    private bool ValidateAllQuestionsAnswers(SurveyAnswer answer)
    {
        List<QuestionAnswerPair> allQuestionAnswerPairs = new List<QuestionAnswerPair>(_surveyReader.GetAllSurveyQuestionAnswerPairs());
        if (!answer.QuestionAnswerPairList.Any())
        {
            throw new ArgumentException("There are no question/answer pair.");
        }
        else
        {
            foreach (QuestionAnswerPair questionAnswerPair in answer.QuestionAnswerPairList) 
            {
                if (!allQuestionAnswerPairs.Contains(questionAnswerPair))
                {
                    throw new ArgumentException("Question/Response pair mismatch with original survey.");
                }
                else if(!_surveyReader.AllQuestionAreAnswered())
                {
                    throw new ArgumentException("Not all survey's questions have been answered.");
                }
            }
        }
        return true;
    }

    private bool ValidateUserEmail(SurveyAnswer answer)
    {
        throw new NotImplementedException();
    }

    private bool ValidateSurveyId(SurveyAnswer answer)
    {
        return _surveyReader.GetAllSurveyIds().Contains((Guid)answer.SurveyId);
    }
}