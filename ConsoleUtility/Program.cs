using Glyphotype;
using Glyphotype.GlyphAnalysisDTOs;
using Glyphotype.Interfaces;
using MTGGlyphs.Data;

namespace ConsoleUtility;

internal class Program
{
    static CardDataGetter _cardDataGetter = new(GlobalSettings.Current);

    static void Main(string[] args)
    {
        switch (args.FirstOrDefault())
        {
            case "score":
                PrintMdlScore(args.ElementAtOrDefault(1));
                break;

            // bench [every] [dump-path]: measures the engine on every Nth card (10 by default), and dumps the sample's parses to a file if given.
            case "bench":
                Bench.Run(_cardDataGetter, int.TryParse(args.ElementAtOrDefault(1), out var every) ? every : 10, args.ElementAtOrDefault(2));
                break;

            // expansion [count]: the glyphs with the largest regexes, and the nested glyphs that make them so.
            case "expansion":
                Expansion.Run(int.TryParse(args.ElementAtOrDefault(1), out var count) ? count : 15);
                break;

            // regexes [path]: a fingerprint of every type's regex and bricks, for checking that a change to graph building builds the same.
            case "regexes":
                Fingerprint.Run(args.ElementAtOrDefault(1) ?? "regex-fingerprints.txt");
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