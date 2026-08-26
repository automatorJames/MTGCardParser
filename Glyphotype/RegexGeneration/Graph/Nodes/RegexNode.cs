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
    /// True for a node whose own group must contain the joiner that would otherwise separate it from its
    /// preceding sibling - because the group might not render at all (e.g. it's optional, or it's a
    /// quantified repeat standing in for zero-or-more siblings), so a joiner placed outside it by the
    /// parent's own sibling-joining loop would render unconditionally. See
    /// <see cref="NamedGroupNode.AppendInnerContentBricks"/>.
    /// </summary>
    public virtual bool OwnsLeadingJoiner => false;

    protected RegexNode(RegexNode parentNode, string name)
    {
        Name = name;
        ParentNode = parentNode;
        Lineage = GetLineage();
        FullyQualifiedName = string.Join('_', Lineage.Select(x => x.Name));
    }

    /// <summary>Appends this node's (and, for group nodes, its children's) <see cref="RegexBrick"/>s to <paramref name="collector"/> in matching order.</summary>
    public abstract void AppendRegexBricks(RegexCollector collector);

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