namespace MTGPlexer.RegexGeneration.Graph.Nodes;

/// <summary>
/// A node in the tree that mirrors a <see cref="TokenUnit"/> type's declared structure (its snippets,
/// its properties, and their own nested <see cref="TokenUnit"/>/enum/primitive types). Walking this tree
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