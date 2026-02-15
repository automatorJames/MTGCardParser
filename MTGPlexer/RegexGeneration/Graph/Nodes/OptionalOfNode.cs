namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public class OptionalOfNode : WrapperNode
{
    NamedGroupNode _optionalItem;
    List<RegexNode> _immediateChildren = [];

    public OptionalOfNode(RegexNode parentNode, PropNavigation navigation) 
        : base(parentNode, navigation)
    {
        SetChildNodes(navigation);
    }

    void SetChildNodes(PropNavigation navigation)
    {
        AnonymousGroupNode optionalItemContainer = new(this, "Optional-Item-Container", GroupQuantifier.Optional);
        _optionalItem = optionalItemContainer.AddWrappedNamedGroupChild(navigation, GenericType, "Optional");
        _immediateChildren.Add(optionalItemContainer);
    }

    protected override List<RegexNode> GetChildNodes() => _immediateChildren;

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