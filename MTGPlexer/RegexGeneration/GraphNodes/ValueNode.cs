namespace MTGPlexer.RegexGeneration.GraphNodes;

public abstract class ValueNode : Node
{
    public CaptureValueInfo HydratedCaptureValueInfo { get; set; }

    protected ValueNode(Node parentNode, string name)
    : base(parentNode, name)
    {
    }

    public abstract CaptureValueInfo GetCaptureValueInfo(CaptureDictionary captureDictionary);
}