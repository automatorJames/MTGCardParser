namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public class VirtualNamedGroupNode : NamedGroupNode
{
    public WrappedNode ChildNode { get; private set; }

    public VirtualNamedGroupNode(RegexNode parentNode, string name, Type childType) 
        : base(parentNode, new TypeNavigation(typeof(object), name))
    {
        ChildNode = new(this, childType);
    }

    public WrappedNode HydrateChild(CaptureContext captureContext)
    {
        var scopedCapture = captureContext[FullyQualifiedName];
        ChildNode.GetValueAndSetHydrationInfo(scopedCapture);
        return ChildNode;
    }

    public override object GetValueAndSetHydrationInfo(CaptureContext captureContext)
    {
        var scopedCapture = captureContext[FullyQualifiedName];
        var value = ChildNode.GetValueAndSetHydrationInfo(scopedCapture);
        CaptureValueHydrationInfo = new(this, scopedCapture.Capture, value);

        return value;
    }

    public override void AppendRegexBricks(RegexCollector collector)
    {
        if (ChildNode == null)
            throw new Exception($"Cannot build regex before {nameof(ChildNode)} has been set");

        builder.OpenNamedGroup(this);
        ChildNode.ComposeRegexLines(builder);
        builder.CloseGroup();
    }
}
