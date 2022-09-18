using Microsoft.Extensions.Options;
using SondageApi.Models;
using System.Text.Json;

namespace SondageApi.Services
{
    public class SurveyAnswerSaver : ISurveyAnswerSaver
    {
        private ILogger<SurveyAnswerSaver> _logger;
        private readonly DataBaseFileUri _dataBaseFileUri;
        private readonly IFileWrapper _fileWrapper;

        public SurveyAnswerSaver(
            ILogger<SurveyAnswerSaver> logger, 
            IOptions<DataBaseFileUri> dataBaseFileUri,
            IFileWrapper fileWrapper)
        {
            _logger = logger;
            _dataBaseFileUri = dataBaseFileUri.Value;
            _fileWrapper = fileWrapper;
        }

        public async Task SaveAnswerAsync(SurveyAnswer answer)
        {
            var path = Directory.GetCurrentDirectory() + _dataBaseFileUri.DataBaseWriteFileUri + answer.SurveyId + ".json";

            Dictionary<string, List<QuestionAnswerPair>> dataBase;

            if (!_fileWrapper.Exists(path))
            {
                _fileWrapper.Create(path);
                dataBase = new Dictionary<string, List<QuestionAnswerPair>>();
            }
            else
            {
                var file = await _fileWrapper.ReadAllTextAsync(path);

                dataBase = JsonSerializer.Deserialize<Dictionary<string, List<QuestionAnswerPair>>>(file) ?? throw new Exception();
            }

            if (dataBase.ContainsKey(answer.UserEmail))
            {
                throw new UnauthorizedAccessException();
            }

            dataBase.Add(answer.UserEmail, answer.QuestionAnswerPairList);

            var jsonDataBase = JsonSerializer.Serialize(dataBase);

            _fileWrapper.WriteAllText(path, jsonDataBase);

            _logger.LogTrace("SaveAnswerAsync success");
        }
    }
}