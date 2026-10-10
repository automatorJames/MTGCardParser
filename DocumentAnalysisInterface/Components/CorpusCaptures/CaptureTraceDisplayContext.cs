using DocumentAnalysisInterface.PresentationRules;
using Glyphotype.RegexGeneration.Graph;
using Glyphotype.GlyphAnalysisDTOs.WordTrees;

namespace DocumentAnalysisInterface.Components.CorpusCaptures;

/// <summary>
/// The per-line, per-viewer rendering facts that depend on <see cref="RuntimeSettings.HideCollapsibleCaptureNodes"/>:
/// which rainbow color each visible <see cref="CaptureTrace"/> gets, and how deep the visible
/// (non-collapsed) nesting actually goes. Built fresh by <c>DocumentBlock</c> on every render of a
/// line rather than cached on <see cref="ProcessedLine"/> itself, since that line is shared,
/// singleton-lifetime corpus state — baking a single viewer's collapse preference into it would
/// leak across every other viewer's session.
/// </summary>
public class CaptureTraceDisplayContext
{
    public IReadOnlyDictionary<CaptureTrace, HexPalette> Palettes { get; }

    /// <summary>The line's top-level captures, which a back-reference's path to its referent has to be told apart from.</summary>
    public IReadOnlyList<RootCaptureTrace> LineRoots { get; }

    readonly ProcessedLine _line;
    readonly RuntimeSettings _runtimeSettings;
    readonly DigestedText _echoCorpus;

    public CaptureTraceDisplayContext(ProcessedLine line, RuntimeSettings runtimeSettings, DigestedText echoCorpus)
    {
        _line = line;
        _runtimeSettings = runtimeSettings;
        _echoCorpus = echoCorpus;
        _clauseSpanning = GetClauseSpanning(line);

        // An embedded capture is grey rather than a color of its own (see CaptureDisplay.IsEmbedded), so it takes no
        // slot, and nor does one spanning its clause (see SpansClause).
        Palettes = line.GetPositionalPalettes(x => IsUnderlineHidden(x) || CaptureDisplay.IsEmbedded(x));
        LineRoots = line.CaptureTraceRoots;
    }

    readonly HashSet<CaptureTrace> _clauseSpanning;

    /// <summary>
    /// Whether <paramref name="trace"/> is the first level drawn of a clause's only capture - the clause captured whole
    /// by one top-level glyph, collapsed nodes hidden. Its underline would run under the whole clause, which every
    /// capture inside it already sits in, telling nothing - so it draws none and takes no palette slot, its table
    /// headed in <see cref="CaptureDisplay.ClauseColorStyle"/>.
    /// </summary>
    public bool SpansClause(CaptureTrace trace) => _clauseSpanning.Contains(trace);

    /// <summary>Whether <paramref name="trace"/> draws no underline of its own: collapsed, or spanning its clause.</summary>
    public bool IsUnderlineHidden(CaptureTrace trace) =>
        IsEffectivelyCollapsed(trace) || SpansClause(trace);

    bool IsEffectivelyCollapsed(CaptureTrace trace) =>
        trace.IsCollapsible && _runtimeSettings.HideCollapsibleCaptureNodes;

    /// <summary>
    /// For each clause captured whole by one top-level glyph, the first level of it that would draw an underline: the
    /// glyph's own, or past any collapsed ones, the descendant they collapse into. None unless collapsed nodes are hidden.
    /// </summary>
    HashSet<CaptureTrace> GetClauseSpanning(ProcessedLine line)
    {
        if (!_runtimeSettings.HideCollapsibleCaptureNodes)
            return [];

        var spanning = new HashSet<CaptureTrace>();

        foreach (var clause in line.Clauses)
        {
            if (clause.Units is not [{ CaptureContext.RootCaptureTrace: { IsSynthesized: false } root }])
                continue;

            CaptureTrace level = root;

            while (IsEffectivelyCollapsed(level))
                level = level.EffectiveChildren.First();

            spanning.Add(level);
        }

        return spanning;
    }

    /// <summary>
    /// How deep the visible (non-collapsed) nesting under <paramref name="roots"/> goes - some of the line's roots, like
    /// one clause's, so each clause reserves only the room its own underlines need.
    /// </summary>
    public int GetMaxEffectiveDepth(IReadOnlyCollection<RootCaptureTrace> roots)
    {
        var captureDepth = roots
            .Select(root => root.GetEffectiveDepth(IsUnderlineHidden))
            .DefaultIfEmpty(0)
            .Max();

        // Echo underlines share the exact same depth-to-pixel-offset scale as capture underlines
        // (both go through DocumentLineMetrics.GetUnderlinePaddingPx), so the same depth that
        // reserves vertical space for the deepest capture nesting can just as well reserve room
        // for the deepest echo lane stack too — whichever is taller wins.
        var echoLaneCount = _runtimeSettings.ShowEchoes && _echoCorpus != null
            ? _echoCorpus.GetMaxEchoLaneCount(
                _line.UnmatchedTextOccurrences.Where(x => roots.Contains(x.Anchor.CaptureContext.RootCaptureTrace)),
                _runtimeSettings.MinSpanWords,
                _runtimeSettings.MinSpanOccurences)
            : 0;

        return Math.Max(captureDepth, echoLaneCount);
    }
}
