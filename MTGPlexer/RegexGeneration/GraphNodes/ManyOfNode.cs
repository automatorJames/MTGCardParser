namespace MTGPlexer.RegexGeneration.GraphNodes;

public class ManyOfNode : WrapperNode
{
    VirtualNode _containerTheFirst;
    VirtualNode _containerTheSecond;
    VirtualNode _containerTheLast;
    WrappedNode _containerTheConjunction;
    public ManyOfNode(Node parentNode, PropertySnippet propertySnippet) : base(parentNode, propertySnippet)
    {
        _containerTheFirst = new VirtualNode(this, ManyItemOrdinal.First.ToString()).AddChild(GetTemplateNodeForType());
        _containerTheSecond = new VirtualNode(this, ManyItemOrdinal.SecondPlus.ToString()).AddChild(GetTemplateNodeForType());
        _containerTheLast = new VirtualNode(this, ManyItemOrdinal.Last.ToString()).AddChild(GetTemplateNodeForType());
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


    public override object TryGetValue(CaptureDictionary captureDictionary, out CaptureValueResult result)
    {
        var captureTheFirst = captureDictionary[_containerTheFirst.FullyQualifiedName].Single();
        var capturesTheSecond = captureDictionary[_containerTheSecond.FullyQualifiedName];
        var captureTheLast = captureDictionary[_containerTheLast.FullyQualifiedName].Single();
        var captureTheConjunction = captureDictionary[_containerTheConjunction.FullyQualifiedName].SingleOrDefault();

        WrappedNodes.Add(_containerTheFirst.HydrateFromCapture(captureTheFirst));
        WrappedNodes.Add(_containerTheLast.HydrateFromCapture(captureTheLast));

        // Second may contain any number, including 0
        for (int i = 0; i < capturesTheSecond.Length; i++)
        {
            Capture ordinalCapture = capturesTheSecond[i];
            var secondOrdinalWrapper = new VirtualNode(this, ManyItemOrdinal.SecondPlus.ToString()).AddChild(GetTemplateNodeForType());
            WrappedNodes.Add(secondOrdinalWrapper.HydrateFromCapture(ordinalCapture));
        }

        //// before we add the conjunction value to WrappedNodes, extract them into a ManyItem value list for convenience
        //var manyItemValues = WrappedNodes.Select(x => x.CaptureValueHydrationInfo.Value).ToList();

        if (captureTheConjunction != null)
            AddNewWrappedNode(captureTheConjunction, genericType: typeof(Conjunction));

        result = CaptureValueResult.FoundWithValue;
        return CreateWrapperValue();
    }

    public override string ToString() => base.ToString();
}