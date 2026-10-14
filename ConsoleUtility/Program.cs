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
}