namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public class CompoundOfNode : WrapperNode
{
    protected override Joiner Joiner => Joiner.Pipe;
    protected override GroupQuantifier? Quantifier => GroupQuantifier.OneOrMore;

    Joiner _localJoinerBetweenTerms;

    NamedGroupNode _nodeTheFirst;
    NamedGroupNode _nodeTheSecond;

    List<RegexNode> _immediateChildren;

    public CompoundOfNode(RegexNode parentNode, PropNavigation navigation) 
        : base(parentNode, navigation)
    {
        _localJoinerBetweenTerms = navigation.UnderlyingType.GetCustomAttribute<CompoundJoinerAttribute>()?.Joiner
            ?? Joiner.Space;

        SetChildNodes(navigation);
    }

    void SetChildNodes(PropNavigation navigation)
    {
        if (_localJoinerBetweenTerms == Joiner.None)
        {
            AnonymousGroupNode twoOrMoreWrapper = new(this, "Two-Or-More", GroupQuantifier.TwoOrMore);
            _nodeTheFirst = twoOrMoreWrapper.AddWrappedNamedGroupChild(navigation, GenericType, MultiItemOrdinal.First.ToString());
            _immediateChildren = [twoOrMoreWrapper];
        }
        else
        {
            _nodeTheFirst = GetNamedGroupChild(this, navigation, GenericType, MultiItemOrdinal.First.ToString());
            AnonymousGroupNode secondItemOneOrMoreWrapper = new(this, "One-Or-More", GroupQuantifier.TwoOrMore);
            secondItemOneOrMoreWrapper.AddWrappedBrick("Joiner", new RegexBrick(this, _localJoinerBetweenTerms.GetDescription(), $"joiner {_localJoinerBetweenTerms}"));
            _nodeTheSecond = secondItemOneOrMoreWrapper.AddWrappedNamedGroupChild(navigation, GenericType, MultiItemOrdinal.SecondPlus.ToString());
            _immediateChildren = [_nodeTheFirst, secondItemOneOrMoreWrapper];
        }
    }

    protected override List<RegexNode> GetChildNodes() => _immediateChildren;

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