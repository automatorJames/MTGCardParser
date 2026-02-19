using MTGPlexer.RegexGeneration.Graph;
using MTGPlexer.TokenUnits;
using System.Diagnostics;

namespace ConsoleUtility;

internal class Program
{
    static void Main(string[] args)
    {
        var thing = RegexGraph.Create(typeof(TestClass));
        Console.WriteLine(thing.BuiltRegex.FormattedRegex);
    }
}
