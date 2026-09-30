using Glyphotype;
using Glyphotype.GlyphAnalysisDTOs;
using MTGGlyphs.Data;

namespace ConsoleUtility;

internal class Program
{
    static CardDataGetter _cardDataGetter = new(GlobalSettings.Current);

    static CorpusAnalyzer _analyzer = new(_cardDataGetter, GlyphGrammar.Default);

    static void Main(string[] args)
    {
        switch (args.FirstOrDefault())
        {
            case "score":
                PrintMdlScore(args.ElementAtOrDefault(1));
                break;

            default:
                PrintStructuralValidationErrors();
                break;
        }
    }

    /// <summary>
    /// Scores the MTG grammar against the card corpus (see <see cref="MdlScorer"/>): up to set sequence
    /// <paramref name="maxSetSequence"/>, or every set for "all", or appsettings' MaxSetSequence when omitted.
    /// </summary>
    static void PrintMdlScore(string maxSetSequence)
    {
        var settings = GlobalSettings.Current;
        var corpusSettings = new GlobalSettings
        {
            SqlConnString = settings.SqlConnString,
            IncludeEmptyDocuments = settings.IncludeEmptyDocuments,
            AllowPartialSegmentMatches = settings.AllowPartialSegmentMatches,
            MaxSetSequence = maxSetSequence switch
            {
                null => settings.MaxSetSequence,
                "all" => null,
                _ => int.Parse(maxSetSequence),
            },
        };

        // Tokenized directly rather than through CorpusAnalyzer, which also builds echo trees the score doesn't use.
        var documents = new CardDataGetter(corpusSettings).GetDocumentsAsync().GetAwaiter().GetResult()
            .AsParallel()
            .AsOrdered()
            .Select(x => new ProcessedDocument(x, GlyphGrammar.Default))
            .ToList();

        Console.Write(MdlScorer.Score(GlyphGrammar.Default, documents).ToReport());
    }

    static void PrintStructuralValidationErrors()
    {
        var errors = GlyphGrammar.Default.GetStructuralValidationErrors();

        if (errors.Count == 0)
        {
            Console.WriteLine("No structural validation errors found.");
            return;
        }

        Console.WriteLine($"{errors.Count} structural validation error(s):");

        foreach (var error in errors)
            Console.WriteLine($"- {error}");
    }

    //static void TestSmartLine()
    //{
    //    var glyphsByType = GetGlyphsByType();
    //
    //    foreach ((var type, var tokens) in glyphsByType)
    //    {
    //        GlyphOccurrenceSummary summary = new(type, tokens.Select(t => new MatchOccurrence(null, t)));
    //        var regexGraph = GlyphTypeCache.GetRegexGraph(type);
    //        var smartRegex = regexGraph.BuiltRegex.ToSmartRegex(summary, regexGraph);
    //        Console.WriteLine(smartRegex);
    //    }
    //}
    //
    //static List<string> GetLines()
    //{
    //    var cards = _cardDataGetter.GetDocumentsAsync().Result;
    //    return cards.SelectMany(x => x.GetFormattedLines()).ToList();
    //}
    //
    //static List<Glyph> GetGlyphs()
    //{
    //    List<Glyph> tokens = [];
    //    var lines = GetLines();
    //
    //    foreach (var line in lines)
    //        tokens.AddRange(GlyphGrammar.Default.Tokenize(line).OfType<Glyph>());
    //
    //    return tokens;
    //}
    //
    //static Dictionary<Type, List<Glyph>> GetGlyphsByType()
    //{
    //    List<Glyph> tokens = [];
    //    var lines = GetLines();
    //
    //    foreach (var line in lines)
    //        tokens.AddRange(GlyphGrammar.Default.Tokenize(line).OfType<Glyph>());
    //
    //    return tokens.GroupBy(x => x.Type).ToDictionary(x => x.Key, x => x.ToList());
    //}
    //
    //static void TestTokenization()
    //{
    //    var tokens = GetGlyphs();
    //    Debugger.Break();
    //}
    //
    //static void TestSummary()
    //{
    //    var glyphsByType = GetGlyphsByType();
    //
    //    foreach ((var type, var tokens) in glyphsByType)
    //    {
    //        GlyphOccurrenceSummary summary = new(type, tokens.Select(t => new MatchOccurrence(null, t)));
    //        Debugger.Break();
    //    }
    //}
}