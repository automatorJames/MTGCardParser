using System.Diagnostics;
using System.Reflection;

namespace ConsoleUtility;

internal class Program
{
    static void Main(string[] args)
    {
        //var thing = TokenTypeRegistry.ClassTokenizer.Tokenize("target creature gains trample and gets +X/+0 until end of turn, where X is its power.");
        RootNode dummyshite = new(typeof(TargetGainsOrLosesBuff));

        RootNode thereeeelmakeyousayreeeel = TokenTypeRegistry.GetRootNode(TokenTypeRegistry.NameToType["GainOrLoseAbility"]);

        //thereeeelmakeyousayreeeel.BuiltRegex.FormattedLines.Select(x => x.Regex).ToList().ForEach(Console.WriteLine);
        //thereeeelmakeyousayreeeel.BuiltRegex.FormattedLines.Select(x => x.Regex.TrimEnd()).ToList().ForEach(x => Debug.WriteLine(x));
        Debug.WriteLine(thereeeelmakeyousayreeeel.BuiltRegex.MinifiedRegexString);
        Console.WriteLine(thereeeelmakeyousayreeeel.BuiltRegex.MinifiedRegexString);
        //DynamicTokenType dynamicTokenType = new("destroy all @CardType Plural()", className: "DestroyAllCardType");
        //TokenTypeRegistry.CreateAndRegisterNewTypeAndSaveToDisk(dynamicTokenType);
    }
}
