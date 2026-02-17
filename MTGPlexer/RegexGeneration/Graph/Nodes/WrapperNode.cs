namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public abstract class WrapperNode : NamedGroupNode
{
    protected Type[] GenericTypes { get; }
    protected Type GenericType => GenericTypes[0];

    public WrapperNode(RegexNode parentNode, TypeNavigation navigation) 
        : base(parentNode, navigation)
    {
        GenericTypes = navigation.UnderlyingType.GenericTypeArguments;
    }


    //public override object GetValueAndSetHydrationInfo(CaptureContext captureContext)
    //{
    //    var scopedCaptureContext = captureContext[FullyQualifiedName];
    //    var value = GetWrapperValue(scopedCaptureContext);
    //
    //    if (value != null)
    //        CaptureValueHydrationInfo = new(this, scopedCaptureContext.Capture, value);
    //
    //    return value;
    //}

    //protected abstract object GetWrapperValue(CaptureContext captureContext);
}