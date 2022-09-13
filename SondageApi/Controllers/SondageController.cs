using Microsoft.AspNetCore.Mvc;
using SondageApi.Models;
using System.Reflection;

namespace SondageApi.Controllers;

[ApiController]
[Route("[controller]")]
public class SondageController : ControllerBase
{
    private readonly ILogger<SondageController> _logger;

    public SondageController(ILogger<SondageController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetSondage")]
    public IEnumerable<SondageQuestion> Get()
    {
        string[] lines = System.IO.File.ReadAllLines(@"C:\Users\Chuck\source\repos\DimkaFontaine\S8_APP1\SondageApi\sondage.txt");
        string[] questions = new String[100];
        string[,] answers = new String[100, 100];
        int lineNum = 0;
        int questionNum = 0;
        int answerNum = 0;

        // Display the file contents by using a foreach loop.
        System.Console.WriteLine("Contents of sondage.txt = ");

        foreach (string line in lines)
        {
            findQuestions(ref questionNum, questions, line);
            findAnswers(questions, answers, questionNum, answerNum, line);
            lineNum++;
        }

        // Keep the console window open in debug mode.
        Console.WriteLine("Press any key to exit.");
        System.Console.ReadKey();


        return Enumerable.Range(0, lineNum)
            .Select(index => 
                new SondageQuestion("random question " + index, new[]{"a", "b", "c"}))
            .ToArray();
    }

    // Find each questions.
    private static void findQuestions(ref int questionNum, string[] questions, string line)
    {
        Console.WriteLine("\t" + line);
        if (line.Contains("?"))
        {
            foreach (char c in line)
            {
                while (c.Equals("?"))
                {
                    questions[questionNum++].Append(c);
                }
            }
        }
        if (questionNum > 0)
        {
            Console.WriteLine("\t" + "Found Question: " + questions[questionNum - 1]);
        }
    }

    // Find each answer
    private static void findAnswers(string[] questions, string[,] answers, int questionNum, int answerNum, string line)
    {
        if (questionNum > 0)
        {
            string txtAnswers = line.Replace(questions[questionNum], "");
            System.Console.WriteLine("Found answers: " + txtAnswers);
            foreach (char c in txtAnswers)
            {
                while (c.Equals(",")!)
                {
                    answers[questionNum, answerNum++].Append(c);
                }
            }
        }
    }
}
