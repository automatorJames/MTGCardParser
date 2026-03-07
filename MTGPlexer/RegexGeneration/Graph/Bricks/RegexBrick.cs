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
    public string[] ContainingGroupNames { get; }
    public int NamePartsToInclude { get; }

    // Optionally set during formatting phase after all RegexBricks have been initially rendered
    string _regexFormatted;
    public string RegexFormatted
    {
        get => _regexFormatted ?? Regex;
        set => _regexFormatted = value;
    }

    public RegexBrick(RegexNode parentNode, string regex, string comment)
    {
        Regex = regex;
        Comment = comment;
        Lineage = parentNode.Lineage;
        NamePath = parentNode.NamePath;
        ContainingGroupNames = parentNode.Lineage.Where(x => !x.MayIgnoreInFormattedOutput).Select(x => x.Name).Reverse().ToArray();
        NestedDepth = CalculateNestedDepth();
        NestedDepthFormatted = CalculateNestedDepthFormatted();
    }

    protected virtual int CalculateNestedDepth() =>
        Lineage.Count(x => x is NamedGroupNode) + NestedDepthModifer;

    protected virtual int CalculateNestedDepthFormatted() =>
        Lineage.Count(x => x is NamedGroupNode node && !node.MayIgnoreInFormattedOutput) + NestedDepthModifer;

    public override string ToString() => RegexFormatted;
}
