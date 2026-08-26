namespace Glyphotype.RegexGeneration.Graph.Nodes;

public class GlyphCompoundOfSecondItemNode : GlyphNode
{
    public override CaptureNodeKind NodeKind => CaptureNodeKind.Internals;

    public GlyphCompoundOfSecondItemNode(RegexNode parentNode, Navigation navigation)
    : base(parentNode, navigation)
    {
    }

    /// <summary>
    /// This node's own group is quantified (it's the element type of a <c>List&lt;&gt;</c> property), so
    /// its content repeats as a single <c>(?&lt;name&gt;...)*</c> span - there's no separate sibling node per
    /// repetition for a leading joiner to render between. The leading comma is this repeated span's own
    /// per-occurrence separator (not a sibling joiner - <see cref="CompoundOfBase.Joiner"/> is deliberately
    /// <see cref="Joiner.None"/> so nothing external ever tries to join before it), so it's emitted first,
    /// inside the group, on every repetition including the first.
    /// </summary>
    protected override void AppendInnerContentBricks(RegexCollector collector)
    {
        collector.Append(new RegexBrickJoiner(this, Joiner.CommaSpace));
        base.AppendInnerContentBricks(collector);
    }
}