using System.Globalization;

namespace SondageApi.Models;

public class Answer
{
    public short Id {get; set;} = 0;
    public string Text { get; set; } = string.Empty;
}
