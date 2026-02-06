namespace MTGPlexer.RegexGeneration.GraphNodes;

public abstract class RegexNode
{
    public string Name { get; set; }
    public RegexNode ParentNode { get; }

    //public abstract IEnumerable<Node> AnalysisNodes { get;}

    // todo: This feels like a hack that prevents duplicate parts in name paths
    // used only when WrappedNodes are in play rather than a univerasal necessity
    public virtual bool IsCollapsible => false;

    public abstract void ComposeRegexLines(RegexBuilder collector);
    public abstract void AppendRegexElements(RegexCollector collector);

    protected RegexNode(RegexNode parentNode, string name)
    {
        Name = name;
        ParentNode = parentNode;
    }
}