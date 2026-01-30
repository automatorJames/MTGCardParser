namespace MTGPlexer.RegexGeneration.RegexSegments;

public record ManyOfNode : WrapperPropertyNode
{
    WrappedNode _conjunctionNode { get; }
    WrappedNode _nodeTheFirst { get; }
    WrappedNode _nodeTheLast { get; }
    List<WrappedNode> _nodesTheSecond { get; } = [];

    public ManyOfNode(Node parentNode, PropertySnippet propertySnippet) : base(parentNode, propertySnippet)
    {
        _conjunctionNode = new(this, typeof(Conjunction));
        _nodeTheFirst = new(this, GenericType, 0, ManyItemOrdinal.First);
        _nodeTheLast = new(this, GenericType, 0, ManyItemOrdinal.Last);
    }

    public override void ComposeRegexLines(RegexBuilder builder)
    {
        var thing = Children;

        builder.OpenGroup(PropertySnippet.ToTemplatePropInfo(), spaceDisposition: SpaceDisposition.DisallowedLocal);
        ConcatenatingComposer.Instance.Compose(builder, [_nodeTheFirst]);
        builder.OpenGroup(spaceDisposition: SpaceDisposition.DisallowedLocal);
        builder.AddTextLine(", ");
        ConcatenatingComposer.Instance.Compose(builder, [TemplateNodeForComposition with { DifferentiatorValue = ManyItemOrdinal.SecondPlus }]);
        builder.CloseGroup(GroupQuantifier.AnyNumber);
        builder.OpenGroup(spaceDisposition: SpaceDisposition.DisallowedLocal);
        builder.AddTextLine(",? ");
        builder.OpenGroup(spaceDisposition: SpaceDisposition.DisallowedLocal);
        _conjunctionNode.ComposeRegexLines(builder);
        builder.AddTextLine(" ");
        builder.CloseGroup(GroupQuantifier.Optional);
        ConcatenatingComposer.Instance.Compose(builder, [_nodeTheLast]);
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