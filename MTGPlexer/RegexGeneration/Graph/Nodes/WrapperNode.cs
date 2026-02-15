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

    //protected WrappedNode GetTemplateNodeForType(int genericTypeIndex = 0) =>
    //    new WrappedNode(this, GenericTypes[genericTypeIndex]);

    //public WrappedNode AddNewWrappedNode(CaptureContext captureContext, int? ordinal = null, Type genericType = null)
    //{
    //    genericType ??= GenericType;
    //    WrappedNode wrappedNode = new(this, genericType, ordinal);
    //    wrappedNode.GetValueAndSetHydrationInfo(captureContext);
    //    WrappedNodes.Add(wrappedNode);
    //    return wrappedNode;
    //}

    //protected object CreateWrapperValue() => CreateWrapperValue(WrappedValues);

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