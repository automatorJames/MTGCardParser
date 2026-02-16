namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public class OptionalOfNode : WrapperNode
{
    public OptionalOfNode(RegexNode parentNode, PropNavigation navigation) 
        : base(parentNode, navigation)
    {
    }

    protected override void AddComputedChildren(List<RegexNode> children)
    {
        AnonymousGroupNode optionalItemContainer = new(this, "Optional_Item_Container", GroupQuantifier.Optional);
        var optionalItem = optionalItemContainer.AddWrappedNamedGroupChild(Navigation, GenericType, "Optional");
        children.Add(optionalItemContainer);
    }

    //protected override object GetWrapperValue(CaptureContext captureContext)
    //{
    //    var scopedCaptureContext = captureContext[FullyQualifiedName + "_" + GenericType.Name];
    //
    //    if (!scopedCaptureContext.Success)
    //        return null;
    //
    //    AddNewWrappedNode(scopedCaptureContext);
    //
    //    return CreateWrapperValue();
    //}
}