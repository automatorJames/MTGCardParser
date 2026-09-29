namespace Glyphotype.RegexGeneration.Graph.Nodes;

/// <summary>
/// Represents the "second and later item" element type of a joined X-Of family - currently
/// <see cref="CompoundOfSecondItem{T}"/> (for <see cref="CompoundOf{T}"/>'s <c>SecondPlus</c>) and
/// <see cref="ManyOfSecondItem{T}"/> (for <see cref="ManyOf{T}"/>'s <c>SecondPlus</c>). Both wrap a single
/// <c>Item</c> property with no <see cref="Glyph.Nibs"/> override of their own, and both need the same
/// leading joiner before that item on every repetition - so both are routed here (see
/// <see cref="GlyphNode.GetNodeForNavigaton"/>) rather than each hand-writing their own leading-joiner text
/// nib the way, say, <see cref="ManyOfSecondItem{T}"/> once did.
/// <para>
/// A <see cref="ManyOf{T}"/> always joins with <see cref="Joiner.CommaSpace"/> ("A, B, and C" is fixed English
/// list grammar). A <see cref="CompoundOf{T}"/> defaults to the same, but its usage site may declare otherwise
/// via <see cref="JoinedByAttribute"/> - see <see cref="ResolveJoiner"/>.
/// </para>
/// </summary>
public class JoinedItemNode : GlyphNode
{
    public JoinedItemNode(RegexNode parentNode, Navigation navigation)
    : base(parentNode, navigation)
    {
    }

    /// <summary>
    /// This node's own group is quantified (it's the element type of a <c>List&lt;&gt;</c> property), so
    /// its content repeats as a single <c>(?&lt;name&gt;...)*</c> span - there's no separate sibling node per
    /// repetition for a leading joiner to render between. The leading joiner is this repeated span's own
    /// per-occurrence separator (not a sibling joiner - the owning X-Of's own <c>Joiner</c> is deliberately
    /// <see cref="Joiner.None"/> so nothing external ever tries to join before it), so it's emitted first,
    /// inside the group, on every repetition including the first.
    /// </summary>
    protected override void AppendInnerContentBricks(RegexCollector collector)
    {
        collector.Append(new RegexBrickJoiner(this, ResolveJoiner()));
        base.AppendInnerContentBricks(collector);
    }

    /// <summary>
    /// The owning <see cref="CompoundOf{T}"/>'s <see cref="JoinedByAttribute"/> - from the property it's bound
    /// to first (the more specific site), else from its own type (a <see cref="CompoundOf{T}"/> subclass, e.g. a
    /// top-level alias with no property to decorate) - else <see cref="Joiner.CommaSpace"/>. Always the latter
    /// for a <see cref="ManyOf{T}"/>. The owner is always the direct parent: a <c>List&lt;&gt;</c> property's
    /// navigation targets its element type, so <c>SecondPlus</c> contributes no intermediate node.
    /// </summary>
    Joiner ResolveJoiner()
    {
        if (ParentNode is not GlyphNode { Navigation: var ownerNavigation } || !typeof(CompoundOfBase).IsAssignableFrom(ownerNavigation.NodeType))
            return Joiner.CommaSpace;

        var joinedBy = ownerNavigation.Prop?.GetCustomAttribute<JoinedByAttribute>()
            ?? ownerNavigation.NodeType.GetCustomAttribute<JoinedByAttribute>();

        return joinedBy?.Joiner ?? Joiner.CommaSpace;
    }
}
