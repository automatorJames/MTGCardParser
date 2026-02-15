namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public class ManyOfNode : WrapperNode
{
    NamedGroupNode _nodeTheFirst;
    NamedGroupNode _nodeTheSecond;
    NamedGroupNode _nodeTheLast;
    NamedGroupNode _nodeTheConjunction;

    List<RegexNode> _immediateChildren;

    public ManyOfNode(RegexNode parentNode, PropNavigation navigation) 
        : base(parentNode, navigation)
    {
        SetChildNodes(navigation);
    }

    void SetChildNodes(PropNavigation navigation)
    {
        _nodeTheFirst = GetNamedGroupChild(this, navigation, GenericType, MultiItemOrdinal.First.ToString());

        AnonymousGroupNode secondItemContainer = new(this, "Second-Item-Container", GroupQuantifier.AnyNumber);
        secondItemContainer.AddWrappedBrick("Oxford-Comma", new RegexBrick(this, ",[ ]", "Oxford comma"));
        _nodeTheSecond = secondItemContainer.AddWrappedNamedGroupChild(navigation, GenericType, MultiItemOrdinal.SecondPlus.ToString());

        AnonymousGroupNode lastItemContainer = new(this, "Last-Item-Outer-Container");
        lastItemContainer.AddWrappedBrick("Optional-Oxford-Comma", new RegexBrick(this, ",?[ ]", "optional Oxford comma"));

        AnonymousGroupNode conjunctionContainer = new(this, "Conjunction-Container", GroupQuantifier.Optional);
        _nodeTheConjunction = conjunctionContainer.AddWrappedNamedGroupChild(navigation, typeof(Conjunction?), nameof(Conjunction));

        lastItemContainer.AddWrappedBrick("Conjunction-Space", new RegexBrick(this, "[ ]", "joiner space"));
        lastItemContainer.AddNode(conjunctionContainer);
        _nodeTheLast = lastItemContainer.AddWrappedNamedGroupChild(navigation, GenericType, MultiItemOrdinal.Last.ToString());

        _immediateChildren = [_nodeTheFirst, secondItemContainer, lastItemContainer];
    }

    protected override List<RegexNode> GetChildNodes() => _immediateChildren;

    //protected override object GetWrapperValue(CaptureContext captureContext)
    //{
    //    var captureContextTheFirst = captureContext[_itemTheFirst.FullyQualifiedName];
    //    var captureContextTheSecond = captureContext[_itemTheSecond.FullyQualifiedName];
    //    var captureContextTheLast = captureContext[_itemLast.FullyQualifiedName];
    //    var captureContextTheConjunction = captureContext[_containerTheConjunction.FullyQualifiedName];
    //
    //    WrappedNodes.Add(_itemTheFirst.HydrateChild(captureContextTheFirst));
    //    WrappedNodes.Add(_itemLast.HydrateChild(captureContextTheLast));
    //
    //    // Second may contain any number, including 0
    //    for (int i = 0; i < captureContextTheSecond.Captures.Length; i++)
    //    {
    //        var secondOrdinalWrapper = new VirtualNamedGroupNode(this, ManyItemOrdinal.SecondPlus.ToString(), GenericType);
    //        var scopedContext = captureContextTheSecond.ScopeToCaptureIndex(i);
    //        WrappedNodes.Add(secondOrdinalWrapper.HydrateChild(scopedContext));
    //    }
    //
    //    // before we add the conjunction value to WrappedNodes, extract them into a ManyItem value list for convenience
    //    var manyItemValues = WrappedNodes.Select(x => x.CaptureValueHydrationInfo.Value).ToList();
    //    object nullableConjunctionItemValue = null;
    //
    //    if (captureContextTheConjunction.Success)
    //        nullableConjunctionItemValue = AddNewWrappedNode(captureContextTheConjunction, genericType: typeof(Conjunction)).CaptureValueHydrationInfo.Value;
    //
    //    return CreateWrapperValue(manyItemValues, nullableConjunctionItemValue);
    //}

    public override string ToString() => base.ToString();
}