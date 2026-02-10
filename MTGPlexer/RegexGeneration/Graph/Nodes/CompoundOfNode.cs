namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public class CompoundOfNode : WrapperNode
{
    Joiner _joiner;
    NamedGroupNode _itemTheFirst;
    NamedGroupNode _itemTheSecond;

    public CompoundOfNode(RegexNode parentNode, TypeNavigation navigation) 
        : base(parentNode, navigation)
    {
        _itemTheFirst = GetWrappedTokenUnitOrEnumNode(this, navigation.Type, MultiItemOrdinal.First.ToString());
        _itemTheSecond = GetWrappedTokenUnitOrEnumNode(this, navigation.Type, MultiItemOrdinal.SecondPlus.ToString());

        _joiner = navigation.UnderlyingType.GetCustomAttribute<CompoundJoinerAttribute>()?.Joiner
            ?? Joiner.Space;
    }

    public override void AppendRegexBricks(RegexCollector collector)
    {
        if (_joiner == Joiner.None)
            AppendRegexBricksNoJoiner(collector);
        else
            AppendRegexBricksWithJoiner(collector);
    }

    void AppendRegexBricksNoJoiner(RegexCollector collector)
    {
        _itemTheFirst.AppendRegexBricks(collector);
    }

    void AppendRegexBricksWithJoiner(RegexCollector collector)
    {
        _itemTheFirst.AppendRegexBricks(collector);

        collector.Append(AnonymousGroupOpenBrick);
        {
            _itemTheSecond.AppendRegexBricks(collector);
        }
        collector.Append(GetGroupCloseBrick(GroupQuantifier.OneOrMore));
    }

    //protected override object GetWrapperValue(CaptureContext captureContext)
    //{
    //    var scopedCaptureContext = captureContext[FullyQualifiedName];
    //
    //    // We expect two or more captures
    //    if (scopedCaptureContext.Captures.Length <= 1)
    //        return null;
    //
    //    for (int i = 0; i < scopedCaptureContext.Captures.Length; i++)
    //    {
    //        var singleScopedContext = scopedCaptureContext.ScopeToCaptureIndex(i);
    //        AddNewWrappedNode(singleScopedContext, ordinal: i);
    //    }
    //
    //    return CreateWrapperValue();
    //}
}