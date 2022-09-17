using Microsoft.AspNetCore.Mvc;
using SondageApi.Models;
using SondageApi.Services;

namespace SondageApi.Controllers;

[ApiController]
[Route("[controller]")]
public class SondageController : ControllerBase
{
    private readonly ILogger<SondageController> _logger;
    private readonly ISondageReader _sondageReader;
    private readonly IResponse _sondageResponse;

    public SondageController(
        ILogger<SondageController> logger, 
        ISondageReader sondageReader, 
        IResponse sondageResponse)
    {
        _logger = logger;
        _sondageReader = sondageReader;
        _sondageResponse = sondageResponse;
    }

    [HttpGet(Name = "GetSondages")]
    public IEnumerable<Sondage> GetSondages()
    {
        return _sondageReader.GetSondages();
    }

    [HttpPost(Name = "SubmitSondage")]
    public async Task<IActionResult> SubmitSondageAsync(Response response)
    {
        if(ValidateSondageId(response))
        {
            return BadRequest();
        }

        if (ValidateUserEmail(response))
        {
            return BadRequest();
        }

        if (ValidateAllQuestionsResponses(response))
        {
            return BadRequest();
        }

        await _sondageResponse.SaveResponseAsync(response);
        return Ok();
    }

    private bool ValidateAllQuestionsResponses(Response response)
    {
        throw new NotImplementedException();
    }

    private bool ValidateUserEmail(Response response)
    {
        throw new NotImplementedException();
    }

    private bool ValidateSondageId(Response response)
    {
        //return _sondageReader.GetAllSondageID().containe(response.SondageId)
        throw new NotImplementedException();
    }
}
