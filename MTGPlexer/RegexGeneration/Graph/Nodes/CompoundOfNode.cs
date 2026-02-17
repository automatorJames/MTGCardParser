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
            AnonymousGroupNode twoOrMoreWrapper = new(this, "Two_Or_More", GroupQuantifier.TwoOrMore);
            var nodeTheFirst = twoOrMoreWrapper.AddWrappedNamedGroupChild(Navigation, GenericType, MultiItemOrdinal.First.ToString());
            children.Add(twoOrMoreWrapper);
        }
        else
        {
            var nodeTheFirst = GetNamedGroupChild(this, Navigation, GenericType, MultiItemOrdinal.First.ToString());
            AnonymousGroupNode secondItemOneOrMoreWrapper = new(this, "One_Or_More", GroupQuantifier.TwoOrMore);
            secondItemOneOrMoreWrapper.AddWrappedBrickContent("Joiner", localJoinerBetweenTerms.GetDescription(), $"joiner {localJoinerBetweenTerms}");
            var nodeTheSecond = secondItemOneOrMoreWrapper.AddWrappedNamedGroupChild(Navigation, GenericType, MultiItemOrdinal.SecondPlus.ToString());
            children.AddRange([nodeTheFirst, secondItemOneOrMoreWrapper]);
        }
    }

    //protected override object GetValue(CaptureContext scopedContext)
    //{
    //    // We expect two or more captures
    //    if (scopedContext.Count < 2)
    //        return null;
    //
    //    for (int i = 0; i < scopedContext.Count; i++)
    //    {
    //        var singleScopedContext = scopedCaptureContext.ScopeToCaptureIndex(i);
    //        AddNewWrappedNode(singleScopedContext, ordinal: i);
    //    }
    //
    //    return CreateWrapperValue();
    //}
}