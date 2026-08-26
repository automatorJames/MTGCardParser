namespace Glyphotype.RegexGeneration.Graph.Nodes;

/// <summary>
/// Represents an optional nested Glyph - reached either via a property decorated with
/// <see cref="OptionalAttribute"/> or one typed <see cref="OptionalOf{T}"/>; see <see cref="Navigation"/>,
/// where both routes converge on the same <see cref="Glyphotype.Quantifier.Optional"/> quantifier, and
/// <see cref="GetNodeForNavigaton"/>, where both routes resolve to this same node type.
///
/// Its own group already carries the "?" quantifier, but since the group may end up matching nothing at
/// all, any joiner that would otherwise separate it from a preceding sibling has to live *inside* its own
/// group - as its first content brick - rather than outside it as an ordinary sibling joiner would (the
/// same trick <see cref="GlyphCompoundOfSecondItemNode"/> uses for repeated groups).
/// </summary>
public class OptionalGlyphNode : GlyphNode
{
    public override CaptureNodeKind NodeKind => CaptureNodeKind.Optional;

    /// <summary>See the class summary - the leading joiner below is this group's own content, not a sibling joiner.</summary>
    public override bool OwnsLeadingJoiner => true;

    public OptionalGlyphNode(RegexNode parentNode, Navigation navigation)
        : base(parentNode, navigation)
    {
    }

    protected override void AppendInnerContentBricks(RegexCollector collector)
    {
        if (ParentNode is NamedGroupNode parent
            && parent.EffectiveChildJoiner != Joiner.None
            && parent.Children.IndexOf(this) > 0
            && collector.LastChar != ' ')
        {
            collector.Append(new RegexBrickJoiner(this, parent.EffectiveChildJoiner));
        }

        base.AppendInnerContentBricks(collector);
    }
}
