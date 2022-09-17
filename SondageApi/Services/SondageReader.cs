using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SondageApi.Models;
using System.Text.Json;

namespace SondageApi.Services
{
    public class SondageReader : ISondageReader
    {
        private ILogger<SondageReader> _logger;
        private DataBaseFileUri _dataBaseFileUri;
        private List<Sondage>? _sondages;
        public SondageReader(ILogger<SondageReader> logger, IOptions<DataBaseFileUri> dataBaseFileUri)
        {
            _dataBaseFileUri = dataBaseFileUri.Value;
            _logger = logger;
        }

        public IEnumerable<Sondage> GetSondages()
        {
            if (_sondages is not null)
            {
                return _sondages;
            }

            try
            {
                _logger.LogInformation("\tTrying to read: " + Directory.GetCurrentDirectory() + _dataBaseFileUri.DataBaseReadFileUri);
                _logger.LogInformation("\tDirectory Exists:  " + Directory.Exists(Directory.GetCurrentDirectory() + _dataBaseFileUri.DataBaseReadFileUri));
                string[] files = Directory.GetFiles(Directory.GetCurrentDirectory() + _dataBaseFileUri.DataBaseReadFileUri);
                _sondages = new List<Sondage>();

                if (files is not null && files.Any())
                {
                    foreach (string file in files)
                    {
                        string jsonText = File.ReadAllText(file);
                        _sondages.Add(JsonSerializer.Deserialize<Sondage>(jsonText) ?? throw new ArgumentNullException(file));
                    }
                }
                return _sondages;
            }
            catch (ArgumentNullException nullEx)
            {
                _logger.LogError("Could not Deserialize " + nullEx.ToString() + "to json object.");
                return Array.Empty<Sondage>();
            }
            catch
            {
                _logger.LogError("An exception occured: Probably a wrong path to json.txt folder.");
                return Array.Empty<Sondage>();
            }

        }
    }
}