using Glyphotype.RegexGeneration.Graph;
using Glyphotype.StaticRegistry;

namespace ConsoleUtility;

/// <summary><c>check TypeName "text"</c>: matches the text with the type's plan and with its flattened regex at every clause start, and prints where they differ - the end, or the captures.</summary>
static class MatchCheck
{
    public static void Run(string typeName, string text, Glyphotype.Interfaces.IDocumentRepository repository = null, int warmDocuments = 0)
    {
        var grammar = GlyphGrammar.Default;

        // Tokenizing other documents first, on this thread, so whatever matching keeps between lines is in play.
        if (warmDocuments > 0)
        {
            var documents = repository.GetDocumentsAsync().GetAwaiter().GetResult().Take(warmDocuments).ToList();
            var started = System.Diagnostics.Stopwatch.StartNew();
            foreach (var document in documents)
                _ = new Glyphotype.GlyphAnalysisDTOs.SpanAnalysis.ProcessedDocument(document, grammar);
            Console.WriteLine($"warmed on {documents.Count} documents in {started.Elapsed.TotalSeconds:F1}s");
        }
        var type = grammar.Types.FirstOrDefault(x => x.Name == typeName) ?? throw new ArgumentException($"No type named {typeName}");
        var graph = GlyphTypeCache.GetRegexGraph(type);
        var start = 0;

        // As the tokenizer makes it, which tries every type in order and shares what nested glyphs matched where.
        var units = grammar.Tokenize(text);
        Console.WriteLine("tokenized: " + Glyphotype.Distiller.Inspection.ParseRenderer.Render(units, nested: true));
        Console.WriteLine("  captures: " + string.Join(" ", units.OfType<Glyphotype.GlyphPrimitives.Glyph>().SelectMany(x => Traces(x.CaptureContext))));

        while (start < text.Length)
        {
            var period = text.IndexOf('.', start);
            var end = period < 0 ? text.Length : period;

            foreach (var whole in new[] { true, false })
            {
                var byRegex = graph.TryMatchRegex(text, start, end, end, whole, out var regexMatch);
                var byPlan = graph.TryMatchPlan(text, start, end, end, whole, out var planMatch);
                Console.WriteLine($"at {start} (whole {whole}): regex {(byRegex ? $"ends {regexMatch.Index + regexMatch.Length}" : "no")}, plan {(byPlan ? $"ends {planMatch.End}" : "no")}");

                if (byRegex && byPlan)
                {
                    var regexContext = new CaptureContext(graph.RootNode, regexMatch, text, grammar.Tokenizer);
                    var planContext = new CaptureContext(graph.RootNode, text, start, planMatch.End - start, planMatch.Captures(), grammar.Tokenizer);
                    graph.RootNode.TryHydrate(regexContext.RootCaptureTrace, out var regexGlyph);
                    graph.RootNode.TryHydrate(planContext.RootCaptureTrace, out var planGlyph);
                    var regexTraces = Traces(regexContext);
                    var planTraces = Traces(planContext);
                    Console.WriteLine($"  hydrated: regex {regexGlyph is not null}, plan {planGlyph is not null}; captures: regex {regexTraces.Count}, plan {planTraces.Count}");

                    foreach (var trace in regexTraces.Except(planTraces))
                        Console.WriteLine($"  only regex: {trace}");

                    foreach (var trace in planTraces.Except(regexTraces))
                        Console.WriteLine($"  only plan:  {trace}");

                    Console.WriteLine($"  plan captures: {string.Join(" ", planMatch.Captures().Select(x => $"{x.Name ?? "pattern"}@{x.Index}+{x.Length}"))}");
                }
            }

            start = end + 1;

            while (start < text.Length && text[start] == ' ')
                start++;
        }
    }

    static List<string> Traces(CaptureContext context) =>
        context.RootCaptureTrace.GetFlatCaptureTree().Values
            .SelectMany(x => x.Success ? x.AsEnumerable() : [])
            .Select(x => $"{x.FullyQualifiedName}@{x.Index}+{x.Length}={x.ResolvedNodeTypeName}")
            .OrderBy(x => x, StringComparer.Ordinal)
            .ToList();
}
