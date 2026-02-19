namespace MTGPlexer.RegexGeneration.Graph;

public class RegexBrick
{
    public string Regex { get; }
    public string Comment { get; }
    public string NamePath { get; }
    public int NestedDepth { get; }
    public RegexNode[] Lineage { get; }
    public RegexNode Parent => Lineage.LastOrDefault();

    public RegexBrick(RegexNode parentNode, string regex, string comment)
    {
        Regex = regex;
        Comment = comment;
        Lineage = parentNode.Lineage;
        NamePath = parentNode.NamePath;
        NestedDepth = CalculateNestedDepth();
    }

    protected virtual int CalculateNestedDepth() =>
        Lineage.Count(x => x is NamedGroupNode);

    public override string ToString() => Regex;
}
