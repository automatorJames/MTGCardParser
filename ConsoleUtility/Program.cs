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
        //TestSimple<ManaValue>("{12}{w}{w}{r}");
        var thing = TokenTypeRegistry.Tokenize("target creature gains flying until end of turn");
        //var thing = TokenTypeRegistry.Tokenize("target creature gains trample and gets +x/+0 until end of turn");
        var thingy = "sdaf";
        //TestTokenization();
    }

    static void TestSimple<T>(string tryToMatchText = null)
    {
        var testGraph = RegexGraph.Create(typeof(T));
        Console.WriteLine(testGraph.BuiltRegex.FormattedRegex);

        if (tryToMatchText != null)
        {
            testGraph.TryMatch(tryToMatchText, out TokenUnit result);
            Debugger.Break();
        }
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
