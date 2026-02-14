namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public abstract class RegexNode
{
    public string Name { get; }
    public string NamePath { get; }
    public RegexNode ParentNode { get; }
    public RegexNode[] Lineage { get; }

    protected RegexNode(RegexNode parentNode, string name)
    {
        Name = name;
        ParentNode = parentNode;
        Lineage = GetLineage();
        NamePath = string.Join('.', Lineage.Select(x => x.Name));
    }

    // todo: This feels like a hack that prevents duplicate parts in name paths
    // used only when WrappedNodes are in play rather than a univerasal necessity
    public virtual bool IsCollapsible => false;

    public RegexBrick GetJoinerBrick(Joiner joiner, bool isOptional = false)
    {
        var regex = joiner.GetDescription() + (isOptional ? "?" : "");
        var comment = $"joiner {joiner.ToString().ToFriendlyCase(TitleDisplayOption.Lower)}";

        if (isOptional)
            comment = "optional " + comment;

        return new RegexBrick(this, regex, comment);

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