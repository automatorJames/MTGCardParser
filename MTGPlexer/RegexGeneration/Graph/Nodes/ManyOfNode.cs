namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public class ManyOfNode : WrapperNode
{
    VirtualNamedGroupNode _containerTheFirst;
    VirtualNamedGroupNode _containerTheSecond;
    VirtualNamedGroupNode _containerTheLast;
    WrappedNode _containerTheConjunction;
    public ManyOfNode(RegexNode parentNode, PropertySnippet propertySnippet) : base(parentNode, propertySnippet)
    {
        _containerTheFirst = new VirtualNamedGroupNode(this, ManyItemOrdinal.First.ToString(), GenericType);
        _containerTheSecond = new VirtualNamedGroupNode(this, ManyItemOrdinal.SecondPlus.ToString(), GenericType);
        _containerTheLast = new VirtualNamedGroupNode(this, ManyItemOrdinal.Last.ToString(), GenericType);
        _containerTheConjunction = new WrappedNode(this, typeof(Conjunction?));
    }

    public override void AppendRegexBricks(RegexCollector collector)
    {
        builder.OpenNamedGroup(this);
        {
            _containerTheFirst.ComposeRegexLines(builder);

            builder.OpenAnonymousGroup();
            {
                builder.AddTextLine(", ");
                _containerTheSecond.ComposeRegexLines(builder);
                builder.CloseGroup(GroupQuantifier.AnyNumber);
            }

            builder.OpenAnonymousGroup();
            {
                builder.AddTextLine(",? ");

                builder.OpenAnonymousGroup();
                {
                    _containerTheConjunction.ComposeRegexLines(builder);
                    builder.AddTextLine(" ");
                    builder.CloseGroup(GroupQuantifier.Optional);
                }

                _containerTheLast.ComposeRegexLines(builder);
                builder.CloseGroup();
            }

            builder.CloseGroup();
        }
    }


    protected override object GetWrapperValue(CaptureContext captureContext)
    {
        var captureContextTheFirst = captureContext[_containerTheFirst.FullyQualifiedName];
        var captureContextTheSecond = captureContext[_containerTheSecond.FullyQualifiedName];
        var captureContextTheLast = captureContext[_containerTheLast.FullyQualifiedName];
        var captureContextTheConjunction = captureContext[_containerTheConjunction.FullyQualifiedName];

        WrappedNodes.Add(_containerTheFirst.HydrateChild(captureContextTheFirst));
        WrappedNodes.Add(_containerTheLast.HydrateChild(captureContextTheLast));

        // Second may contain any number, including 0
        for (int i = 0; i < captureContextTheSecond.Captures.Length; i++)
        {
            var secondOrdinalWrapper = new VirtualNamedGroupNode(this, ManyItemOrdinal.SecondPlus.ToString(), GenericType);
            var scopedContext = captureContextTheSecond.ScopeToCaptureIndex(i);
            WrappedNodes.Add(secondOrdinalWrapper.HydrateChild(scopedContext));
        }

        // before we add the conjunction value to WrappedNodes, extract them into a ManyItem value list for convenience
        var manyItemValues = WrappedNodes.Select(x => x.CaptureValueHydrationInfo.Value).ToList();
        object nullableConjunctionItemValue = null;

        if (captureContextTheConjunction.Success)
            nullableConjunctionItemValue = AddNewWrappedNode(captureContextTheConjunction, genericType: typeof(Conjunction)).CaptureValueHydrationInfo.Value;

        return CreateWrapperValue(manyItemValues, nullableConjunctionItemValue);
    }

    public override string ToString() => base.ToString();
}