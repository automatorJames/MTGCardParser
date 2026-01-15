using System.Collections.Immutable;

namespace MTGPlexer.RegexGeneration.RegexSegments;

public record OptionalOfSegment : XOfSegmentBase
{
    public ImmutableList<RegexSegmentBase> ChildSegments { get; init; }
    public override Regex ManyMatchRegex => TokenTypeRegistry.ManyOfRegexes[GenericType];


    public OptionalOfSegment(TemplatePropInfo captureProp) : base(captureProp)
    {
        GenericType = TemplatePropInfo.GenericTypes.Single();
        var derivedPropInfo = captureProp.DeriveForXOfItem();
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

    public override object GetValueToSet(TokenUnit parentTokenUnit, Group namedGroup)
    {
        // In Optional captures, "namedGroup" is the parent capture (at the Optional container level),
        // but the actual item captures reside in the next level down at the prop level.
        var itemContainerCapture = parentTokenUnit.Match[Name + "_" + TemplatePropInfo.Prop.Name];

        CaptureGroupPropPath capturePath = parentTokenUnit.Match.CapturePath.Append(TemplatePropInfo.Name, Name);
        TokenUnitMatch typeMatch = new(GenericType, parentTokenUnit.Match.RegexMatch, parentTokenUnit.Match.SourceText, capturePath);
        var tokenUnitChild = TokenUnit.InstantiateFromMatch(typeMatch);
        PolyItemCapture hydratedItem = new(tokenUnitChild, itemContainerCapture, TemplatePropInfo);
        var optionalType = typeof(OptionalOf<>).MakeGenericType(GenericType);
        var optionalPropVal = Activator.CreateInstance(optionalType, hydratedItem);

        return optionalPropVal;
    }


    public override string ToString() => base.ToString();
}