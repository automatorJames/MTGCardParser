using System.Collections.Immutable;

namespace MTGPlexer.RegexGeneration.RegexSegments;

public record OptionalOfSegment : CaptureGroupSegmentBase
{
    public Type BaseType { get; set; }
    public ImmutableList<RegexSegmentBase> ChildSegments { get; init; }
    public override Regex ManyMatchRegex => TokenTypeRegistry.ManyOfRegexes[BaseType];


    public OptionalOfSegment(TemplatePropInfo captureProp) : base(captureProp)
    {
        // TemplatePropInfo capture prop is a OptionalOf<T> prop here

        BaseType = captureProp.BaseType;
        var derivedPropInfo = captureProp with { TemplatePropType = TemplatePropInfo.GetRegexPropType(TemplatePropInfo.BaseType) };
        var template = TokenTypeRegistry.GetTypeTemplate(captureProp.BaseType);
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
        var polyItemCaptureType = typeof(PolyItemCapture<>).MakeGenericType(BaseType);

        // In Optional captures, "namedGroup" is the parent capture (at the Optional container level),
        // but the actual item captures reside in the next level down at the prop level.
        var itemContainerCapture = parentTokenUnit.Match[Name + "_" + TemplatePropInfo.Prop.Name];

        CaptureGroupPropPath capturePath = parentTokenUnit.Match.CapturePath.Append(TemplatePropInfo.Name, Name);
        TokenUnitMatch typeMatch = new(BaseType, parentTokenUnit.Match.RegexMatch, parentTokenUnit.Match.SourceText, capturePath);
        var tokenUnitChild = TokenUnit.InstantiateFromMatch(typeMatch);
        var hydratedItem = Activator.CreateInstance(polyItemCaptureType, tokenUnitChild, itemContainerCapture, TemplatePropInfo);
        var optionalType = typeof(OptionalOf<>).MakeGenericType(BaseType);
        var optionalPropVal = Activator.CreateInstance(optionalType, hydratedItem);

        return optionalPropVal;
    }


    public override string ToString() => base.ToString();
}