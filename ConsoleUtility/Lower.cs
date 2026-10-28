using Glyphotype.Interfaces;
using MTGGlyphs.Data;
using MTGRulesIR.Lowering;
using MTGRulesIR.Model;

namespace ConsoleUtility;

/// <summary>
/// <c>lower out-dir [every]</c>: the corpus (or every Nth document) tokenized and lowered to the rules IR, written as
/// <c>rules.jsonl</c> (one card's <see cref="CardRules"/> per line) and <c>report.txt</c> (what stayed opaque, by
/// glyph type), with the headline numbers printed.
/// <para>
/// <c>lowertext "text" [types]</c>: one line of card text (with <c>{this}</c> for the card's name) lowered and printed
/// as indented JSON, for looking at what a construction becomes.
/// </para>
/// </summary>
static class Lower
{
    public static void Run(IDocumentRepository repository, string outDir, int every)
    {
        var grammar = GlyphGrammar.Default;
        var all = repository.GetDocumentsAsync().GetAwaiter().GetResult();
        var corpus = every > 1 ? all.Where((d, i) => i % every == 0).ToList() : all;
        var started = System.Diagnostics.Stopwatch.StartNew();
        var documents = corpus.AsParallel().AsOrdered().Select(d => new ProcessedDocument(d, grammar)).ToList();
        Console.WriteLine($"Tokenized {corpus.Count:N0} documents in {started.Elapsed.TotalSeconds:F1}s");

        started.Restart();
        var lowering = new CorpusLowering();
        var cards = documents.AsParallel().AsOrdered().Select(lowering.Lower).ToList();
        Console.WriteLine($"Lowered {cards.Count:N0} documents in {started.Elapsed.TotalSeconds:F1}s");

        Directory.CreateDirectory(outDir);
        var report = new LoweringReport();

        using (var writer = new StreamWriter(Path.Combine(outDir, "rules.jsonl")))
            foreach (var card in cards)
            {
                report.Add(card);
                writer.WriteLine(IrJson.Serialize(card));
            }

        var text = report.ToText();
        File.WriteAllText(Path.Combine(outDir, "report.txt"), text);
        Console.WriteLine(text);
    }

    public static void RunText(string text, string types)
    {
        var card = new Card { Name = "Sample", Text = text, Types = types ?? "Creature" };
        var document = new ProcessedDocument(card, GlyphGrammar.Default);
        var rules = new CorpusLowering().Lower(document);
        Console.WriteLine(IrJson.Serialize(rules, indented: true));
    }
}
