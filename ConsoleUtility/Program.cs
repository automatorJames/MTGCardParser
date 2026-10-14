using Glyphotype;
using Glyphotype.GlyphAnalysisDTOs;
using Glyphotype.Interfaces;
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
    /// Scores the MTG grammar against the card corpus (see <see cref="MdlScorer"/>): the sets <paramref name="volumes"/> names by set
    /// sequence - a range ("1-5") or a list ("1,3,7") - or every set when omitted.
    /// </summary>
    static void PrintMdlScore(string volumes)
    {
        var selection = volumes switch
        {
            null => VolumeSelection.All,
            _ when volumes.Split('-') is [var min, var max] => VolumeSelection.Range(int.Parse(min), int.Parse(max)),
            _ => VolumeSelection.Only(volumes.Split(',').Select(int.Parse).ToArray()),
        };

        // Tokenized directly rather than through CorpusAnalyzer, which also builds echo trees the score doesn't use.
        var documents = selection.GetDocumentsAsync(_cardDataGetter).GetAwaiter().GetResult()
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