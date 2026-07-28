using Microsoft.Extensions.Configuration;
using MTGPlexer;
using MTGPlexer.Data;
using MTGPlexer.RegexGeneration.Graph;
using MTGPlexer.TokenAnalysisDTOs;
using MTGPlexer.TokenUnitPrimitives;
using MTGPlexer.TokenUnits;
using Newtonsoft.Json;
using System.Diagnostics;

namespace ConsoleUtility;

internal class Program
{
    static async Task Main(string[] args)
    {
        //TestSimple<ManaValue>("{12}{w}{w}{r}");
        //var thing = TokenTypeRegistry.Tokenize("target creature gains flying until end of turn");
        //var debug = thing[0].JsonDebug;
        //var captureTree = thing[0].CaptureContext.GetCaptureTree();
        await TestTokenizationAsync();

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

    static async Task TestTokenizationAsync()
    {
        IConfiguration conf = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .Build();

        var globalSettings = conf
            .GetSection(nameof(GlobalSettings))
            .Get<GlobalSettings>()
            ?? throw new InvalidOperationException("GlobalSettings is missing from appsettings.json.");

        CardDataGetter cardDataGetter = new(globalSettings);
        DocumentCorpusAnalyzer analyzer = new(cardDataGetter);
        await analyzer.EnsureInitializedAsync();

        List<TokenUnit> tokens = [];

        foreach (var card in cardDataGetter.GetDocumentsAsync().Result)
            foreach (var line in card.GetFormattedLines())
                tokens.AddRange(TokenTypeRegistry.ClassTokenizer.Tokenize(line));

        Debugger.Break();
    }
}
