using Microsoft.AspNetCore.Mvc;
using SondageApi.Models;
using System.Reflection;
using System.IO;
using System.Runtime.ConstrainedExecution;
using static System.Net.Mime.MediaTypeNames;
using Newtonsoft.Json;
using System.Linq;

namespace SondageApi.Controllers;

[ApiController]
[Route("[controller]")]
public class SondageController : ControllerBase
{
    private readonly ILogger<SondageController> _logger;

    public SondageController(ILogger<SondageController> logger)
    {
        _logger = logger;
        Get();
    }

    [HttpGet(Name = "GetSondage")]
    public IEnumerable<Sondage> Get()
    {
        int fileNum = 0;
        string[] files = Directory.GetFiles(DataBaseFileUri.Section);
        List<Sondage> sondages = new List<Sondage>();

        foreach (string file in files)
        {
            _logger.LogInformation(file);
            sondages.Add(JsonConvert.DeserializeObject<Sondage>(file));
            fileNum++;
        }
        return sondages;
    }
}
