namespace Glyphotype.RegexGeneration.Graph.Nodes;

/// <summary>
/// A node in the tree that mirrors a <see cref="Glyph"/> type's declared structure (its nibs,
/// its properties, and their own nested <see cref="Glyph"/>/enum/primitive types). Walking this tree
/// via <see cref="AppendRegexBricks"/> produces the flat <see cref="RegexBrick"/> sequence that compiles
/// into the type's matching regex.
/// </summary>
public abstract class RegexNode
{
    /// <summary>This node's own short name (not fully qualified).</summary>
    public string Name { get; }

    /// <summary>This node's name qualified by every ancestor's name, e.g. <c>Root_Child_Grandchild</c>. Used as the regex capture group name.</summary>
    public string FullyQualifiedName { get; }

    /// <summary>The node that owns this node as a child, or null for the graph's root.</summary>
    public RegexNode ParentNode { get; }

    /// <summary>This node's full ancestor chain, root-to-self (inclusive of this node).</summary>
    public RegexNode[] Lineage { get; }

    /// <summary>
    /// Whether this node's own span can legitimately match nothing at all - the grammar-theory sense of
    /// "nullable" (a production that can derive the empty string), which for this graph means a group whose
    /// own <see cref="GroupNode.Quantifier"/> permits zero occurrences (<c>?</c> or <c>*</c>) - see the
    /// override on <see cref="GroupNode"/>. Drives where <see cref="AppendRegexBricks"/> places this node's
    /// own leading joiner: immediately before it when false, or as its own first inner content brick when
    /// true - since only then would a joiner placed unconditionally outside it risk rendering even though
    /// this node matched nothing.
    /// </summary>
    public virtual bool IsNullable => false;

    protected RegexNode(RegexNode parentNode, string name)
    {
        Name = name;
        ParentNode = parentNode;
        Lineage = GetLineage();
        FullyQualifiedName = string.Join('_', Lineage.Select(x => x.Name));
    }

    /// <summary>
    /// Appends this node's bricks, preceded by the joiner separating it from its preceding sibling when
    /// <see cref="JoinerRules.PlaceLeadingJoiner"/> places that joiner here. (A nullable node places it inside its
    /// own group instead - see <see cref="NamedGroupNode.AppendOwnRegexBricks"/>.) Every node goes through this one
    /// template method, so no node type has to opt into or duplicate joiner placement; every decision about
    /// joiners is <see cref="JoinerRules"/>'.
    /// </summary>
    public void AppendRegexBricks(RegexCollector collector)
    {
        if (LeadingJoinerPlacement == JoinerPlacement.BeforeNode)
            AppendJoinerBefore(this, collector, owner: ParentNode);

        AppendOwnRegexBricks(collector);
    }

    /// <summary>This node's own bricks - for a nullable node, including its own leading joiner (see <see cref="AppendRegexBricks"/>).</summary>
    protected abstract void AppendOwnRegexBricks(RegexCollector collector);

    /// <summary>Where the joiner separating this node from its preceding sibling goes - see <see cref="JoinerRules.PlaceLeadingJoiner"/>.</summary>
    protected JoinerPlacement LeadingJoinerPlacement
    {
        get
        {
            if (ParentNode is not NamedGroupNode parent)
                return JoinerPlacement.None;

            var index = parent.Children.IndexOf(this);

            return JoinerRules.PlaceLeadingJoiner(isFirstChild: index <= 0, IsNullable, hasAnchorBefore: index > 0 && HasAnchorBefore(parent, index));
        }
    }

    /// <summary>The following sibling whose leading joiner this node carries as its own trailing content (see <see cref="JoinerRules.OwnsTrailingJoiner"/>), or null.</summary>
    protected RegexNode TrailingJoinerSuccessor
    {
        get
        {
            if (ParentNode is not NamedGroupNode parent)
                return null;

            var siblings = parent.Children;
            var index = siblings.IndexOf(this);
            var next = index >= 0 && index < siblings.Count - 1 ? siblings[index + 1] : null;

            bool owns = JoinerRules.OwnsTrailingJoiner(
                IsNullable,
                hasNext: next is not null,
                hasAnchorBefore: index > 0 && HasAnchorBefore(parent, index));

            return owns ? next : null;
        }
    }

    /// <summary>Whether any of <paramref name="parent"/>'s children before <paramref name="index"/> is guaranteed to render something (i.e. isn't <see cref="IsNullable"/>).</summary>
    static bool HasAnchorBefore(NamedGroupNode parent, int index) =>
        parent.Children.Take(index).Any(sibling => !sibling.IsNullable);

    /// <summary>Appends the joiner <see cref="JoinerRules.Between"/> calls for in front of <paramref name="after"/>, if any.</summary>
    /// <param name="after">The node this joiner immediately precedes. It decides which joiner (if any) goes here; it is deliberately not the brick's owner, since a joiner sitting before a group's open bookend is not inside that group.</param>
    /// <param name="owner">
    /// The node in whose rendered scope this brick actually sits, and therefore the node it's attributed to.
    /// For a joiner emitted before a sibling that's the enclosing group; for one emitted within a nullable
    /// node's own bookends it's that node itself. Everything downstream keys off this - a brick's indent depth
    /// (<c>RegexBrick.NestedDepth</c>), its rainbow color (<c>RegexBrick.NamedGroupParent</c>), and the "which
    /// bricks belong to my group" queries that <c>RegexBrickFormattingPipeline</c> and the section builders run -
    /// so attributing a joiner to the group it merely precedes makes it render as (and be harvested as) that
    /// group's own inner content.
    /// </param>
    /// <returns>The joiner appended, or <see cref="Joiner.None"/> when none was.</returns>
    protected static Joiner AppendJoinerBefore(RegexNode after, RegexCollector collector, RegexNode owner)
    {
        var joiner = JoinerRules.Between(JoinSite.Of((NamedGroupNode)after.ParentNode, after, collector));

        if (joiner != Joiner.None)
            collector.Append(new RegexBrickJoiner(owner, joiner));

        return joiner;
    }

    RegexNode[] GetLineage()
    {
        List<RegexNode> lineage = [];
        RegexNode current = this;

        while (current != null)
        {
            lineage.Add(current);
            current = current.ParentNode;
        }

        lineage.Reverse();
        return lineage.ToArray();
    }

    public override string ToString() => FullyQualifiedName;
}