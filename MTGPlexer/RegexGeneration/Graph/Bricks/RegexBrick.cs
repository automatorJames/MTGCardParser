namespace MTGPlexer.RegexGeneration.Graph.Bricks;

/// <summary>
/// One atomic piece of a compiled regex, tied back to the <see cref="RegexNode"/> that produced it.
/// <see cref="Regex"/> is the immutable text actually used for pattern matching (see <c>BuiltRegex.MinifiedRegex</c>);
/// it is never touched after construction. <see cref="RegexFormatted"/> and <see cref="CommentFormatted"/> are
/// display-only and are populated entirely by the Formatting layer (e.g. <c>RegexBrickFormattingPipeline</c>,
/// <c>SmartLineRenderer</c>) — nothing in the Graph layer assigns them, so there is exactly one place responsible
/// for deciding what a brick's rendered comment says.
/// </summary>
public class RegexBrick
{
    /// <summary>Adjustment applied to <see cref="NestedDepth"/> by subclasses (e.g. group bookends sit one level shallower than their contents).</summary>
    protected virtual int NestedDepthModifer => 0;

    /// <summary>The immutable regex text this brick contributes to the compiled matching pattern.</summary>
    public string Regex { get; }

    /// <summary>The fully qualified name of the <see cref="RegexNode"/> that produced this brick.</summary>
    public string FullyQualifiedName { get; }

    /// <summary>How many enclosing named groups (excluding transparent roots) this brick is nested within.</summary>
    public int NestedDepth { get; }

    /// <summary>The full ancestor chain (root-to-self) of the <see cref="RegexNode"/> that produced this brick.</summary>
    public RegexNode[] NodeLineage { get; }

    /// <summary>The <see cref="RegexNode"/> that directly produced this brick.</summary>
    public RegexNode Parent => NodeLineage.LastOrDefault();

    /// <summary>The subset of <see cref="NodeLineage"/> that are named groups, root-to-self.</summary>
    public NamedGroupNode[] NamedGroupNodeLineage => NodeLineage.OfType<NamedGroupNode>().ToArray();

    /// <summary>The nearest enclosing named group.</summary>
    public NamedGroupNode NamedGroupParent => NamedGroupNodeLineage.LastOrDefault();

    /// <summary>Fully qualified names of every named group enclosing this brick, root-to-self.</summary>
    public string[] NamedGroupLineageNames => NamedGroupNodeLineage.Select(x => x.FullyQualifiedName).ToArray();

    /// <summary>Enclosing named groups, self-to-root, omitting transparent root groups.</summary>
    public NamedGroupNode[] GroupLineage { get; }

    /// <summary>Short names (not fully qualified) of <see cref="GroupLineage"/>, self-to-root.</summary>
    public string[] GroupLineageNames { get; }

    string _regexFormatted;

    /// <summary>Display-only regex text, defaulting to <see cref="Regex"/> until the Formatting layer overrides it (e.g. simplified group names, ranked enum members).</summary>
    public string RegexFormatted
    {
        get => _regexFormatted ?? Regex ?? "";
        set => _regexFormatted = value;
    }

    /// <summary>Display-only comment text for this brick, assigned exclusively by the Formatting layer. Empty until then.</summary>
    public string CommentFormatted { get; set; } = "";

    public RegexBrick(RegexNode parentNode, string regex)
    {
        Regex = regex;
        NodeLineage = parentNode.Lineage;
        FullyQualifiedName = parentNode.FullyQualifiedName;
        GroupLineage = parentNode.Lineage.OfType<NamedGroupNode>().Where(x => !x.IsTransparentRoot).Reverse().ToArray();
        GroupLineageNames = GroupLineage.Select(x => x.Name).ToArray();
        NestedDepth = CalculateNestedDepth();
    }

    /// <summary>Computes <see cref="NestedDepth"/> from <see cref="GroupLineage"/>, adjusted by <see cref="NestedDepthModifer"/>.</summary>
    protected virtual int CalculateNestedDepth() =>
        GroupLineage.Length + NestedDepthModifer;

    public override string ToString() => RegexFormatted;
}
