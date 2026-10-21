using Glyphotype.Definitions;
using Glyphotype.Distiller.Inspection;
using Glyphotype.RegexGeneration.Graph;
using Glyphotype.RegexGeneration.Matching;
using Glyphotype.Tests.Corpus;
using Glyphotype.Tests.Infrastructure;

namespace Glyphotype.Tests;

/// <summary>
/// A pattern's plan (see <see cref="MatchPlan"/>) finds the very matches its flattened regex finds. Checked with every
/// nested glyph matched by a call - the threshold set to nothing - so the test grammar, whose glyphs are small, exercises
/// the calls, their quantifiers, the alternations around them and the backtracking into the leaves between them.
/// </summary>
[Collection(CorpusCollection.Name)]
public class MatchPlanTests(CorpusFixture corpus)
{
    /// <summary>The test grammar built afresh with every nested glyph called.</summary>
    static readonly Lazy<GlyphGrammar> _called = new(() =>
    {
        var threshold = MatchPlan.CallThreshold;
        MatchPlan.CallThreshold = 1;

        try
        {
            return GlyphGrammar.FromDefinition(GlyphGrammar.FromAssemblies([typeof(CorpusFixture).Assembly]).ToDefinition());
        }
        finally
        {
            MatchPlan.CallThreshold = threshold;
        }
    });

    [Fact]
    public void Every_type_at_every_clause_start_matches_as_its_flattened_regex_does()
    {
        var grammar = _called.Value;
        var tried = 0;
        var matched = 0;
        var calls = grammar.TopLevelTypes.Count(x => GlyphTypeCache.GetRegexGraph(x).BuiltRegex.Plan.Root is not PlanNode.Leaf);
        Assert.True(calls > 0, "No top-level type's plan has a call: the threshold didn't take");

        foreach (var document in TestCorpus.Documents)
            foreach (var line in ((IDocument)document).GetFormattedLines().Where(x => !string.IsNullOrWhiteSpace(x)))
                foreach (var (start, end) in ClauseWindows(line))
                    foreach (var type in grammar.TopLevelTypes.Concat(grammar.DependentTypes))
                    {
                        var graph = GlyphTypeCache.GetRegexGraph(type);

                        foreach (var whole in new[] { true, false })
                        {
                            var byRegex = graph.TryMatchRegex(line, start, end, end, whole, out var regexMatch);
                            var byPlan = graph.TryMatchPlan(line, start, end, end, whole, out var planMatch);
                            tried++;

                            Assert.True(byRegex == byPlan, $"{type.Name} at {start} of \"{line}\" (whole: {whole}): regex {byRegex}, plan {byPlan}");

                            if (!byRegex)
                                continue;

                            matched++;
                            Assert.True(regexMatch.Index + regexMatch.Length == planMatch.End, $"{type.Name} at {start} of \"{line}\": regex ends at {regexMatch.Index + regexMatch.Length}, plan at {planMatch.End}");
                        }
                    }

        Assert.True(matched > 0, $"Nothing matched in {tried} tries");
    }

    [Fact]
    public void The_corpus_tokenizes_the_same_with_every_nested_glyph_called()
    {
        var expected = corpus.ProcessedDocuments.SelectMany(x => x.Lines).Select(Signature).ToList();
        var actual = CorpusFixture.Process(_called.Value).SelectMany(x => x.Lines).Select(Signature).ToList();

        Assert.Equal(expected, actual);
    }

    /// <summary>A line's parse, capture by capture - what the engines' outputs are diffed on.</summary>
    static string Signature(ProcessedLine line) =>
        ParseRenderer.Render(line, nested: true) + " | " + string.Join(" ", line.Glyphs.Select(unit =>
            $"{unit.Type.Name}@{unit.CaptureContext.RootCaptureTrace.Index}+{unit.CaptureContext.RootCaptureTrace.Length}"
            + string.Concat(unit.CaptureContext.RootCaptureTrace.GetFlatCaptureTree().Values
                .OrderBy(x => x.FullyQualifiedName, StringComparer.Ordinal)
                .SelectMany(x => x.Success ? x.AsEnumerable() : [])
                .Select(x => $" {x.FullyQualifiedName}@{x.Index}+{x.Length}={x.ResolvedNodeTypeName}"))));

    /// <summary>Each clause of <paramref name="line"/> as a window: where the tokenizer offers a type a match.</summary>
    static IEnumerable<(int Start, int End)> ClauseWindows(string line)
    {
        var start = 0;

        while (start < line.Length)
        {
            var period = line.IndexOf('.', start);
            var end = period < 0 ? line.Length : period;

            if (end > start)
                yield return (start, end);

            start = end + 1;

            while (start < line.Length && line[start] == ' ')
                start++;
        }
    }
}
