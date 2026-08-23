namespace Glyphotype.RegexGeneration.Presentation;

/// <summary>
/// Renders one matched occurrence's captured text as a flat sequence of colored <see cref="MatchContentSpan"/>s,
/// for TypeRegexPage's "Matches" footer tray view. The match itself is split into leaf runs via
/// <see cref="CaptureTraceWalker"/> - the same tree walk <c>SpanView</c> uses to build DocumentLines' nested,
/// per-level underline spans, here flattened to a single span per run, colored by whichever named group is
/// most specific to it (its own color from <see cref="GetNamedGroupPalettes"/> if it has one, else the
/// nearest ancestor's) - the same per-named-group rainbow TypeRegexPage's formatted regex output colors that
/// group with, including named groups only reachable through a dynamic capture's resolved sub-type. Up to
/// <see cref="ContextWordCount"/> whole words of surrounding source text are added as plain grey context on
/// either side, ellipsis-truncated if more text exists beyond what's shown.
/// </summary>
public static class MatchContentRenderer
{
    /// <summary>How many whole words of surrounding source text to show, uncolored, on either side of a match.</summary>
    public const int ContextWordCount = 4;

    static readonly Regex _wordPattern = new(@"\S+", RegexOptions.Compiled);
    static readonly SpanStylePalette _contextPalette = SpanStylePalette.FromHexPalette(DeterministicPalette.GetStaticPalette(HexColor.DimGrey));

    /// <summary>
    /// The named-group palette to pass into <see cref="Build"/> for every match of <paramref name="summary"/>'s
    /// type: the same one <see cref="SmartLineRenderer"/> colors that type's own formatted regex with,
    /// including any named groups that only appear once a dynamic capture's resolved sub-type is expanded
    /// (see <see cref="RegexGraph.GetNamedGroupPaletteSet(IEnumerable{RegexBrick}, HexColor[])"/>) - computed
    /// against <see cref="RegexDisplayMode.MatchedOnly"/>'s bricks specifically, so the Matches view's
    /// coloring stays the richest, most-differentiated one available regardless of which display mode the
    /// card's own pre currently happens to show. Just <see cref="GetNamedGroupHexPalettes"/> converted to
    /// <see cref="SpanStylePalette"/>.
    /// </summary>
    public static IReadOnlyDictionary<NamedGroupNode, SpanStylePalette> GetNamedGroupPalettes(GlyphOccurrenceSummary summary) =>
        GetNamedGroupHexPalettes(summary).ToDictionary(x => x.Key, x => SpanStylePalette.FromHexPalette(x.Value));

    /// <summary>
    /// The raw <see cref="HexPalette"/> flavor of <see cref="GetNamedGroupPalettes"/> - the public entry
    /// point for any other caller (e.g. <c>CSharpClassView</c>, outside this assembly) that needs this same
    /// dynamic-resolution-expanded palette but, unlike the Matches view, isn't just going to hand every span's
    /// color straight to a <c>SpanStylePalette</c>-shaped consumer - <see cref="GlyphClassRenderer"/> resolves
    /// several of its own roles (a class header, a Nibs-array literal) from the raw hex first.
    /// </summary>
    public static Dictionary<NamedGroupNode, HexPalette> GetNamedGroupHexPalettes(GlyphOccurrenceSummary summary)
    {
        var pipeline = new RegexBrickFormattingPipeline(summary.RegexGraph, summary, RegexDisplayMode.MatchedOnly);
        var formattedBricks = pipeline.Format(summary.RegexGraph.BuiltRegex.Bricks);

        return summary.RegexGraph.GetNamedGroupPaletteSet(formattedBricks, HexColor.Silver);
    }

    public static List<MatchContentSpan> Build(RootCaptureTrace matchTrace, IReadOnlyDictionary<NamedGroupNode, SpanStylePalette> namedGroupPalettes)
    {
        List<MatchContentSpan> spans = [];

        if (BuildContextSpan(matchTrace, before: true) is { } leftContext)
            spans.Add(leftContext);

        spans.AddRange(BuildMatchSpans(matchTrace, matchTrace, namedGroupPalettes));

        if (BuildContextSpan(matchTrace, before: false) is { } rightContext)
            spans.Add(rightContext);

        return spans;
    }

    /// <summary>
    /// Walks <paramref name="trace"/>'s leaf runs via <see cref="CaptureTraceWalker"/>, coloring each by
    /// <paramref name="colorSource"/> - the deepest trace enclosing it - which becomes the child itself on
    /// every recursion, so a run's color is always its single most specific named group, never a blend of
    /// its ancestors'.
    /// </summary>
    static IEnumerable<MatchContentSpan> BuildMatchSpans(CaptureTrace trace, CaptureTrace colorSource, IReadOnlyDictionary<NamedGroupNode, SpanStylePalette> namedGroupPalettes)
    {
        foreach (var segment in CaptureTraceWalker.GetSegments(trace))
        {
            if (segment.Child is { } child)
            {
                foreach (var span in BuildMatchSpans(child, child, namedGroupPalettes))
                    yield return span;
            }
            else if (segment.Text.Length > 0)
            {
                yield return new(segment.Text, GetTargetFullyQualifiedName(colorSource), ResolvePalette(colorSource, namedGroupPalettes));
            }
        }
    }

    /// <summary>
    /// <paramref name="colorSource"/>'s FullyQualifiedName, extended with its captured enum member's own
    /// name (e.g. "EnchantedCard_Buff_Keyword_Protection") when it's an enum capture - matching exactly how
    /// <see cref="EnumMemberNode"/> names that member's own row in the formatted regex output, so
    /// hovering a matched enum value's word here highlights that specific member line there. Any other
    /// capture kind has no such finer-grained row of its own, so its plain FullyQualifiedName is used as-is.
    /// </summary>
    static string GetTargetFullyQualifiedName(CaptureTrace colorSource) =>
        colorSource.NodeKind == CaptureNodeKind.Enum && colorSource.ClrValue != null
            ? $"{colorSource.FullyQualifiedName}_{colorSource.ClrValue}"
            : colorSource.FullyQualifiedName;

    /// <summary>
    /// <paramref name="trace"/>'s own palette if <paramref name="namedGroupPalettes"/> has one, else its
    /// nearest ancestor's - walked via <see cref="CaptureTrace.ParentName"/> against the owning
    /// <see cref="RootCaptureTrace"/>'s flat lookup, the same way <see cref="RootCaptureTrace.AddCaptureTrace"/>
    /// resolves a trace's real parent. Looked up by <see cref="CaptureTrace.SourceNode"/> rather than
    /// <see cref="CaptureTrace.FullyQualifiedName"/> specifically because a dynamic capture's re-tokenized,
    /// rebased descendants (see <see cref="CaptureTrace.AdoptDynamicChildren"/>) get their FullyQualifiedName
    /// rewritten to read as part of the outer graph's path - a string <see cref="RegexGraph.NamedGroupFlatGraph"/>
    /// never declared - while SourceNode keeps pointing at the same, still-registered node object
    /// <see cref="GetNamedGroupPalettes"/> assigned a real color to.
    /// </summary>
    static SpanStylePalette ResolvePalette(CaptureTrace trace, IReadOnlyDictionary<NamedGroupNode, SpanStylePalette> namedGroupPalettes)
    {
        var root = trace.CaptureContext.RootCaptureTrace;

        for (var current = trace; current != null; current = current.ParentName is { } parentName ? root[parentName] : null)
            if (namedGroupPalettes.TryGetValue(current.SourceNode, out var palette))
                return palette;

        return _contextPalette;
    }

    /// <summary>
    /// Up to <see cref="ContextWordCount"/> whole words immediately <paramref name="before"/> (or after)
    /// <paramref name="matchTrace"/> within its own line of source text, closest words first/last as
    /// appropriate so the result reads contiguously - null if there are none. An ellipsis is added on the
    /// far (outer) edge when more words exist beyond what's shown, never on the edge touching the match. A
    /// single space is added on the edge touching the match instead, so the context doesn't visually abut
    /// the match's own colored text.
    /// </summary>
    static MatchContentSpan BuildContextSpan(RootCaptureTrace matchTrace, bool before)
    {
        var sourceText = matchTrace.CaptureContext.SourceText;
        var words = _wordPattern.Matches(sourceText);

        var candidateWords = (before
                ? words.Where(w => w.Index + w.Length <= matchTrace.Index)
                : words.Where(w => w.Index >= matchTrace.End))
            .ToList();

        if (candidateWords.Count == 0)
            return null;

        var takenWords = before
            ? candidateWords.TakeLast(ContextWordCount).ToList()
            : candidateWords.Take(ContextWordCount).ToList();

        var hasMoreBeyond = candidateWords.Count > takenWords.Count;
        var first = takenWords[0];
        var last = takenWords[^1];
        var text = sourceText.Substring(first.Index, last.Index + last.Length - first.Index);

        if (hasMoreBeyond)
            text = before ? "..." + text : text + "...";

        text = before ? text + " " : " " + text;

        return new(text, null, _contextPalette);
    }
}
