using Microsoft.Extensions.Configuration;
using Glyphotype;
using MTGGlyphs.Data;
using Glyphotype.RegexGeneration.Graph;
using Glyphotype.StaticRegistry;
using Glyphotype.GlyphAnalysisDTOs;
using Glyphotype.GlyphAnalysisDTOs.TypeExpressions;
using Glyphotype.GlyphPrimitives;
using MTGGlyphs;
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

    static CorpusAnalyzer _analyzer = new(_cardDataGetter);

    static void Main(string[] args)
    {
        PrintStructuralValidationErrors();
    }

    static void PrintStructuralValidationErrors()
    {
        var errors = GlyphTypeRegistry.GetStructuralValidationErrors();

        if (errors.Count == 0)
        {
            Console.WriteLine("No structural validation errors found.");
            return;
        }

        Console.WriteLine($"{errors.Count} structural validation error(s):");

        foreach (var error in errors)
            Console.WriteLine($"- {error}");
    }

    static void TestSmartLine()
    {
        var glyphsByType = GetGlyphsByType();

        foreach ((var type, var tokens) in glyphsByType)
        {
            GlyphOccurrenceSummary summary = new(type, tokens.Select(t => new MatchOccurrence(null, t)));
            var regexGraph = GlyphTypeRegistry.RegexGraphs[type];
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
    //        testGraph.TryMatch(tryToMatchText, out Glyph result);
    //        Debugger.Break();
    //    }
    //}

    static List<string> GetLines()
    {
        var cards = _cardDataGetter.GetDocumentsAsync().Result;
        return cards.SelectMany(x => x.GetFormattedLines()).ToList();
    }

    static List<Glyph> GetGlyphs()
    {
        List<Glyph> tokens = [];
        var lines = GetLines();

        foreach (var line in lines)
            tokens.AddRange(GlyphTypeRegistry.ClassTokenizer.Tokenize(line).OfType<Glyph>());

        return tokens;
    }

    static Dictionary<Type, List<Glyph>> GetGlyphsByType()
    {
        List<Glyph> tokens = [];
        var lines = GetLines();

        foreach (var line in lines)
            tokens.AddRange(GlyphTypeRegistry.ClassTokenizer.Tokenize(line).OfType<Glyph>());

        return tokens.GroupBy(x => x.Type).ToDictionary(x => x.Key, x => x.ToList());
    }

    static void TestTokenization()
    {
        var tokens = GetGlyphs();
        Debugger.Break();
    }

    static void TestSummary()
    {
        var glyphsByType = GetGlyphsByType();

        foreach ((var type, var tokens) in glyphsByType)
        {
            GlyphOccurrenceSummary summary = new(type, tokens.Select(t => new MatchOccurrence(null, t)));
            Debugger.Break();
        }
    }
}