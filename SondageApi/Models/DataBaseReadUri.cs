namespace SondageApi.Models;
public class DataBaseFileUri
{
    public string DataBaseReadFileUri { get; set; } = string.Empty;
    public string DataBaseWriteFileUri { get; set; } = string.Empty;
    public const string Section = "DataBaseFileUri";
}