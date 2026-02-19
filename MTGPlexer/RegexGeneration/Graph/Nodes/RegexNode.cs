namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public abstract class RegexNode
{
    public string Name { get; }
    public string NamePath { get; }
    public RegexNode ParentNode { get; }
    public NamedGroupNode NamedGroupParentNode { get; }
    public RegexNode[] Lineage { get; }

    protected RegexNode(RegexNode parentNode, string name)
    {
        Name = name;
        ParentNode = parentNode;
        Lineage = GetLineage();
        NamePath = string.Join('.', Lineage.Select(x => x.Name));

        NamedGroupParentNode = Lineage
            .Take(Lineage.Length - 1) // exclude self
            .OfType<NamedGroupNode>()
            .LastOrDefault();
    }

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

    public override string ToString() => NamePath;
}