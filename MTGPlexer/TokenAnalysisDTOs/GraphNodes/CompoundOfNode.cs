namespace MTGPlexer.RegexGeneration.RegexSegments;

public record CompoundOfNode : WrapperPropertyNode
{
    public CollectionNode Collection { get; set; }
    public CompoundOfNode(PropertySnippet propertySnippet) : base(propertySnippet)
    {
        var genericType = GenericTypes[0];
        Collection = new CollectionNode(genericType);

        if (genericType.IsAssignableTo(typeof(TokenUnit)))
            _regexProp = new TokenUnitNode(derivedPropInfo);
        else if (GenericType.IsEnum)
            _regexProp = new EnumNode(derivedPropInfo);
        else
            throw new Exception($"CompoundOfNode type may only be derived from TokenUnit or be an enum");
    }

    public override void ComposeRegexLines(RegexBuilder builder)
    {
        builder.OpenGroup(TemplatePropInfo);
        builder.OpenGroup(spaceDisposition: SpaceDisposition.DisallowedLocal);
        ConcatenatingComposer.Instance.Compose(builder, [_regexProp]);
        builder.AddTextLine(" ?");
        builder.CloseGroup(GroupQuantifier.OneOrMore);
        builder.AddNegativeSpaceLookbehindBoundary();
        builder.CloseGroup();
    }

    public override object SetPropertyValue(MatchTraversalState parentTokenUnitMatch, ExtractedCapture scopedCapture, out ValueResult result)
    {
        List<PolyItemCapture> hydratedItems = [];
        var scopedCaptures = parentTokenUnitMatch[LeafName + "_" + TemplatePropInfo.Prop.Name];

        for (int i = 0; i < scopedCaptures.Length; i++)
        {
            var ordinalCapture = scopedCaptures[i];
            MatchTraversalState state = new(GenericType, parentTokenUnitMatch, TemplatePropInfo.Prop.Name);
            var childItem = _regexProp.GetPropertyValue(state, ordinalCapture, out var ordinalResult);
            PolyItemCapture hydratedItem = new(childItem, ordinalCapture, TemplatePropInfo);
            hydratedItems.Add(hydratedItem);
        }

        var compoundType = typeof(CompoundOf<>).MakeGenericType(GenericType);
        var compoundPropVal = Activator.CreateInstance(compoundType, hydratedItems);

        result = ValueResult.Success;
        return compoundPropVal;
    }


    public override string ToString() => base.ToString();
}