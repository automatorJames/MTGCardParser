using MTGPlexer.RegexGeneration.GraphNodes;
using System.Diagnostics;

namespace ConsoleUtility;

internal class Program
{
    static void Main(string[] args)
    {
        //var thing = TokenTypeRegistry.ClassTokenizer.Tokenize("target creature gains trample and gets +X/+0 until end of turn, where X is its power.");
        RootNode rootNode = new(typeof(TargetGainsOrLosesBuff));
        rootNode.BuiltRegex.FormattedLines.Select(x => x.Regex).ToList().ForEach(Console.WriteLine);
        //DynamicTokenType dynamicTokenType = new("destroy all @CardType Plural()", className: "DestroyAllCardType");
        //TokenTypeRegistry.CreateAndRegisterNewTypeAndSaveToDisk(dynamicTokenType);
    }
}
