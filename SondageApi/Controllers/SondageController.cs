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
    private readonly ISondageResponse _sondageResponse;

    public SondageController(
        ILogger<SondageController> logger, 
        ISondageReader sondageReader, 
        ISondageResponse sondageResponse)
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

    [HttpPost(Name = "SubmitSondage/{questionIndex}/{responseIndex}")]
    public async Task SubmitSondageAsync(int questionIndex, int responseIndex)
    {
        await _sondageResponse.SaveResponseAsync(questionIndex, responseIndex);
    }
}
