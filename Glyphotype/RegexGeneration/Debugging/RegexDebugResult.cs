namespace Glyphotype.RegexGeneration.Debugging;

/// <summary>
/// The outcome of <see cref="RegexMatchDebugger.Analyze"/> testing one <see cref="RegexGraph"/> against a
/// user-highlighted text segment: how far the graph's longest-matching "stemmed permutation" (a prefix of
/// the graph's meaningful units, with every still-open group closed off so it compiles) got before its
/// first non-matching unit, plus the scoring that ranks graphs against each other in the debug dialog.
/// Identifies the failure by the actual <see cref="RegexBrick"/> it occurred on, so the precise line in
/// the graph's formatted rendering can be resolved later (see <see cref="RegexDebugRenderer"/>) without
/// this DTO having to know anything about display formatting.
/// </summary>
public class RegexDebugResult
{
    /// <summary>The root <see cref="Glyph"/> type whose graph was analyzed.</summary>
    public Type GlyphType { get; init; }

    /// <summary>The analyzed graph itself.</summary>
    public RegexGraph Graph { get; init; }

    /// <summary>The (word-boundary-corrected) highlighted text the graph was tested against.</summary>
    public string TextSegment { get; init; }

    /// <summary>Whether the complete graph matched from the start of <see cref="TextSegment"/> (possibly stopping before its end).</summary>
    public bool IsFullMatch { get; init; }

    /// <summary>How many characters of <see cref="TextSegment"/> the most successful stemmed permutation matched.</summary>
    public int MatchedCharCount { get; init; }

    /// <summary>How many whole words of <see cref="TextSegment"/> fall entirely within <see cref="MatchedCharCount"/>.</summary>
    public int MatchedWordCount { get; init; }

    /// <summary>Total word count of <see cref="TextSegment"/>.</summary>
    public int TotalWordCount { get; init; }

    /// <summary>The percentage (0-100) of <see cref="TextSegment"/>'s words covered by the most successful stemmed permutation — the dialog's "Match Score".</summary>
    public double MatchScorePercent =>
        TotalWordCount == 0 ? 0 : 100.0 * MatchedWordCount / TotalWordCount;

    /// <summary>How many meaningful units (literal lines, joiners, atomic named groups) matched before the first failure.</summary>
    public int MatchedUnitCount { get; init; }

    /// <summary>Total meaningful units in the whole graph, counted with the same unit decomposition the walk itself uses.</summary>
    public int TotalUnitCount { get; init; }

    /// <summary>The percentage (0-100) of the graph's meaningful units matched before failing.</summary>
    public double UnitMatchPercent =>
        TotalUnitCount == 0 ? 0 : 100.0 * MatchedUnitCount / TotalUnitCount;

    /// <summary>
    /// The identity of the first non-matching unit: a literal line's or joiner's own (escaped) regex text,
    /// or a named group's fully qualified name. Empty when <see cref="IsFullMatch"/>.
    /// </summary>
    public string FirstFailureDisplay { get; init; } = "";

    /// <summary>The failing named group's fully qualified name, when the failing unit was a named group; null otherwise.</summary>
    public string FirstFailureFullyQualifiedName { get; init; }

    /// <summary>
    /// The graph brick the first failure anchors to — a literal/joiner brick, or a failing group's open
    /// bookend — used to resolve the failing line number within the formatted regex (these brick objects
    /// are the same shared instances the formatting pipeline renders). Null when <see cref="IsFullMatch"/>.
    /// </summary>
    public RegexBrick FirstFailureBrick { get; init; }

    /// <summary>
    /// The most successful stemmed permutation, as bricks: the committed prefix of the graph's own brick
    /// sequence, followed by the real close bookend of every group still open at the cut (innermost first),
    /// so the sequence is a complete, renderable regex in its own right.
    /// </summary>
    public List<RegexBrick> MaxMatchStemBricks { get; init; } = [];

    /// <summary>The compiled (unescaped) pattern text of <see cref="MaxMatchStemBricks"/>' stem — for copying or ad-hoc verification.</summary>
    public string MaxMatchStemRegex { get; init; } = "";

    public override string ToString() =>
        $"{GlyphType?.Name}: {MatchScorePercent:0.#}% ({(IsFullMatch ? "full match" : FirstFailureDisplay)})";
}
