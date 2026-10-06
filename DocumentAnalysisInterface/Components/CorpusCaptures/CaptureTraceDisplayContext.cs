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

        Palettes = line.GetPositionalPalettes(IsEffectivelyCollapsed);
        LineRoots = line.CaptureTraceRoots;
    }

    bool IsEffectivelyCollapsed(CaptureTrace trace) =>
        trace.IsCollapsible && _runtimeSettings.HideCollapsibleCaptureNodes;

    /// <summary>
    /// How deep the visible (non-collapsed) nesting under <paramref name="roots"/> goes - some of the line's roots, like
    /// one clause's, so each clause reserves only the room its own underlines need.
    /// </summary>
    public int GetMaxEffectiveDepth(IReadOnlyCollection<RootCaptureTrace> roots)
    {
        var captureDepth = roots
            .Select(root => root.GetEffectiveDepth(IsEffectivelyCollapsed))
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
