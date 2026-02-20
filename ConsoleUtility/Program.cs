using MTGPlexer.RegexGeneration.Graph;
using MTGPlexer.TokenUnitPrimitives;
using MTGPlexer.TokenUnits;

namespace ConsoleUtility;

internal class Program
{
    static void Main(string[] args)
    {
        var testGraph = RegexGraph.Create(typeof(ManaValue));
        Console.WriteLine(testGraph.BuiltRegex.FormattedRegex);

        //var text = "target a, b, c, and d";
        var text = "target b c d";

        testGraph.TryMatch(text, out TokenUnit result);
    }
}
