using System.Collections.Immutable;

namespace MTGPlexer.RegexGeneration.RegexSegments;

public record OptionalOfSegment : XOfSegmentBase
{
    TokenUnitSegment _tokenUnitSegment;
    public ImmutableList<RegexSegmentBase> ChildSegments { get; init; }
    public override Regex ManyMatchRegex => TokenTypeRegistry.ManyOfRegexes[GenericType];


    public OptionalOfSegment(TemplatePropInfo captureProp) : base(captureProp)
    {
        var derivedPropInfo = captureProp.DeriveForXOfItem();
        _tokenUnitSegment = new TokenUnitSegment(derivedPropInfo);
        GenericType = TemplatePropInfo.GenericTypes.Single();
        var template = TokenTypeRegistry.GetTypeTemplate(GenericType);
        ChildSegments = template.RegexSegments.ToImmutableList();
    }

    public override void ComposeRegexLines(RegexBuilder builder)
    {
        builder.OpenGroup(TemplatePropInfo);
        ConcatenatingComposer.Instance.Compose(builder, ChildSegments.ToList());
        GroupQuantifier? groupQuantifier = GroupQuantifier.Optional;
        builder.CloseGroup(groupQuantifier);
    }

    public override object GetPropertyValue(MatchTraversalState parentTokenUnitMatch, ExtractedCapture scopedCapture)
    {
        var itemCapture = parentTokenUnitMatch[LeafName + "_" + TemplatePropInfo.Prop.Name].Single();
        MatchTraversalState typeMatch = new(GenericType, parentTokenUnitMatch, TemplatePropInfo.Prop.Name);
        var tokenUnitChild = TokenUnit.InstantiateFromMatch(typeMatch);
        PolyItemCapture hydratedItem = new(tokenUnitChild, itemCapture, TemplatePropInfo);
        var optionalType = typeof(OptionalOf<>).MakeGenericType(GenericType);
        var optionalPropVal = Activator.CreateInstance(optionalType, hydratedItem);

        return optionalPropVal;
    }


    public override string ToString() => base.ToString();
}