namespace Glyphotype.RegexGeneration.Graph.Nodes;

public class GlyphCompoundOfSecondItemNode : GlyphNode
{
    public override CaptureNodeKind NodeKind => CaptureNodeKind.Internals;

    /// <summary>The leading comma this node prepends below is its own content, not a sibling joiner - so nothing outside it should also try to join before it.</summary>
    public override bool OwnsLeadingJoiner => true;

    public GlyphCompoundOfSecondItemNode(RegexNode parentNode, Navigation navigation)
    : base(parentNode, navigation)
    {
    }

    /// <summary>
    /// This node's own group is quantified (it's the element type of a <c>List&lt;&gt;</c> property), so
    /// its content repeats as a single <c>(?&lt;name&gt;...)*</c> span - there's no separate sibling node per
    /// repetition for the base class's between-children joiner logic to insert a separator between. The
    /// leading comma has to be part of the repeated span itself, so it's emitted first, inside the group.
    /// </summary>
    protected override void AppendInnerContentBricks(RegexCollector collector)
    {
        collector.Append(new RegexBrickJoiner(this, Joiner.CommaSpace));
        base.AppendInnerContentBricks(collector);
    }
}