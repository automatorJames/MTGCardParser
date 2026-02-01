namespace MTGPlexer.RegexGeneration.GraphNodes;

public class ManyOfNode : WrapperNode
{
    public ManyOfNode(Node parentNode, PropertySnippet propertySnippet) : base(parentNode, propertySnippet)
    {
    }

    public override void ComposeRegexLines(RegexBuilder builder)
    {
        builder.OpenNamedGroup(this, spaceDisposition: SpaceDisposition.DisallowedLocal);
        ConcatenatingComposer.Instance.Compose(builder, [GetTemplateNodeForType(differentiatorValue: ManyItemOrdinal.First)]);
        builder.OpenAnonymousGroup(spaceDisposition: SpaceDisposition.DisallowedLocal);
        builder.AddTextLine(", ");
        ConcatenatingComposer.Instance.Compose(builder, [GetTemplateNodeForType(differentiatorValue: ManyItemOrdinal.SecondPlus)]);
        builder.CloseGroup(GroupQuantifier.AnyNumber);
        builder.OpenAnonymousGroup(spaceDisposition: SpaceDisposition.DisallowedLocal);
        builder.AddTextLine(",? ");
        builder.OpenAnonymousGroup(spaceDisposition: SpaceDisposition.DisallowedLocal);
        new WrappedNode(this, typeof(Conjunction?)).ComposeRegexLines(builder);
        builder.AddTextLine(" ");
        builder.CloseGroup(GroupQuantifier.Optional);
        ConcatenatingComposer.Instance.Compose(builder, [GetTemplateNodeForType(differentiatorValue: ManyItemOrdinal.Last)]);
        builder.CloseGroup();
        builder.CloseGroup();
    }

    public override object TryGetValue(CaptureDictionary captureDictionary, out CaptureValueResult result)
    {
        var captureTheFirst = captureDictionary[FullyQualifiedName + "_" + ManyItemOrdinal.First].Single();
        var capturesTheSecond = captureDictionary[FullyQualifiedName + "_" + ManyItemOrdinal.SecondPlus];
        var captureTheLast = captureDictionary[FullyQualifiedName + "_" + ManyItemOrdinal.Last].Single();
        var captureTheConjunction = captureDictionary[FullyQualifiedName + "_" + nameof(Conjunction)].SingleOrDefault();

        AddNewWrappedNode(captureTheFirst, differentiatorValue: ManyItemOrdinal.First);

        for (int i = 0; i < capturesTheSecond.Length; i++)
        {
            Capture ordinalCapture = capturesTheSecond[i];
            AddNewWrappedNode(ordinalCapture, ordinal: i, differentiatorValue: ManyItemOrdinal.SecondPlus);
        }

        AddNewWrappedNode(captureTheLast, differentiatorValue: ManyItemOrdinal.Last);

        // before we add the conjunction value to WrappedNodes, extract them into a ManyItem value list for convenience
        var manyItemValues = WrappedNodes.Select(x => x.CaptureValueHydrationInfo.Value).ToList();

        if (captureTheConjunction != null)
            AddNewWrappedNode(captureTheConjunction, genericType: typeof(Conjunction));

        result = CaptureValueResult.FoundWithValue;
        return CreateWrapperValue();
    }

    public override string ToString() => base.ToString();
}