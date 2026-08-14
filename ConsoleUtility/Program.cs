using Microsoft.Extensions.Configuration;
using MTGPlexer;
using MTGPlexer.Data;
using MTGPlexer.RegexGeneration.Graph;
using MTGPlexer.TokenAnalysisDTOs;
using MTGPlexer.TokenAnalysisDTOs.TypeExpressions;
using MTGPlexer.TokenUnitPrimitives;
using MTGPlexer.TokenUnits;
using Newtonsoft.Json;
using System.Diagnostics;

namespace ConsoleUtility;

internal class Program
{
    static IConfiguration _conf = new ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
        .Build();

    static GlobalSettings _globalSettings = _conf
        .GetSection(nameof(GlobalSettings))
        .Get<GlobalSettings>()
        ?? throw new InvalidOperationException("GlobalSettings is missing from appsettings.json.");

    static CardDataGetter _cardDataGetter = new(_globalSettings);

    static DocumentCorpusAnalyzer _analyzer = new(_cardDataGetter);

    static void Main(string[] args)
    {
        TestSmartLine();
    }

    static void TestSmartLine()
    {
        var tokensByType = GetTokensByType();

        foreach ((var type, var tokens) in tokensByType)
        {
            TokenOccurrenceSummary summary = new(type, tokens);
            var regexGraph = TokenTypeRegistry.RegexGraphs[type];
            var smartRegex = regexGraph.BuiltRegex.ToSmartRegex(summary, regexGraph);
            Console.WriteLine(smartRegex);
        }
    }

    //static void TestSimple<T>(string tryToMatchText = null)
    //{
    //    var testGraph = RegexGraph.Create(typeof(T));
    //    Console.WriteLine(testGraph.BuiltRegex.FormattedRegex);
    //
    //    if (tryToMatchText != null)
    //    {
    //        testGraph.TryMatch(tryToMatchText, out TokenUnit result);
    //        Debugger.Break();
    //    }
    //}

    static List<string> GetLines()
    {
        var cards = _cardDataGetter.GetDocumentsAsync().Result;
        return cards.SelectMany(x => x.GetFormattedLines()).ToList();
    }

    static List<TokenUnit> GetTokens()
    {
        List<TokenUnit> tokens = [];
        var lines = GetLines();

        foreach (var line in lines)
            tokens.AddRange(TokenTypeRegistry.ClassTokenizer.Tokenize(line).OfType<TokenUnit>());

        return tokens;
    }

    static Dictionary<Type, List<TokenUnit>> GetTokensByType()
    {
        List<TokenUnit> tokens = [];
        var lines = GetLines();

        foreach (var line in lines)
            tokens.AddRange(TokenTypeRegistry.ClassTokenizer.Tokenize(line).OfType<TokenUnit>());

        return tokens.GroupBy(x => x.Type).ToDictionary(x => x.Key, x => x.ToList());
    }

    static void TestTokenization()
    {
        var tokens = GetTokens();
        Debugger.Break();
    }

    static void TestSummary()
    {
        var tokensByType = GetTokensByType();

        foreach ((var type, var tokens) in tokensByType)
        {
            TokenOccurrenceSummary summary = new(type, tokens);
            Debugger.Break();
        }
    }
}