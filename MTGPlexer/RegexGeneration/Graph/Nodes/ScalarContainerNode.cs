

namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public abstract class ScalarContainerNode : NamedGroupNode
{
    protected ScalarContainerNode(RegexNode parentNode, TypeNavigation navigation)
        : base(parentNode, navigation)
    {
    }

    //public override object GetValueAndSetHydrationInfo(CaptureContext captureContext)
    //{
    //    var singleCapture = captureContext[FullyQualifiedName].Capture;
    //
    //    if (singleCapture == null)
    //        return null;
    //
    //    var value = GetValueSingle(singleCapture);
    //
    //    if (value == null)
    //        return null;
    //
    //    CaptureValueHydrationInfo = new(this, singleCapture, value);
    //    return value;
    //}

    public abstract object GetValueSingle(Capture capture);
}
