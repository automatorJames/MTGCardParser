using System.Diagnostics;
using System.Text.Json;
using Glyphotype;
using Glyphotype.Definitions;
using Glyphotype.Distiller.Scoring;
using Glyphotype.GlyphPrimitives;
using Glyphotype.Distiller.Workbench;
using Glyphotype.GlyphAnalysisDTOs.SpanAnalysis;
using Glyphotype.Interfaces;
using Glyphotype.StaticRegistry;
using Glyphotype.Tokenizers;

namespace ConsoleUtility;

/// <summary>
/// Replays one step of a workspace's history as the workbench evaluates it - build the grammar, tokenize again what
/// may differ, score - against the whole corpus (or every Nth document), timing each part finely enough to say where
/// a step's seconds go. The step's "before" definition is the basis (built and tokenized whole first, as the app has
/// it after startup); its "after" is the next step's "before", or the working definition for the last step.
/// </summary>
static class Replay
{
    /// <summary><c>replay history.json step [every] [working.json]</c></summary>
    public static void Run(IDocumentRepository repository, string historyPath, int step, int every, string workingPath)
    {
        Console.WriteLine($"Cores: {Environment.ProcessorCount}; server GC: {System.Runtime.GCSettings.IsServerGC}; every {every}th document");

        using var history = JsonDocument.Parse(File.ReadAllText(historyPath));
        var steps = history.RootElement.EnumerateArray().ToList();
        var before = GrammarDefinition.FromJson(steps[step - 1].GetProperty("Before").GetRawText());
        var after = step < steps.Count
            ? GrammarDefinition.FromJson(steps[step].GetProperty("Before").GetRawText())
            : GrammarDefinition.FromJson(File.ReadAllText(workingPath ?? throw new ArgumentException("The last step needs the working definition's path")));

        Console.WriteLine($"Step {step}: {steps[step - 1].GetProperty("Description").GetString()}");
        Console.WriteLine($"  before: {before.Glyphs.Count} glyphs, {before.Vocabularies.Count} vocabularies; after: {after.Glyphs.Count} glyphs, {after.Vocabularies.Count} vocabularies");

        Section("Corpus");
        var all = Time("Load documents", () => repository.GetDocumentsAsync().GetAwaiter().GetResult());
        var corpus = every > 1 ? all.Where((d, i) => i % every == 0).ToList() : all;
        Console.WriteLine($"  {corpus.Count:N0} documents");

        Section("Basis grammar (the step's before)");
        var basis = BuildTimed(before, basisTypes: null);

        Section("Tokenize the corpus with the basis (whole, as at startup)");
        var documents = TokenizeTimed("ProcessedDocument x corpus (AsParallel)", corpus, basis);
        var lines = documents.SelectMany(x => x.Lines).ToList();
        Console.WriteLine($"  {lines.Count:N0} lines; attempts/line {lines.Average(l => l.TokenizationTrace?.AttemptCount ?? 0):F1}");
        Heap("after basis tokenization");

        Section("Score the basis");
        Time("MdlScorer.Score (first: compiles pattern regexes)", () => MdlScorer.Score(basis, documents));
        Time("MdlScorer.Score (again)", () => MdlScorer.Score(basis, documents));
        Heap("after basis scores");

        Section("Candidate grammar (the step's after) - what evaluate builds");
        var candidate = BuildTimed(after, basis.Types.ToHashSet());

        Section("Compile the candidate's new regexes (the app pays this lazily, inside tokenizing)");
        var basisSet = basis.Types.ToHashSet();
        var newTypes = candidate.Types.Where(x => !basisSet.Contains(x)).ToList();
        Time($"WarmUp {newTypes.Count} new types' anchored + scope-filling regexes", () => { foreach (var t in newTypes) GlyphTypeCache.GetRegexGraph(t).BuiltRegex.WarmUp(); return 0; });
        Heap("after compiling");

        Section("Incremental re-tokenization - what evaluate does after building");
        var change = Time("TokenizationChange.Between", () => TokenizationChange.Between(basis, candidate));
        Console.WriteLine($"  IsNone: {change.IsNone}");
        var mayDiffer = Time("MayDiffer over every line, serial (its CPU cost)", () => lines.Count(change.MayDiffer));
        Console.WriteLine($"  lines that may differ: {mayDiffer:N0} of {lines.Count:N0}");
        Time("MayDiffer over every line, parallel (as Retokenize runs it)", () => lines.AsParallel().Count(change.MayDiffer));
        var retokenized = TokenizeTimed("Retokenize (AsParallel over documents; includes MayDiffer again)", documents, candidate, change);
        var retokenizedLines = documents.Zip(retokenized).Sum(pair => ReferenceEquals(pair.First, pair.Second) ? 0 : pair.Second.Lines.Count(line => !pair.First.Lines.Contains(line)));
        Console.WriteLine($"  lines tokenized again: {retokenizedLines:N0}");
        Heap("after re-tokenization");

        Section("Whole re-tokenization with the candidate, for comparison");
        TokenizeTimed("ProcessedDocument x corpus (AsParallel), candidate", corpus, candidate);
        Heap("after whole re-tokenization");

        Section("Score the candidate");
        Time("MdlScorer.Score (first: compiles new pattern regexes)", () => MdlScorer.Score(candidate, retokenized));
        Time("MdlScorer.Score (again)", () => MdlScorer.Score(candidate, retokenized));

        Section("Refresh - what apply does to keep lines held with superseded types");
        var superseded = Time("IsSupersededBy over documents", () => retokenized.Count(x => x.IsSupersededBy(candidate)));
        Console.WriteLine($"  documents holding superseded types: {superseded:N0}");

        if (superseded > 0)
            Time("Refresh (AsParallel)", () => retokenized.AsParallel().AsOrdered().Select(x => x.Refresh(candidate)).ToList());

        Heap("at the end");
    }

    /// <summary>Builds a grammar from <paramref name="definition"/> as the workbench does, timing emission, graph building (per type, when there's a basis to tell new types by) and the rest of construction.</summary>
    static GlyphGrammar BuildTimed(GrammarDefinition definition, HashSet<Type> basisTypes)
    {
        var allocated0 = GC.GetTotalAllocatedBytes(true);
        var gen2 = GC.CollectionCount(2);
        var total = Stopwatch.StartNew();

        var trimmed = definition.WithoutUnreferencedTerminals();
        var types = Time("GrammarEmitter.Emit", () => GrammarEmitter.Emit(trimmed));
        var fresh = (basisTypes is null ? types : types.Where(x => !basisTypes.Contains(x))).Where(x => x.IsClass && !x.IsAbstract && typeof(Glyph).IsAssignableFrom(x) && !x.ContainsGenericParameters && !x.IsAssignableTo(typeof(DynamicGlyph))).ToList();
        Console.WriteLine($"  {types.Count} types emitted or reused, {fresh.Count} glyph types emitted afresh");

        // Graphs built here, in parallel as GlyphGrammar.Build does, so each type's own build time is seen - the
        // grammar's constructor then finds them cached.
        var timings = new System.Collections.Concurrent.ConcurrentBag<(Type Type, double Ms, int Length)>();

        Time($"Build {fresh.Count} graphs in parallel (GlyphTypeCache.GetRegexGraph)", () =>
        {
            Parallel.ForEach(fresh, type =>
            {
                var sw = Stopwatch.StartNew();
                var graph = GlyphTypeCache.GetRegexGraph(type);
                timings.Add((type, sw.Elapsed.TotalMilliseconds, graph.BuiltRegex.MinifiedRegex.Length));
            });

            return 0;
        });

        var ordered = timings.OrderByDescending(x => x.Ms).ToList();
        Console.WriteLine($"  sum of per-type graph build time: {ordered.Sum(x => x.Ms) / 1000:F1}s (CPU, across threads); regex chars built: {ordered.Sum(x => (long)x.Length):N0}");

        foreach (var (type, ms, length) in ordered.Take(12))
            Console.WriteLine($"    {ms,8:F0} ms {length,11:N0} chars  {type.Name}");

        var grammar = Time("new GlyphGrammar(types) (discover, validate, order; graphs already built)", () => new GlyphGrammar(types));
        Console.WriteLine($"  build total {total.Elapsed.TotalSeconds:F1}s; allocated {(GC.GetTotalAllocatedBytes(true) - allocated0) / 1e9:F1} GB; gen2 GCs {GC.CollectionCount(2) - gen2}");
        Heap("after build");
        return grammar;
    }

    static List<ProcessedDocument> TokenizeTimed(string label, List<IDocument> documents, GlyphGrammar grammar)
    {
        var allocated0 = GC.GetTotalAllocatedBytes(true);
        var gen2 = GC.CollectionCount(2);
        var processed = Time(label, () => documents.AsParallel().AsOrdered().Select(d => new ProcessedDocument(d, grammar)).ToList());
        Console.WriteLine($"  allocated {(GC.GetTotalAllocatedBytes(true) - allocated0) / 1e9:F1} GB; gen2 GCs {GC.CollectionCount(2) - gen2}");
        return processed;
    }

    static List<ProcessedDocument> TokenizeTimed(string label, List<ProcessedDocument> documents, GlyphGrammar grammar, TokenizationChange change)
    {
        var allocated0 = GC.GetTotalAllocatedBytes(true);
        var gen2 = GC.CollectionCount(2);
        var processed = Time(label, () => documents.AsParallel().AsOrdered().Select(d => d.Retokenize(grammar, change)).ToList());
        Console.WriteLine($"  allocated {(GC.GetTotalAllocatedBytes(true) - allocated0) / 1e9:F1} GB; gen2 GCs {GC.CollectionCount(2) - gen2}");
        return processed;
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

    static void Heap(string when)
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        Console.WriteLine($"  heap {when}: {GC.GetTotalMemory(true) / 1e9:F2} GB");
    }
}
