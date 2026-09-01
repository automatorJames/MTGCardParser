using Glyphotype.GlyphAnalysisDTOs.TypeExpressions;
using Glyphotype.RegexGeneration.Debugging;
using Glyphotype.RegexGeneration.Graph.Bricks;
using MTGGlyphs.GlyphDefinitions;
using Xunit.Abstractions;

namespace Tests;

public class RegexDebuggerTests
{
    readonly ITestOutputHelper _output;

    public RegexDebuggerTests(ITestOutputHelper output) => _output = output;

    [Fact]
    public void AsLongAsThing_LocalizesFirstFailureAtAssertion()
    {
        // The walkthrough case: everything matches through EnchantedCardHasAspect's CardType (and the
        // space after it), then "didn't" fails to match the Assertion enum (is | isn't).
        var graph = GlyphTypeRegistry.GetRegexGraph(typeof(MTGGlyphs.GlyphDefinitions.AsLongAsThing));
        var text = "as long as enchanted artifact didn't attack this turn, it's an artifact creature";

        var result = RegexMatchDebugger.Analyze(graph, text);

        _output.WriteLine($"IsFullMatch: {result.IsFullMatch}");
        _output.WriteLine($"FirstFailure: {result.FirstFailureDisplay}");
        _output.WriteLine($"Matched words: {result.MatchedWordCount}/{result.TotalWordCount} ({result.MatchScorePercent:0.##}%)");
        _output.WriteLine($"Matched units: {result.MatchedUnitCount}/{result.TotalUnitCount}");
        _output.WriteLine($"Matched chars: {result.MatchedCharCount} -> \"{text[..result.MatchedCharCount]}\"");
        _output.WriteLine($"Stem regex: {result.MaxMatchStemRegex}");

        Assert.False(result.IsFullMatch);
        Assert.Equal("AsLongAsThing_EnchantedCardHasAspect_Assertion", result.FirstFailureFullyQualifiedName);
        Assert.Equal(5, result.MatchedWordCount); // "as long as enchanted artifact"
        Assert.Equal(13, result.TotalWordCount);

        // The failure brick is the Assertion group's open bookend — resolvable to a formatted line.
        Assert.IsType<RegexBrickGroupOpen>(result.FirstFailureBrick);

        // The stem itself is a valid regex that matches the segment's start.
        var stem = new System.Text.RegularExpressions.Regex(result.MaxMatchStemRegex, System.Text.RegularExpressions.RegexOptions.ExplicitCapture);
        var match = stem.Match(text);
        Assert.True(match.Success && match.Index == 0);
    }

    [Fact]
    public void AsLongAsThing_FullMatchReportsNoFailure()
    {
        var graph = GlyphTypeRegistry.GetRegexGraph(typeof(MTGGlyphs.GlyphDefinitions.AsLongAsThing));
        var text = "as long as enchanted creature is blue, it's an artifact";

        var result = RegexMatchDebugger.Analyze(graph, text);

        _output.WriteLine($"IsFullMatch: {result.IsFullMatch}, FirstFailure: '{result.FirstFailureDisplay}'");

        Assert.True(result.IsFullMatch);
        Assert.Equal("", result.FirstFailureDisplay);
        Assert.Null(result.FirstFailureBrick);
        Assert.Equal(result.TotalWordCount, result.MatchedWordCount);
    }

    [Fact]
    public void AsLongAsThing_RendersOriginalAndStemPreviews()
    {
        var graph = GlyphTypeRegistry.GetRegexGraph(typeof(MTGGlyphs.GlyphDefinitions.AsLongAsThing));
        var text = "as long as enchanted artifact didn't attack this turn, it's an artifact creature";
        var result = RegexMatchDebugger.Analyze(graph, text);
        var summary = new GlyphOccurrenceSummary(typeof(MTGGlyphs.GlyphDefinitions.AsLongAsThing));

        var (originalLines, failureLineIndex) = RegexDebugRenderer.RenderOriginal(result, summary);
        var stemLines = RegexDebugRenderer.RenderMaxMatchStem(result, summary);

        _output.WriteLine($"--- Original ({originalLines.Count} lines, failure at line index {failureLineIndex}) ---");
        for (int i = 0; i < originalLines.Count; i++)
            _output.WriteLine($"{(i == failureLineIndex ? ">" : " ")} {i + 1,3}  {originalLines[i]}");

        _output.WriteLine("--- Max match stem ---");
        foreach (var line in stemLines)
            _output.WriteLine(line.ToString());

        Assert.True(failureLineIndex >= 0);
        Assert.Contains("Assertion", originalLines[failureLineIndex].ToString());
        Assert.True(stemLines.Count > 0 && stemLines.Count < originalLines.Count);
    }

    [Fact]
    public void SegmentEndingMidGraph_ReportsJoinerFailureAsTheJoinerLine()
    {
        // The segment covers everything through the "," but ends there, so the space joiner before
        // ItGetsOrLosesBuff is the first unit that can't match — and the failure must identify that
        // joiner line itself, not the group that merely owns it.
        var graph = GlyphTypeRegistry.GetRegexGraph(typeof(MTGGlyphs.GlyphDefinitions.AsLongAsThing));
        var result = RegexMatchDebugger.Analyze(graph, "as long as enchanted artifact isn't a creature,");

        _output.WriteLine($"FirstFailure: {result.FirstFailureDisplay}");

        Assert.False(result.IsFullMatch);
        Assert.Equal("[ ]", result.FirstFailureDisplay);
        Assert.Null(result.FirstFailureFullyQualifiedName);
        Assert.Equal(result.TotalWordCount, result.MatchedWordCount); // every word still matched
    }

    [Fact]
    public void UnrelatedText_ScoresZero()
    {
        var graph = GlyphTypeRegistry.GetRegexGraph(typeof(MTGGlyphs.GlyphDefinitions.AsLongAsThing));
        var result = RegexMatchDebugger.Analyze(graph, "destroy target creature at end of turn");

        Assert.False(result.IsFullMatch);
        Assert.Equal(0, result.MatchedWordCount);
    }
}
