using MTGPlexer.RegexGeneration.Graph;
using MTGPlexer.TokenUnits;
using System.Diagnostics;

namespace ConsoleUtility;

internal class Program
{
    static void Main(string[] args)
    {
        var thing = TokenTypeRegistry.AppliedOrderTypes;
        //var type = TokenTypeRegistry.NameToType["TargetGainsOrLosesBuff_Many"];
        var thingThatMakesYouSayReeeeaL = RegexGraph.Create(typeof(TestClass));
        Console.WriteLine(string.Join(Environment.NewLine, thingThatMakesYouSayReeeeaL.BuiltRegex.FormattedLines));
        //var sourceText = "destroy all lands";
        //thingThatMakesYouSayReeeeaL.TryMatch(sourceText, out var hyrdrated);


        //RootNode thereeeelmakeyousayreeeel = TokenTypeRegistry.GetRootNode(TokenTypeRegistry.NameToType["DestroyAllCardType"]);

        //thereeeelmakeyousayreeeel.BuiltRegex.FormattedLines.Select(x => x.Regex).ToList().ForEach(Console.WriteLine);
        //thereeeelmakeyousayreeeel.BuiltRegex.FormattedLines.Select(x => x.Regex.TrimEnd()).ToList().ForEach(x => Debug.WriteLine(x));
        //Debug.WriteLine(thingThatMakesYouSayReeeeaL.BuiltRegex.MinifiedRegexString);
        //DynamicTokenType dynamicTokenType = new("destroy all @CardType Plural()", className: "DestroyAllCardType");
        //TokenTypeRegistry.CreateAndRegisterNewTypeAndSaveToDisk(dynamicTokenType);
    }
}
