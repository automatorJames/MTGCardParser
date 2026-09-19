namespace Glyphotype.RegexGeneration.Graph.Nodes;

/// <summary>
/// Represents <see cref="ClauseBreak"/>, the token the Tokenizer emits for a standalone clause-separating
/// period. Like <see cref="UnmatchedGlyphNode"/>, this never goes through the Glyph type registry (no
/// configuration, no nibs, never hydrated); it exists only so <see cref="ClauseBreak"/>'s constructor has
/// something to seed a CaptureContext/RootCaptureTrace with, and so
/// <see cref="RootCaptureTrace.IsClauseBreak"/> can tell it apart from a real matched Glyph.
/// </summary>
public class ClauseBreakNode : GlyphNode
{
    public ClauseBreakNode(RegexNode parentNode, Navigation navigation)
        : base(parentNode, navigation)
    {
    }

    protected override void AddReflectedChildren(List<RegexNode> children)
    {
        // A clause break is a fixed-shape token, not a Glyph - there are no nibs to reflect over.
    }

    public override bool TryHydrate(CaptureTrace captureTrace, out Glyph glyph) =>
        throw new NotSupportedException($"{nameof(ClauseBreak)} is constructed directly (see its own constructor), never hydrated via {nameof(GlyphNode)}.");
}
