using Glyphotype.RegexGeneration.Graph;

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

    public CaptureTraceDisplayContext(ProcessedLine line, RuntimeSettings runtimeSettings)
    {
        bool IsEffectivelyCollapsed(CaptureTrace trace) =>
            trace.IsCollapsible && runtimeSettings.HideCollapsibleCaptureNodes;

        Palettes = line.GetPositionalPalettes(IsEffectivelyCollapsed);

        MaxEffectiveDepth = line.CaptureTraceRoots
            .Select(root => root.GetEffectiveDepth(IsEffectivelyCollapsed))
            .DefaultIfEmpty(0)
            .Max();
    }
}
