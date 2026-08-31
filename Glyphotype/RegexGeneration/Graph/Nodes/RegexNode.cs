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
    /// Appends the joiner between this node and whatever precedes it - unless there's no preceding sibling,
    /// or this node isn't itself <see cref="IsNullable"/> and nothing anywhere in its prefix (every sibling
    /// back to index 0) is guaranteed to render (see <see cref="HasAnchorBefore"/>). That second case is the
    /// one place this node can't safely place its joiner externally: with no guaranteed "anchor" anywhere
    /// before it, this node could end up being the literal start of the match, so an unconditional joiner
    /// here would render even when the whole prefix matched nothing. When that happens, the nearest
    /// preceding nullable sibling takes over instead, embedding this same joiner as its own trailing content
    /// (see <see cref="AppendTrailingJoinerBrickIfOwned"/>) - it's the only place left that can legitimately
    /// hide the joiner precisely when the prefix does render something.
    /// </summary>
    /// <param name="insideOwnGroup">
    /// Whether this call is emitting the joiner within this node's own group bookends (the
    /// <see cref="IsNullable"/> path, called from <see cref="NamedGroupNode.AppendOwnRegexBricks"/>) rather
    /// than immediately before them. Decides only which node <em>owns</em> the resulting brick - see the
    /// <c>owner</c> parameter on <see cref="AppendJoinerBrickIfWarranted"/>.
    /// </param>
    protected void AppendLeadingJoinerBrick(RegexCollector collector, bool insideOwnGroup = false)
    {
        if (ParentNode is not NamedGroupNode parent)
            return;

        var index = parent.Children.IndexOf(this);

        if (index <= 0)
            return;

        if (!IsNullable && !HasAnchorBefore(parent, index))
            return;

        AppendJoinerBrickIfWarranted(collector, parent, after: this, owner: insideOwnGroup ? this : parent);
    }

    /// <summary>
    /// For a nullable node immediately followed by a non-nullable sibling that has no other guaranteed
    /// anchor earlier in its own prefix (see <see cref="HasAnchorBefore"/>), appends the joiner that belongs
    /// between them here, as this node's own trailing content. That successor can't place an unconditional
    /// joiner of its own - nothing before it is guaranteed to render - so this node, being the last thing in
    /// an otherwise all-nullable prefix, is the one place left that can render the joiner exactly when it's
    /// actually needed. (A run of several nullable siblings all sharing an all-nullable prefix isn't fully
    /// covered by this - only the one immediately before the non-nullable node ever takes over - but that's
    /// the shape every case in this codebase actually takes.)
    /// </summary>
    protected void AppendTrailingJoinerBrickIfOwned(RegexCollector collector)
    {
        if (ParentNode is not NamedGroupNode parent)
            return;

        var siblings = parent.Children;
        var index = siblings.IndexOf(this);

        if (index < 0 || index >= siblings.Count - 1)
            return;

        var next = siblings[index + 1];

        if (next.IsNullable || HasAnchorBefore(parent, index + 1))
            return;

        AppendJoinerBrickIfWarranted(collector, parent, after: next, owner: this);
    }

    /// <summary>Whether any of <paramref name="parent"/>'s children before <paramref name="index"/> is guaranteed to render something (i.e. isn't <see cref="IsNullable"/>) - see <see cref="AppendLeadingJoinerBrick"/>.</summary>
    static bool HasAnchorBefore(NamedGroupNode parent, int index) =>
        parent.Children.Take(index).Any(sibling => !sibling.IsNullable);

    /// <summary>
    /// Appends <paramref name="parent"/>'s joiner - unless the parent doesn't join its children, the
    /// collector already ends in a space, or <paramref name="after"/> is text that absorbs the joiner itself -
    /// by opening with punctuation that binds to the token before it (e.g. <c>'s</c> or a <c>","</c> nib) or
    /// by already supplying the space (see <see cref="TextNode.AbsorbsPrecedingJoiner"/>).
    /// </summary>
    /// <param name="after">The node this joiner immediately precedes. Only consulted to decide <em>whether</em> to join; it is deliberately not the brick's owner, since a joiner sitting before a group's open bookend is not inside that group.</param>
    /// <param name="owner">
    /// The node in whose rendered scope this brick actually sits, and therefore the node it's attributed to.
    /// For a joiner emitted before a sibling that's the enclosing <paramref name="parent"/> group; for one
    /// emitted within a nullable node's own bookends it's that node itself. Everything downstream keys off
    /// this - a brick's indent depth (<c>RegexBrick.NestedDepth</c>), its rainbow color
    /// (<c>RegexBrick.NamedGroupParent</c>), and the "which bricks belong to my group" queries that
    /// <c>RegexBrickFormattingPipeline</c> and the section builders run - so attributing a joiner to the
    /// group it merely precedes makes it render as (and be harvested as) that group's own inner content.
    /// </param>
    static void AppendJoinerBrickIfWarranted(RegexCollector collector, NamedGroupNode parent, RegexNode after, RegexNode owner)
    {
        var joiner = parent.EffectiveChildJoiner;

        bool shouldJoin =
            joiner != Joiner.None
            && !collector.AlreadySeparated
            && !(after is TextNode textNode && textNode.AbsorbsPrecedingJoiner);

        if (shouldJoin)
            collector.Append(new RegexBrickJoiner(owner, joiner));
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