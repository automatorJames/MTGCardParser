namespace MTGPlexer.RegexGeneration.Graph.Bricks;

public class RegexBrick
{
    protected virtual int NestedDepthModifer => 0;

    public string Regex { get; }
    public string Comment { get; }
    public string FullyQualifiedName { get; }
    public int NestedDepth { get; }
    public RegexNode[] NodeLineage { get; }
    public RegexNode Parent => NodeLineage.LastOrDefault();
    public NamedGroupNode[] NamedGroupNodeLineage => NodeLineage.OfType<NamedGroupNode>().ToArray();
    public NamedGroupNode NamedGroupParent => NamedGroupNodeLineage.LastOrDefault();
    public string[] NamedGroupLineageNames => NamedGroupNodeLineage.Select(x => x.FullyQualifiedName).ToArray();

    // Omits transparent root groups (includes root groups with quantifiers)
    public NamedGroupNode[] GroupLineage { get; }
    public string[] GroupLineageNames { get; }

    // Optionally set during formatting phase after all RegexBricks have been initially rendered
    string _regexFormatted;
    public string RegexFormatted
    {
        get => _regexFormatted ?? Regex ?? "";
        set => _regexFormatted = value;
    }

    public RegexBrick(RegexNode parentNode, string regex, string comment)
    {
        Regex = regex;
        Comment = comment ?? "";
        NodeLineage = parentNode.Lineage;
        FullyQualifiedName = parentNode.FullyQualifiedName;
        GroupLineage = parentNode.Lineage.OfType<NamedGroupNode>().Where(x => !x.IsTransparentRoot).Reverse().ToArray();
        GroupLineageNames = GroupLineage.Select(x => x.Name).ToArray();
        NestedDepth = CalculateNestedDepth();
    }

    protected virtual int CalculateNestedDepth() =>
        GroupLineage.Length + NestedDepthModifer;

    public override string ToString() => RegexFormatted;
}
