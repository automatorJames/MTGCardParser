namespace MTGPlexer.RegexGeneration.RegexSegments;

public record CompoundOfSegment : XOfSegmentBase
{
    CaptureGroupSegmentBase _regexProp;
    public override Regex ManyMatchRegex => TokenTypeRegistry.ManyOfRegexes[GenericType];


    public CompoundOfSegment(TemplatePropInfo captureProp) : base(captureProp)
    {
        var derivedPropInfo = captureProp.DeriveForXOfItem();

        if (GenericType.IsAssignableTo(typeof(TokenUnit)))
            _regexProp = new TokenUnitSegment(derivedPropInfo);
        else if (GenericType.IsEnum)
            _regexProp = new EnumSegment(derivedPropInfo);
        else
            throw new Exception($"CompoundProp base type may only be derived from TokenUnit or be an enum");
    }

    public override void ComposeRegexLines(RegexBuilder builder)
    {
        builder.OpenGroup(TemplatePropInfo);
        builder.OpenGroup(spaceDisposition: SpaceDisposition.DisallowedLocal);
        ConcatenatingComposer.Instance.Compose(builder, [_regexProp]);
        builder.AddTextLine(" ?");
        builder.CloseGroup(GroupQuantifier.OneOrMore);
        builder.CloseGroup();
    }

    public override object GetPropertyValue(MatchTraversalState parentTokenUnitMatch, Capture scopedCapture)
    {
        List<PolyItemCapture> hydratedItems = [];
        var scopedCaptures = parentTokenUnitMatch[LeafName + "_" + TemplatePropInfo.Prop.Name];

        for (int i = 0; i < scopedCaptures.Length; i++)
        {
            var ordinalCapture = scopedCaptures[i];
            MatchTraversalState state = new(GenericType, parentTokenUnitMatch, TemplatePropInfo.Prop.Name, i);
            var childItem = _regexProp.GetPropertyValue(state, ordinalCapture);
            PolyItemCapture hydratedItem = new(childItem, ordinalCapture, TemplatePropInfo);
            hydratedItems.Add(hydratedItem);
        }

        var compoundType = typeof(CompoundOf<>).MakeGenericType(GenericType);
        var compoundPropVal = Activator.CreateInstance(compoundType, hydratedItems);

        return compoundPropVal;
    }


    public override string ToString() => base.ToString();
}