using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using Glyphotype;
using Glyphotype.Definitions;
using Glyphotype.Distiller.Inspection;
using Glyphotype.Distiller.Scoring;
using Glyphotype.Distiller.Workbench;
using Glyphotype.GlyphAnalysisDTOs.SpanAnalysis;
using Glyphotype.GlyphPrimitives;
using Glyphotype.Interfaces;
using Glyphotype.RegexGeneration.Graph;
using Glyphotype.StaticRegistry;
using Glyphotype.Tokenizers;

namespace ConsoleUtility;

/// <summary>
/// Measures the engine on the real grammar and a sample of the real corpus - what the app's startup, an agent's
/// evaluate step and the corpus's memory cost come to - and dumps a sample's parses so an engine change can be checked
/// for changing none of them. See PERFORMANCE-PLAN.md for the measurements this was written for.
/// </summary>
static class Bench
{
    /// <summary>
    /// <c>bench [every] [dump-path]</c>: every <paramref name="every"/>th card (10 by default), and, given a path, each
    /// line's nested rendering written there, one per line, for diffing against another engine's.
    /// </summary>
    public static void Run(IDocumentRepository repository, int every, string dumpPath)
    {
        Console.WriteLine($"Sample: every {every}th card. Cores: {Environment.ProcessorCount}");

        Section("Grammar build");
        var m0 = Mem();
        var grammar = Time("GlyphGrammar.Default (discover, build graphs, validate)", () => GlyphGrammar.Default);
        var m1 = Mem();
        Console.WriteLine($"Types {grammar.Types.Count}, top-level {grammar.TopLevelTypes.Count}, dependent {grammar.DependentTypes.Count}; heap after build {(m1 - m0) / 1e6:F0} MB");

        var graphs = grammar.TopLevelTypes.Select(GlyphTypeCache.GetRegexGraph).ToList();
        var allGraphs = grammar.Types.Select(GlyphTypeCache.GetRegexGraph).ToList();

        Section("Regex size (top-level types)");
        var sizes = graphs.Select(g => (g.RootGlyphType.Name, Len: g.BuiltRegex.MinifiedRegexLength, Groups: g.NamedGroupFlatGraph.Count, Any: g.StartChars.IsAny)).OrderByDescending(x => x.Len).ToList();
        Console.WriteLine($"Total regex chars (top-level): {sizes.Sum(x => (long)x.Len):N0}; all types: {allGraphs.Sum(g => (long)g.BuiltRegex.MinifiedRegexLength):N0}");
        Console.WriteLine($"Median {sizes[sizes.Count / 2].Len:N0}, p90 {sizes[sizes.Count / 10].Len:N0}, max {sizes[0].Len:N0}; StartChars=Any: {sizes.Count(x => x.Any)}");
        Console.WriteLine($"Named groups per top-level regex: avg {sizes.Average(x => x.Groups):F0}, max {sizes.Max(x => x.Groups)}");

        foreach (var x in sizes.Take(10))
            Console.WriteLine($"  {x.Len,9:N0} chars {x.Groups,5} groups  any={x.Any,-5} {x.Name}");

        var m2 = Mem();
        Time("Compile all regexes (every type)", () => { foreach (var g in allGraphs) g.BuiltRegex.WarmUp(); return 0; });
        Console.WriteLine($"Heap for compiled regexes: {(Mem() - m2) / 1e6:F0} MB");

        Section("Corpus");
        var all = Time("Load documents", () => repository.GetDocumentsAsync().GetAwaiter().GetResult());
        var sampleA = all.Where((d, i) => i % every == 0).ToList();
        var sampleB = all.Where((d, i) => i % every == every / 2).ToList();
        var linesA = sampleA.SelectMany(d => d.GetFormattedLines().Where(l => !string.IsNullOrWhiteSpace(l))).ToList();
        Console.WriteLine($"All documents {all.Count:N0}; sample A {sampleA.Count:N0} docs, {linesA.Count:N0} lines, {linesA.Sum(l => l.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length):N0} words");

        Section("Tokenize (parallel, as the app does)");
        var processed = Tokenize("Sample A", sampleA, grammar);
        var processedB = Tokenize("Sample B (its retained delta is the per-line cost alone)", sampleB, grammar);
        var lines = processed.SelectMany(d => d.Lines).ToList();
        Console.WriteLine($"Coverage {processed.Sum(d => d.CapturedWordCount) / (double)processed.Sum(d => d.WordCount):P1}; tokens/line {lines.Average(l => l.Glyphs.Count):F1}; positions tried/line {lines.Average(l => l.TokenizationTrace?.AttemptCount ?? 0):F1}, max {lines.Max(l => l.TokenizationTrace?.AttemptCount ?? 0)}");
        GC.KeepAlive(processedB);

        if (dumpPath is not null)
        {
            File.WriteAllLines(dumpPath, lines.Select(l => ParseRenderer.Render(l, nested: true)));
            Console.WriteLine($"Dumped {lines.Count:N0} parses to {dumpPath}");
        }

        Section("Serial per-line timing (warm)");
        var timings = new List<(double Ms, string Line)>();
        var stopwatch = new Stopwatch();

        foreach (var line in linesA)
        {
            stopwatch.Restart();
            grammar.Tokenize(line);
            stopwatch.Stop();
            timings.Add((stopwatch.Elapsed.TotalMilliseconds, line));
        }

        timings.Sort((a, b) => b.Ms.CompareTo(a.Ms));
        Console.WriteLine($"Serial total {timings.Sum(t => t.Ms) / 1000:F1}s; mean {timings.Average(t => t.Ms):F2} ms/line; median {timings[timings.Count / 2].Ms:F2}; p99 {timings[timings.Count / 100].Ms:F1}; max {timings[0].Ms:F0}");
        Console.WriteLine($"Slowest 1% of lines: {timings.Take(timings.Count / 100).Sum(t => t.Ms) / timings.Sum(t => t.Ms):P0} of the time");

        foreach (var t in timings.Take(5))
            Console.WriteLine($"  {t.Ms,7:F0} ms  {Cut(t.Line, 110)}");

        Section("Score");
        var score = Time("MdlScorer.Score (first: compiles pattern regexes)", () => MdlScorer.Score(grammar, processed));
        Time("MdlScorer.Score (again)", () => MdlScorer.Score(grammar, processed));
        Console.WriteLine($"Score {score.TotalBits:N0} bits, coverage {score.Coverage:P1}");

        Section("Evaluate step: one new glyph");
        GrammarEmitter.Reuse(grammar.Types);
        var definition = grammar.ToDefinition();
        var added = ChangeSet.FromSource("public class BenchNewGlyph : Glyph { public override Nib[] Nibs => [\"bench\", \"glyph\", \"zzz\"]; }", [], definition);
        EvaluateStep(grammar, processed, added.ApplyTo(definition));

        Section("Evaluate step: the most-referenced dependent glyph changes");
        var common = definition.Glyphs.Where(g => g.IsDependent).Select(g => (Glyph: g, Referrers: definition.Glyphs.Count(o => o.GetReferencedGlyphNames().Contains(g.Name)))).OrderByDescending(x => x.Referrers).First();
        Console.WriteLine($"{common.Glyph.Name} ({common.Referrers} direct referrers) gets a marker it didn't have");
        var tweaked = definition.WithGlyph(common.Glyph with { Markers = [.. common.Glyph.Markers, "IBenchMarker"] }) with { Markers = [.. definition.Markers, "IBenchMarker"] };
        EvaluateStep(grammar, processed, tweaked);

        Console.WriteLine();
        Console.WriteLine($"Heap at the end: {Mem() / 1e9:F2} GB");
    }

    static List<ProcessedDocument> Tokenize(string label, List<IDocument> documents, GlyphGrammar grammar)
    {
        var m0 = Mem();
        var allocated0 = GC.GetTotalAllocatedBytes(true);
        var gen2 = GC.CollectionCount(2);
        var processed = Time(label, () => documents.AsParallel().AsOrdered().Select(d => new ProcessedDocument(d, grammar)).ToList());
        var allocated1 = GC.GetTotalAllocatedBytes(true);
        var m1 = Mem();
        var lines = processed.Sum(d => d.Lines.Count);
        Console.WriteLine($"  {documents.Count:N0} docs, {lines:N0} lines; allocated {(allocated1 - allocated0) / 1e9:F1} GB, gen2 GCs {GC.CollectionCount(2) - gen2}; retained delta {(m1 - m0) / 1e6:F0} MB = {(m1 - m0) / (double)lines / 1e3:F1} KB/line");
        return processed;
    }

    static void EvaluateStep(GlyphGrammar basis, List<ProcessedDocument> processed, GrammarDefinition candidate)
    {
        var basisTypes = basis.Types.ToHashSet();
        var grammar = Time("Build grammar from definition", () => GlyphGrammar.FromDefinition(candidate.WithoutUnreferencedTerminals()));
        Console.WriteLine($"  types emitted (not reused from the basis): {grammar.Types.Count(t => !basisTypes.Contains(t))}");
        var change = TokenizationChange.Between(basis, grammar);
        var lines = processed.SelectMany(x => x.Lines).ToList();
        var mayDiffer = Time("TokenizationChange.MayDiffer over the sample", () => lines.Count(change.MayDiffer));
        Console.WriteLine($"  lines that may differ: {mayDiffer:N0} of {lines.Count:N0}");
        var retokenized = Time("Re-tokenize incrementally", () => processed.AsParallel().AsOrdered().Select(d => d.Retokenize(grammar, change)).ToList());
        Time("CompareTokenizations (the evaluate report)", () => CorpusQueries.CompareTokenizations(processed, retokenized, 8));
        var names = candidate.Glyphs.Select(x => x.Name).ToHashSet();
        Time("FindSteals", () => CorpusQueries.FindSteals(processed, retokenized, names, 2));
        Time("MdlScorer.Score", () => MdlScorer.Score(grammar, retokenized));
    }

    static void Section(string title)
    {
        Console.WriteLine();
        Console.WriteLine("=== " + title + " ===");
    }

    static T Time<T>(string label, Func<T> action)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = action();
        Console.WriteLine($"[{stopwatch.Elapsed.TotalSeconds,8:F2}s] {label}");
        return result;
    }

    static long Mem()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        return GC.GetTotalMemory(true);
    }

    static string Cut(string text, int length) => text.Length > length ? text[..length] + "..." : text;
}
