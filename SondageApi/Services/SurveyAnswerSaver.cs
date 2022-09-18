using Microsoft.Extensions.Options;
using SondageApi.Models;
using System.Text.Json;

namespace SondageApi.Services
{
    public class SurveyAnswerSaver : ISurveyAnswerSaver
    {
        private ILogger<SurveyAnswerSaver> _logger;
        private readonly DataBaseFileUri _dataBaseFileUri;

        public SurveyAnswerSaver(ILogger<SurveyAnswerSaver> logger, IOptions<DataBaseFileUri> dataBaseFileUri)
        {
            _logger = logger;
            _dataBaseFileUri = dataBaseFileUri.Value;
        }

        public async Task SaveAnswerAsync(SurveyAnswer answer)
        {
            var path = Directory.GetCurrentDirectory() + _dataBaseFileUri.DataBaseWriteFileUri + answer.SurveyId + ".json";

            Dictionary<string, List<QuestionAnswerPair>> dataBase;

            if (!File.Exists(path))
            {
                File.Create(path).Close();
                dataBase = new Dictionary<string, List<QuestionAnswerPair>>();
            }
            else
            {
                var file = await File.ReadAllTextAsync(path);

                dataBase = JsonSerializer.Deserialize<Dictionary<string, List<QuestionAnswerPair>>>(file) ?? throw new Exception();
            }

            if (dataBase.ContainsKey(answer.UserEmail))
            {
                throw new UnauthorizedAccessException();
            }

            dataBase.Add(answer.UserEmail, answer.QuestionAnswerPairList);

            var jsonDataBase = JsonSerializer.Serialize(dataBase);

            File.WriteAllText(path, jsonDataBase);

            _logger.LogTrace("SaveAnswerAsync success");
        }
    }
}