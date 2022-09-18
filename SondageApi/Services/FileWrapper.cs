namespace SondageApi.Services;

public class FileWrapper : IFileWrapper
{
    public void Create(string file)
        => File.Create(file).Close();

    public bool Exists(string file)
        => File.Exists(file);
    

    public Task<string> ReadAllTextAsync(string file)
        => File.ReadAllTextAsync(file);

    public void WriteAllText(string file, string json)
        => File.WriteAllText(file, json);
}