using Microsoft.AspNetCore.Mvc;
using SondageApi.Models;
using SondageApi.Services;
using System.Net;
using System.Net.Mail;

namespace SondageApi.Controllers;

[ApiController]
[Route("[controller]")]
public class SurveyController : ControllerBase
{
    private readonly ILogger<SurveyController> _logger;
    private readonly ISurveyReader _surveyReader;
    private readonly ISurveyAnswerSaver _surveyAnswer;

    public SurveyController(
        ILogger<SurveyController> logger,
        ISurveyReader surveyReader,
        ISurveyAnswerSaver surveyAnswer)
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
        if (!ValidateSurveyId(answer))
        {
            _logger.LogInformation("Invalid survey id.");
            return BadRequest("Invalid survey id.");
        }

        if (!ValidateUserEmail(answer))
        {
            _logger.LogInformation("Invalid survey email.");
            return BadRequest("Invalid survey email.");
        }

        if (!ValidateAllQuestionsAnswers(answer))
        {
            _logger.LogInformation("Invalid survey format.");
            return BadRequest("Invalid survey format.");
        }

        try
        {
            await _surveyAnswer.SaveAnswerAsync(answer);
            return Ok();
        }
        catch (UnauthorizedAccessException)
        {
            _logger.LogInformation("Already existing answer from this user");
            return BadRequest("Already existing answer from this user");
        }
        catch (Exception e)
        {
            _logger.LogError("Faild with error:{e.Message}, to save response:{response}", e.Message, answer);
            return StatusCode((int)HttpStatusCode.InternalServerError);
        }
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
                // TODO: fix bug (Contains not working) 
                //if (!allQuestionAnswerPairs.Contains(questionAnswerPair))
                //{
                //    throw new ArgumentException("Question/Response pair mismatch with original survey.");
                //}
                //else if(!_surveyReader.AllQuestionAreAnswered())
                //{
                //    throw new ArgumentException("Not all survey's questions have been answered.");
                //}
            }
        }
        return true;
    }

    private bool ValidateUserEmail(SurveyAnswer answer)
    {
        try
        {
            return !string.IsNullOrEmpty(answer.UserEmail) && new MailAddress(answer.UserEmail) is not null;
        }
        catch
        {
            return false;
        }
    }

    private bool ValidateSurveyId(SurveyAnswer answer)
    {
        return _surveyReader.GetAllSurveyIds().Contains((Guid)answer.SurveyId);
    }
}