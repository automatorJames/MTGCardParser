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
    /// Appends this node's own leading joiner - the separator that belongs between it and its preceding
    /// sibling, if any (see <see cref="NamedGroupNode.EffectiveChildJoiner"/>) - unless this node is
    /// <see cref="IsNullable"/>, in which case it's this node's own responsibility to embed that same
    /// joiner wherever it belongs inside its own span (see <see cref="NamedGroupNode.AppendOwnRegexBricks"/>).
    /// Then appends this node's own bricks. Every node renders its own leading joiner uniformly through this
    /// one template method, rather than leaving the decision to the parent's child-walking loop, so no node
    /// type has to opt into or duplicate this logic.
    /// </summary>
    public void AppendRegexBricks(RegexCollector collector)
    {
        if (!IsNullable)
            AppendLeadingJoinerBrick(collector);

        AppendOwnRegexBricks(collector);
    }

    /// <summary>This node's own bricks - for a <see cref="IsNullable"/> node, including its own leading joiner (see <see cref="AppendRegexBricks"/>).</summary>
    protected abstract void AppendOwnRegexBricks(RegexCollector collector);

    /// <summary>
    /// Appends the joiner that belongs between this node and its preceding sibling, per the parent's own
    /// <see cref="NamedGroupNode.EffectiveChildJoiner"/> - or nothing, if there's no preceding sibling, the
    /// parent doesn't join its children at all, the collector already ends in a space, or this node is text
    /// starting with a literal apostrophe (e.g. <c>'s</c>, which should hug the token before it).
    /// </summary>
    protected void AppendLeadingJoinerBrick(RegexCollector collector)
    {
        if (ParentNode is not NamedGroupNode parent)
            return;

        var joiner = parent.EffectiveChildJoiner;

        bool shouldJoin =
            parent.Children.IndexOf(this) > 0
            && joiner != Joiner.None
            && collector.LastChar != ' '
            && !(this is TextNode textNode && textNode.FirstChar == '\'');

        if (shouldJoin)
            collector.Append(new RegexBrickJoiner(this, joiner));
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