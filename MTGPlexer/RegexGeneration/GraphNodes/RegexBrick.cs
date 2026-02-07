namespace MTGPlexer.RegexGeneration.GraphNodes;

public class RegexBrick
{
    public string Regex { get; }
    public string Comment { get; }
    public string NamePath { get; }
    public int NestedDepth { get; }
    public RegexNode[] Lineage { get; }
    public RegexNode Parent => Lineage.FirstOrDefault();

    public RegexBrick(RegexNode parentNode, string regex, string comment)
    {
        Regex = regex;
        Comment = comment;
        Lineage = parentNode.Lineage;
        NamePath = parentNode.NamePath;
        NestedDepth = Lineage.Count(x => x is GroupNode);
    }
}
