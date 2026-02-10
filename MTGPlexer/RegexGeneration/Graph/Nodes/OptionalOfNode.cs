namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public class OptionalOfNode : WrapperNode
{
    NamedGroupNode _optionalItem;

    public OptionalOfNode(RegexNode parentNode, TypeNavigation navigation) 
        : base(parentNode, navigation)
    {
        _optionalItem = GetWrappedTokenUnitOrEnumNode(this, navigation.Type, MultiItemOrdinal.First.ToString());
    }

    public override void AppendRegexBricks(RegexCollector collector)
    {
        collector.Append(GroupOpenBrick);
        {
            _optionalItem.AppendRegexBricks(collector);
        }
        collector.Append(GetGroupCloseBrick(GroupQuantifier.Optional));
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