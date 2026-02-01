namespace MTGPlexer.RegexGeneration.GraphNodes;

public abstract class Node
{
    public string Name { get; set; }
    public Node ParentNode { get; }

    // todo: This feels like a hack that prevents duplicate parts in name paths
    // used only when WrappedNodes are in play rather than a univerasal necessity
    public virtual bool IsCollapsible => false;

    public abstract void ComposeRegexLines(RegexBuilder collector);

    protected Node(Node parentNode, string name)
    {
        Name = name;
        ParentNode = parentNode;
    }
}