using Microsoft.AspNetCore.Mvc;
using SondageApi.Models;

namespace SondageApi.Controllers;

[ApiController]
[Route("[controller]")]
public class SondageController : ControllerBase
{
    private readonly ILogger<SondageController> _logger;

    public SondageController(ILogger<SondageController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetSondage")]
    public IEnumerable<SondageQuestion> Get()
    {
        return Enumerable.Range(1, 5)
            .Select(index => 
                new SondageQuestion("random question " + index, new[]{"a", "b", "c"}))
            .ToArray();
    }
}
