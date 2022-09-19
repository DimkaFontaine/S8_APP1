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
        if (!ValidateUserEmail(answer))
        {
            return BadRequest("Invalid email");
        }

        if(!ValidateSurveyId(answer))
        {
            return BadRequest("Invalid survey id");
        }

        if (!ValidateAllQuestionsAnswers(answer))
        {
            return BadRequest("Invalid survey content");
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
        if (!answer.QuestionAnswerPairList.Any())
        {
            _logger.LogInformation("There are no question/answer pair.");
            return false;
        }
        else
        {
            if (!_surveyReader.Contains(answer))
            {
                _logger.LogInformation("Question/Answer mismatch with original survey.");
                return false;
            }

            if (!_surveyReader.AllQuestionAreAnswered(answer))
            {
                _logger.LogInformation("Not all survey's questions have been answered.");
                return false;
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