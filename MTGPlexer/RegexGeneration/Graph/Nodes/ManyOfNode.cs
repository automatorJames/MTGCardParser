namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public class ManyOfNode : WrapperNode
{
    NamedGroupNode _nodeTheFirst;
    NamedGroupNode _nodeTheSecond;
    NamedGroupNode _nodeTheLast;
    NamedGroupNode _nodeTheConjunction;

    public ManyOfNode(RegexNode parentNode, PropNavigation navigation) 
        : base(parentNode, navigation)
    {
    }

    protected override void AddComputedChildren(List<RegexNode> children)
    {
        _nodeTheFirst = GetNamedGroupChild(this, Navigation, GenericType, MultiItemOrdinal.First.ToString());

        AnonymousGroupNode secondItemContainer = new(this, "Second_Item_Container", GroupQuantifier.AnyNumber);
        secondItemContainer.AddWrappedBrickContent("Oxford_Comma", ",[ ]", "Oxford comma");
        _nodeTheSecond = secondItemContainer.AddWrappedNamedGroupChild(Navigation, GenericType, MultiItemOrdinal.SecondPlus.ToString());

        AnonymousGroupNode lastItemContainer = new(this, "Last_Item_Outer_Container");
        lastItemContainer.AddWrappedBrickContent("Optional_Oxford_Comma", ",?[ ]", "optional Oxford comma");

        AnonymousGroupNode conjunctionContainer = new(lastItemContainer, "Conjunction_Container", GroupQuantifier.Optional);
        _nodeTheConjunction = conjunctionContainer.AddWrappedNamedGroupChild(Navigation, typeof(Conjunction?), nameof(Conjunction));

        conjunctionContainer.AddWrappedBrickContent("Conjunction_Space", "[ ]", "joiner space");
        lastItemContainer.AddNode(conjunctionContainer);
        _nodeTheLast = lastItemContainer.AddWrappedNamedGroupChild(Navigation, GenericType, MultiItemOrdinal.Last.ToString());

        children.AddRange([_nodeTheFirst, secondItemContainer, lastItemContainer]);
    }

    //protected override object GetValue(CaptureContext context)
    //{
    //    var scopedContext = GetScopedContext(context);
    //    var firstValue = _nodeTheFirst.GetValueForNamedPath(scopedContext);
    //    var secondPlusValues = _nodeTheSecond.GetValueForNamedPath(scopedContext);
    //
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
}