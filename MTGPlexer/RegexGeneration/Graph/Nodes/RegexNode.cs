namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public abstract class RegexNode
{
    public string Name { get; set; }
    public string NamePath { get; }
    public RegexNode[] Lineage { get; }
    public RegexNode ParentNode => Lineage.LastOrDefault();

    public RegexBrick GetJoinerBrick(Joiner joiner, bool isOptional = false)
    {
        var regex = joiner.GetDescription() + (isOptional ? "?" : "");
        var comment = $"joiner {joiner.ToString().ToFriendlyCase(TitleDisplayOption.Lower)}";

        if (isOptional)
            comment = "optional " + comment;

        return new RegexBrick(this, regex, comment);

    }

    // todo: This feels like a hack that prevents duplicate parts in name paths
    // used only when WrappedNodes are in play rather than a univerasal necessity
    public virtual bool IsCollapsible => false;

    public abstract void AppendRegexBricks(RegexCollector collector);

    protected RegexNode(RegexNode parentNode, string name)
    {
        Name = name;
        Lineage = GetLineage();
        NamePath = string.Join('.', Lineage.Select(x => x.Name));
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
}