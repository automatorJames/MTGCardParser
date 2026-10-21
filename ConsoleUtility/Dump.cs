using Glyphotype;
using Glyphotype.Definitions;
using Glyphotype.Distiller.Inspection;
using Glyphotype.GlyphAnalysisDTOs.SpanAnalysis;
using Glyphotype.Interfaces;
using Glyphotype.RegexGeneration.Graph;
using Glyphotype.StaticRegistry;

namespace ConsoleUtility;

/// <summary>
/// <c>dump out-path [definition.json|-] [every]</c>: every line of the corpus (or of every Nth document) as the grammar
/// tokenizes it - the compiled grammar's, or one built from a definition - written one line per corpus line: its nested
/// rendering, then every capture in every token (its group, where it sits, what it holds), so two engines' outputs
/// can be diffed capture for capture.
/// </summary>
static class Dump
{
    public static void Run(IDocumentRepository repository, string outPath, string definitionPath, int every)
    {
        var grammar = definitionPath is null or "-"
            ? GlyphGrammar.Default
            : GlyphGrammar.FromDefinition(GrammarDefinition.FromJson(File.ReadAllText(definitionPath)));

        var all = repository.GetDocumentsAsync().GetAwaiter().GetResult();
        var corpus = every > 1 ? all.Where((d, i) => i % every == 0).ToList() : all;
        var started = System.Diagnostics.Stopwatch.StartNew();
        var documents = corpus.AsParallel().AsOrdered().Select(d => new ProcessedDocument(d, grammar)).ToList();
        Console.WriteLine($"Tokenized {corpus.Count:N0} documents in {started.Elapsed.TotalSeconds:F1}s");

        using var writer = new StreamWriter(outPath);
        var lines = 0;

        foreach (var document in documents)
            foreach (var line in document.Lines)
            {
                writer.Write(document.Document.Name);
                writer.Write('\t');
                writer.Write(line.SourceText.LineIndex);
                writer.Write('\t');
                writer.Write(ParseRenderer.Render(line, nested: true));

                foreach (var unit in line.Glyphs)
                {
                    writer.Write('\t');
                    writer.Write(unit.Type.Name);
                    writer.Write('@');
                    writer.Write(unit.CaptureContext.RootCaptureTrace.Index);
                    writer.Write('+');
                    writer.Write(unit.CaptureContext.RootCaptureTrace.Length);

                    foreach (var trace in unit.CaptureContext.RootCaptureTrace.GetFlatCaptureTree().Values.OrderBy(x => x.FullyQualifiedName, StringComparer.Ordinal))
                        foreach (var occurrence in trace.Success ? trace.AsEnumerable() : [])
                        {
                            writer.Write(' ');
                            writer.Write(occurrence.FullyQualifiedName);
                            writer.Write('@');
                            writer.Write(occurrence.Index);
                            writer.Write('+');
                            writer.Write(occurrence.Length);
                            writer.Write('=');
                            writer.Write(occurrence.ResolvedNodeTypeName ?? "");
                        }
                }

                writer.WriteLine();
                lines++;
            }

        Console.WriteLine($"Wrote {lines:N0} lines to {outPath}");
    }
}
