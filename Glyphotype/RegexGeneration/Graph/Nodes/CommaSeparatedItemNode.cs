namespace Glyphotype.RegexGeneration.Graph.Nodes;

/// <summary>
/// Represents the "second and later item" element type of a comma-separated X-Of family - currently
/// <see cref="CompoundOfSecondItem{T}"/> (for <see cref="CompoundOf{T}"/>'s <c>SecondPlus</c>) and
/// <see cref="ManyOfSecondItem{T}"/> (for <see cref="ManyOf{T}"/>'s <c>SecondPlus</c>). Both wrap a single
/// <c>Item</c> property with no <see cref="Glyph.Nibs"/> override of their own, and both need the same
/// leading <c>", "</c> before that item on every repetition - so both are routed here (see
/// <see cref="GlyphNode.GetNodeForNavigaton"/>) rather than each hand-writing their own leading-comma text
/// nib the way, say, <see cref="ManyOfSecondItem{T}"/> once did.
/// </summary>
public class CommaSeparatedItemNode : GlyphNode
{
    public CommaSeparatedItemNode(RegexNode parentNode, Navigation navigation)
    : base(parentNode, navigation)
    {
    }

    /// <summary>
    /// This node's own group is quantified (it's the element type of a <c>List&lt;&gt;</c> property), so
    /// its content repeats as a single <c>(?&lt;name&gt;...)*</c> span - there's no separate sibling node per
    /// repetition for a leading joiner to render between. The leading comma is this repeated span's own
    /// per-occurrence separator (not a sibling joiner - the owning X-Of's own <c>Joiner</c> is deliberately
    /// <see cref="Joiner.None"/> so nothing external ever tries to join before it), so it's emitted first,
    /// inside the group, on every repetition including the first.
    /// </summary>
    protected override void AppendInnerContentBricks(RegexCollector collector)
    {
        collector.Append(new RegexBrickJoiner(this, Joiner.CommaSpace));
        base.AppendInnerContentBricks(collector);
    }
}
