namespace MTGPlexer.RegexGeneration.GraphNodes;

public abstract class SingleCaptureNode : CaptureNode
{
    protected SingleCaptureNode(Node parentNode, INavigable navigable)
        : base(parentNode, navigable)
    {
    }

    public override object GetValueAndSetHydrationInfo(CaptureContext captureContext)
    {
        var singleCapture = captureContext[FullyQualifiedName].Capture;

        if (singleCapture == null)
            return null;

        var value = GetValueSingle(singleCapture);

        if (value == null)
            return null;

        CaptureValueHydrationInfo = new(this, singleCapture, value);
        return value;
    }

    public abstract object GetValueSingle(Capture capture);
}
