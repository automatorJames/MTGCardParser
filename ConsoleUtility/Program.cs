using MTGPlexer.RegexGeneration.Graph;
using MTGPlexer.TokenUnitPrimitives;
using MTGPlexer.TokenUnits;

namespace ConsoleUtility;

internal class Program
{
    static void Main(string[] args)
    {
        var testGraph = RegexGraph.Create(typeof(TestClass));
        Console.WriteLine(testGraph.BuiltRegex.FormattedRegex);

        var text = "target c";

        var things = TokenTypeRegistry.Tokenize(text);

        testGraph.TryMatch(text, out TokenUnit result);
    }
}
