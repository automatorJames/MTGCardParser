namespace Glyphotype.RegexGeneration.Graph.Nodes;

/// <summary>
/// Represents <see cref="UnmatchedString"/>, the fallback type for source text that matched no
/// <see cref="Glyph"/>. UnmatchedString never goes through the Glyph type-registry (it has no
/// GlyphTypeConfiguration, no nibs to reflect over, and is never hydrated via TryHydrate); this node
/// exists solely so UnmatchedString's own constructor has something to seed a CaptureContext/
/// RootCaptureTrace with, and so <see cref="RootCaptureTrace.IsUnmatchedString"/> can distinguish it
/// from a real matched Glyph.
/// </summary>
public class UnmatchedGlyphNode : GlyphNode
{
    public UnmatchedGlyphNode(RegexNode parentNode, Navigation navigation)
        : base(parentNode, navigation)
    {
    }

    protected override void AddReflectedChildren(List<RegexNode> children)
    {
        // UnmatchedString has no nibs to reflect over - it's a fixed-shape fallback, not a Glyph.
    }

    public override bool TryHydrate(CaptureTrace captureTrace, out Glyph glyph) =>
        throw new NotSupportedException($"{nameof(UnmatchedString)} is constructed directly (see its own constructor), never hydrated via {nameof(GlyphNode)}.");
}
