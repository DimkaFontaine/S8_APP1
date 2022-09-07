using Microsoft.Extensions.Options;
using SondageApi.Models;

namespace SondageApi.Services;

public class TextDataBase : ITextDataBase
{
    private ILogger<TextDataBase> _logger;
    private DataBaseFileUri _dataBaseFileUri;
    private readonly object _lock = new object();

    public TextDataBase(ILogger<TextDataBase> logger, IOptions<DataBaseFileUri> dataBaseFileUri)
    {
        _logger = logger;
        _dataBaseFileUri = dataBaseFileUri.Value;

    }

    public void SaveEntries(string[] entries)
        => SaveEntry(entries.Aggregate((current, next)=> current+", "+ next));

    public void SaveEntry(string entry)
    {
        lock(_lock)
        {
            File.AppendAllText(_dataBaseFileUri.uri, entry + Environment.NewLine);
        }
        _logger.LogInformation(entry);
    }
}