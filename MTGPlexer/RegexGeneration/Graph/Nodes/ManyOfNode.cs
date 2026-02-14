namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public class ManyOfNode : WrapperNode
{
    NamedGroupNode _itemTheFirst;
    NamedGroupNode _itemTheSecond;
    NamedGroupNode _itemLast;
    NamedGroupNode _containerTheConjunction;
    public ManyOfNode(RegexNode parentNode, TypeNavigation navigation) 
        : base(parentNode, navigation)
    {
        _itemTheFirst = GetWrappedTokenUnitOrEnumNode(this, GenericType, MultiItemOrdinal.First.ToString());
        _itemTheSecond = GetWrappedTokenUnitOrEnumNode(this, GenericType, MultiItemOrdinal.SecondPlus.ToString());
        _itemLast = GetWrappedTokenUnitOrEnumNode(this, GenericType, MultiItemOrdinal.Last.ToString());
        _containerTheConjunction = GetWrappedTokenUnitOrEnumNode(this, typeof(Conjunction?), nameof(Conjunction));
    }

    public override void AppendRegexBricks(RegexCollector collector)
    {
        collector.Append(GroupOpenBrick);
        {
            _itemTheFirst.AppendRegexBricks(collector);

            collector.Append(AnonymousGroupOpenBrick);
            {
                collector.Append(new RegexBrick(this, ",[ ]", "Oxford comma"));
                _itemTheSecond.AppendRegexBricks(collector);
            }
            collector.Append(GetGroupCloseBrick(GroupQuantifier.AnyNumber));

            collector.Append(AnonymousGroupOpenBrick);
            {
                collector.Append(new RegexBrick(this, ",?[ ]", "optional Oxford comma"));

                collector.Append(AnonymousGroupOpenBrick);
                {
                    _containerTheConjunction.AppendRegexBricks(collector);
                    collector.Append(GetJoinerBrick(Joiner.Space));
                }
                collector.Append(GetGroupCloseBrick(GroupQuantifier.Optional));

                _itemLast.AppendRegexBricks(collector);
            }
            collector.Append(GroupCloseBrick);

        }
        collector.Append(GroupCloseBrick);
    }

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