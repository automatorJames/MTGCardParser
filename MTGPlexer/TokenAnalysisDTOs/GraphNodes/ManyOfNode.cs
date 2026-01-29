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
    }

    public override void ComposeRegexLines(RegexBuilder builder)
    {
        builder.OpenGroup(PropertySnippet.ToTemplatePropInfo(), spaceDisposition: SpaceDisposition.DisallowedLocal);
        ConcatenatingComposer.Instance.Compose(builder, [TemplateNodeForComposition with { DifferentiatorValue = ManyItemOrdinal.First }]);
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
        ConcatenatingComposer.Instance.Compose(builder, [TemplateNodeForComposition with { DifferentiatorValue = ManyItemOrdinal.Last }]);
        builder.CloseGroup();
        builder.CloseGroup();
    }

    protected override object GetPropertyValue(Capture capture)
    {
        //List<PolyItemCapture> hydratedItems = [];
        //MatchTraversalState manyOfContainerState = new(null, parentTokenUnitMatch, LeafName);
        //
        //for (int i = 0; i < _ordinalRegexProps.Length; i++)
        //{
        //    var ordinalProp = _ordinalRegexProps[i];
        //    var manyItemOrdinal = (ManyItemOrdinal)i;
        //
        //    // In manyof captures, "namedGroup" is the parent capture (at the many-of container level),
        //    // but the actual item captures reside in the next level down at the ordinal level.
        //    var sectionGroupCaptures = manyOfContainerState[manyItemOrdinal.ToString()];
        //
        //    // an empty section group should only possibly occur for the second second
        //    if (sectionGroupCaptures.Length == 0)
        //        continue;
        //
        //    // "first" will always have 1 item
        //    // "second" will have any number of items (including 0)
        //    // "last" will always have 1 item
        //    for (int j = 0; j < sectionGroupCaptures.Length; j++)
        //    {
        //        var ordinalCapture = sectionGroupCaptures[j];
        //        var childItem = ordinalProp.GetPropertyValue(manyOfContainerState, ordinalCapture, out var ordinalResult);
        //        PolyItemCapture hydratedItem = new(childItem, ordinalCapture, TemplatePropInfo, distinguishingValue: manyItemOrdinal);
        //        hydratedItems.Add(hydratedItem);
        //    }
        //}
        //
        //var conjunctionCapture = parentTokenUnitMatch[LeafName + "_" + nameof(Conjunction)].SingleOrDefault();
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