namespace Glyphotype.RegexGeneration.Graph.Nodes;

/// <summary>
/// Represents <see cref="EnclosureMark"/>, as <see cref="ClauseBreakNode"/> does <see cref="ClauseBreak"/>: never
/// hydrated, it exists only to seed the mark's CaptureContext/RootCaptureTrace, and so
/// <see cref="RootCaptureTrace.IsEnclosureMark"/> can tell it apart from a matched Glyph.
/// </summary>
public class EnclosureMarkNode : GlyphNode
{
    public EnclosureMarkNode(RegexNode parentNode, Navigation navigation)
        : base(parentNode, navigation)
    {
    }

    protected override void AddReflectedChildren(List<RegexNode> children)
    {
        // A fixed-shape token, not a Glyph - there are no nibs to reflect over.
    }

    public override bool TryHydrate(CaptureTrace captureTrace, out Glyph glyph) =>
        throw new NotSupportedException($"{nameof(EnclosureMark)} is constructed directly (see its own constructor), never hydrated via {nameof(GlyphNode)}.");
}
