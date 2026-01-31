namespace MTGPlexer.RegexGeneration.GraphNodes;

public abstract class ValueNode : Node
{
    protected ValueNode(Node parentNode, string name)
    : base(parentNode, name)
    {
    }

    public abstract CaptureValueInfo GetCaputureValueInfo(CaptureDictionary captures);
}
