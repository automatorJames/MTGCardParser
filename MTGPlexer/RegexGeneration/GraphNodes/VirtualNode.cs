namespace MTGPlexer.RegexGeneration.GraphNodes;

public class VirtualNode : CaptureNode
{
    public WrappedNode ChildNode { get; private set; }

    public VirtualNode(Node parentNode, string name) 
        : base(parentNode, new TypeNavigation(typeof(object), name))
    {
    }

    public VirtualNode AddChild(WrappedNode childnode)
    {
        ChildNode = childnode;
        return this;
    }
    
    public WrappedNode HydrateFromCapture(Capture capture) =>
        ChildNode.HydrateFromCapture(capture);

    public override void ComposeRegexLines(RegexBuilder builder)
    {
        if (ChildNode == null)
            throw new Exception($"Cannot build regex before {nameof(ChildNode)} has been set");

        builder.OpenNamedGroup(this);
        ChildNode.ComposeRegexLines(builder);
        builder.CloseGroup();
    }

    public override object TryGetValue(CaptureDictionary captureDictionary, out CaptureValueResult result)
    {
        if (ChildNode == null)
        {
            result = CaptureValueResult.Exception;
            return null;
        }

        var value = ChildNode.TryGetValue(captureDictionary, out result);
        return new { value };
    }
}
