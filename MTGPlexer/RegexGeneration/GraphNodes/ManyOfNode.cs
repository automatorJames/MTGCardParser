namespace MTGPlexer.RegexGeneration.GraphNodes;

public class ManyOfNode : WrapperNode
{
    VirtualNode _containerTheFirst;
    VirtualNode _containerTheSecond;
    VirtualNode _containerTheLast;
    WrappedNode _containerTheConjunction;
    public ManyOfNode(Node parentNode, PropertySnippet propertySnippet) : base(parentNode, propertySnippet)
    {
        _containerTheFirst = new VirtualNode(this, ManyItemOrdinal.First.ToString(), GenericType);
        _containerTheSecond = new VirtualNode(this, ManyItemOrdinal.SecondPlus.ToString(), GenericType);
        _containerTheLast = new VirtualNode(this, ManyItemOrdinal.Last.ToString(), GenericType);
        _containerTheConjunction = new WrappedNode(this, typeof(Conjunction?));
    }

    public override void ComposeRegexLines(RegexBuilder builder)
    {
        builder.OpenNamedGroup(this, spaceDisposition: SpaceDisposition.DisallowedLocal);
        {
            _containerTheFirst.ComposeRegexLines(builder);

            builder.OpenAnonymousGroup(spaceDisposition: SpaceDisposition.DisallowedLocal);
            {
                builder.AddTextLine(", ");
                _containerTheSecond.ComposeRegexLines(builder);
                builder.CloseGroup(GroupQuantifier.AnyNumber);
            }

            builder.OpenAnonymousGroup(spaceDisposition: SpaceDisposition.DisallowedLocal);
            {
                builder.AddTextLine(",? ");

                builder.OpenAnonymousGroup(spaceDisposition: SpaceDisposition.DisallowedLocal);
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
            var secondOrdinalWrapper = new VirtualNode(this, ManyItemOrdinal.SecondPlus.ToString(), GenericType);
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