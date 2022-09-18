
namespace SondageApi.Services;
public interface IFileWrapper
{
    bool Exists(string file);
    void Create(string file);
    Task<string> ReadAllTextAsync(string file);
    void WriteAllText(string file, string json);
}