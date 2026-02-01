using System.Diagnostics;
using System.Reflection;

namespace ConsoleUtility;

internal class Program
{
    static void Main(string[] args)
    {
        var thing = TokenTypeRegistry.AppliedOrderTypes;
        var type = TokenTypeRegistry.NameToType["TargetGainsOrLosesBuff_Many"];
        //RootNode thingThatMakesYouSayReeeeaL = new(typeof(DestroyAllCardType));
        //var sourceText = "destroy all lands";
        //thingThatMakesYouSayReeeeaL.TryMatch(sourceText, out var hyrdrated);

        RootNode thingThatMakesYouSayReeeeaL = new(type);
        Debug.WriteLine(thingThatMakesYouSayReeeeaL.BuiltRegex.MinifiedRegexString);
        thingThatMakesYouSayReeeeaL.BuiltRegex.FormattedLines.Select(x => x.Regex.TrimEnd()).ToList().ForEach(x => Console.WriteLine(x));
        var sourceText = "target creature gains trample and gets +x/+0";
        thingThatMakesYouSayReeeeaL.TryMatch(sourceText, out var hyrdrated);

        //RootNode thereeeelmakeyousayreeeel = TokenTypeRegistry.GetRootNode(TokenTypeRegistry.NameToType["DestroyAllCardType"]);

        //thereeeelmakeyousayreeeel.BuiltRegex.FormattedLines.Select(x => x.Regex).ToList().ForEach(Console.WriteLine);
        //thereeeelmakeyousayreeeel.BuiltRegex.FormattedLines.Select(x => x.Regex.TrimEnd()).ToList().ForEach(x => Debug.WriteLine(x));
        Debug.WriteLine(thingThatMakesYouSayReeeeaL.BuiltRegex.MinifiedRegexString);
        //DynamicTokenType dynamicTokenType = new("destroy all @CardType Plural()", className: "DestroyAllCardType");
        //TokenTypeRegistry.CreateAndRegisterNewTypeAndSaveToDisk(dynamicTokenType);
    }
}
