using MTGPlexer.Data;
using MTGPlexer.RegexGeneration.Graph;
using MTGPlexer.TokenUnitPrimitives;
using MTGPlexer.TokenUnits;
using System.Diagnostics;

namespace ConsoleUtility;

internal class Program
{
    static void Main(string[] args)
    {
        TestSimple();
        TestTokenization();
    }

    static void TestSimple()
    {
        var testGraph = RegexGraph.Create(typeof(TestClass));
        Console.WriteLine(testGraph.BuiltRegex.FormattedRegex);
        var text = "target b c d";
        testGraph.TryMatch(text, out TokenUnit result);
    }

    static void TestTokenization()
    {
        CardDataGetter cardDataGetter = new("Server=localhost;Database=Magic;Integrated Security=True;MultipleActiveResultSets=True;Command Timeout=3600;TrustServerCertificate=True");

        List<TokenUnit> tokens = [];

        foreach (var card in cardDataGetter.GetCardsAsync().Result)
            foreach (var line in card.FormattedLinesLower)
                tokens.AddRange(TokenTypeRegistry.ClassTokenizer.Tokenize(line));

        Debugger.Break();
    }
}
