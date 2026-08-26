namespace Glyphotype.RegexGeneration.Graph.Nodes;

/// <summary>
/// Represents an optional nested Glyph - reached either via a property decorated with
/// <see cref="OptionalAttribute"/> or one typed <see cref="OptionalOf{T}"/>; see <see cref="Navigation"/>,
/// where both routes converge on the same <see cref="Glyphotype.Quantifier.Optional"/> quantifier, and
/// <see cref="GetNodeForNavigaton"/>, where both routes resolve to this same node type.
///
/// This class exists purely to carry that identity - <see cref="CaptureNodeKind.Optional"/> for
/// presentation - rather than to change how the group itself renders: since this group's <see cref="Quantifier"/>
/// is already <see cref="Glyphotype.Quantifier.Optional"/>, the base <see cref="GroupNode.IsNullable"/> is
/// already true for it, so its leading joiner already gets embedded inside its own group, not outside, for
/// free (see <see cref="RegexNode.AppendRegexBricks"/> / <see cref="NamedGroupNode.AppendOwnRegexBricks"/>).
/// </summary>
public class OptionalGlyphNode : GlyphNode
{
    public override CaptureNodeKind NodeKind => CaptureNodeKind.Optional;

    public OptionalGlyphNode(RegexNode parentNode, Navigation navigation)
        : base(parentNode, navigation)
    {
    }
}
