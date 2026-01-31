namespace MTGPlexer.RegexGeneration.GraphNodes;

public class ManyOfNode : WrapperPropertyNode
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

    public override object GetValue(Capture[] captures)
    {
        //List<PolyItemCapture> hydratedItems = [];
        //
        //for (int i = 0; i < captures.Length; i++)
        //{
        //    var manyItemOrdinal =
        //        i == 0 ? ManyItemOrdinal.First
        //        : i == captures.Length - 1 ? ManyItemOrdinal.Last
        //        : ManyItemOrdinal.SecondPlus;
        //
        //    WrappedNode wrappedNode = new(this, GenericType, i, manyItemOrdinal);
        //    var capture = captures[i];
        //    ExtractedCapture extractedCapture = new(capture, FullyQualifiedName, i, captures.Length); // todo: deprecate ExtractedCapture?
        //    var wrappedNodeValue = wrappedNode.GetValue(capture);
        //    PolyItemCapture hydratedItem = new(wrappedNodeValue, extractedCapture, PropertySnippet.ToTemplatePropInfo(), distinguishingValue: manyItemOrdinal);
        //    hydratedItems.Add(hydratedItem);
        //}
        //
        //var conjunctionCapture = captures[FullyQualifiedName + "_" + nameof(Conjunction)].SingleOrDefault();
        //
        //Conjunction? conjunctionValue = conjunctionCapture == null ? null
        //    : Enum.TryParse<Conjunction>(conjunctionCapture.Value, true, out var parsed)
        //    ? parsed : null;
        //
        //var manyTokenType = typeof(ManyOf<>).MakeGenericType(TemplatePropInfo.GenericTypes);
        //var manyPropVal = Activator.CreateInstance(manyTokenType, hydratedItems, conjunctionValue, conjunctionCapture);
        //
        //result = ValueResult.Success;
        //return manyPropVal;

        throw new NotImplementedException();
    }

    public override string ToString() => base.ToString();
}