namespace MTGPlexer.RegexGeneration.Graph.Bricks;

public class RegexBrick
{
    protected virtual int NestedDepthModifer => 0;

    public string Regex { get; }
    public string Comment { get; }
    public string NamePath { get; }
    public int NestedDepth { get; }
    public int NestedDepthFormatted { get; }
    public RegexNode[] Lineage { get; }
    public RegexNode Parent => Lineage.LastOrDefault();

    public RegexBrick(RegexNode parentNode, string regex, string comment)
    {
        Regex = regex;
        Comment = comment;
        Lineage = parentNode.Lineage;
        NamePath = parentNode.NamePath;
        NestedDepth = CalculateNestedDepth();
        NestedDepthFormatted = CalculateNestedDepthFormatted();
    }

    protected virtual int CalculateNestedDepth() =>
        Lineage.Count(x => x is NamedGroupNode) + NestedDepthModifer;

    protected virtual int CalculateNestedDepthFormatted() =>
        Lineage.Count(x => x is NamedGroupNode node && !node.MayIgnoreInFormattedOutput) + NestedDepthModifer;

    public override string ToString() => Regex;
}
