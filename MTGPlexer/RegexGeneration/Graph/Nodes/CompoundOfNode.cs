namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public class CompoundOfNode : WrapperNode
{
    protected override Joiner Joiner => Joiner.Pipe;
    protected override GroupQuantifier? Quantifier => GroupQuantifier.OneOrMore;

    public CompoundOfNode(RegexNode parentNode, PropNavigation navigation) 
        : base(parentNode, navigation)
    {
    }

    protected override void AddComputedChildren(List<RegexNode> children)
    {
        var localJoinerBetweenTerms = Navigation.UnderlyingType.GetCustomAttribute<CompoundJoinerAttribute>()?.Joiner
            ?? Joiner.Space;

        if (localJoinerBetweenTerms == Joiner.None)
        {
            AnonymousGroupNode twoOrMoreWrapper = new(this, "Two-Or-More", GroupQuantifier.TwoOrMore);
            var nodeTheFirst = twoOrMoreWrapper.AddWrappedNamedGroupChild(Navigation, GenericType, MultiItemOrdinal.First.ToString());
            children.Add(twoOrMoreWrapper);
        }
        else
        {
            var nodeTheFirst = GetNamedGroupChild(this, Navigation, GenericType, MultiItemOrdinal.First.ToString());
            AnonymousGroupNode secondItemOneOrMoreWrapper = new(this, "One-Or-More", GroupQuantifier.TwoOrMore);
            secondItemOneOrMoreWrapper.AddWrappedBrickContent("Joiner", localJoinerBetweenTerms.GetDescription(), $"joiner {localJoinerBetweenTerms}");
            var nodeTheSecond = secondItemOneOrMoreWrapper.AddWrappedNamedGroupChild(Navigation, GenericType, MultiItemOrdinal.SecondPlus.ToString());
            children.AddRange([nodeTheFirst, secondItemOneOrMoreWrapper]);
        }
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