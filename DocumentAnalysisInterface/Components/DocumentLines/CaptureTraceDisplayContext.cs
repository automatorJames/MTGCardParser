using Glyphotype.RegexGeneration.Graph;
using Glyphotype.GlyphAnalysisDTOs.WordTrees;

namespace DocumentAnalysisInterface.Components.DocumentLines;

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
    public int MaxEffectiveDepth { get; }

    public CaptureTraceDisplayContext(ProcessedLine line, RuntimeSettings runtimeSettings, DigestedText echoCorpus)
    {
        bool IsEffectivelyCollapsed(CaptureTrace trace) =>
            trace.IsCollapsible && runtimeSettings.HideCollapsibleCaptureNodes;

        Palettes = line.GetPositionalPalettes(IsEffectivelyCollapsed);

        var captureDepth = line.CaptureTraceRoots
            .Select(root => root.GetEffectiveDepth(IsEffectivelyCollapsed))
            .DefaultIfEmpty(0)
            .Max();

        // Echo underlines share the exact same depth-to-pixel-offset scale as capture underlines
        // (see EchoUnderline's own padding-bottom formula), so the same MaxEffectiveDepth the line
        // already uses to reserve vertical space for the deepest capture nesting can just as well
        // reserve room for the deepest echo lane stack too — whichever is taller wins.
        var echoLaneCount = runtimeSettings.ShowEchoes && echoCorpus != null
            ? echoCorpus.GetMaxEchoLaneCount(line, runtimeSettings.MinSpanWords, runtimeSettings.MinSpanOccurences)
            : 0;

        MaxEffectiveDepth = Math.Max(captureDepth, echoLaneCount);
    }
}
