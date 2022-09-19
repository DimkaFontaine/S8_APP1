
namespace SondageApi.Services;
public interface IFileWrapper
{
    bool Exists(string file);
    void Create(string file);
    Task<string> ReadAllTextAsync(string file);
    void WriteAllText(string file, string json);
    string ReadAllText(string file);
    string[] GetFiles(string path);
}