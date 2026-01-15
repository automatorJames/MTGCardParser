using System.Collections.Immutable;

namespace MTGPlexer.RegexGeneration.RegexSegments;

public record OptionalOfProp : CaptureGroupPropBase
{
    CaptureGroupPropBase _regexProp;
    public Type BaseType { get; set; }
    public ImmutableList<RegexSegmentBase> ChildSegments { get; init; }
    public override Regex ManyMatchRegex => TokenTypeRegistry.ManyOfRegexes[BaseType];


    public OptionalOfProp(TemplatePropInfo captureProp) : base(captureProp)
    {
        // RegexPropInfo capture prop is a OptionalOf<T> prop here

        BaseType = captureProp.BaseType;
        var derivedPropInfo = captureProp with { TemplatePropType = TemplatePropInfo.GetRegexPropType(RegexPropInfo.BaseType) };
        var template = TokenTypeRegistry.GetTypeTemplate(captureProp.BaseType);
        ChildSegments = template.RegexSegments.ToImmutableList();
    }

    public override void ComposeRegexLines(RegexBuilder builder)
    {
        builder.OpenGroup(RegexPropInfo);
        ConcatenatingComposer.Instance.Compose(builder, ChildSegments.ToList());
        GroupQuantifier? groupQuantifier = GroupQuantifier.Optional;
        builder.CloseGroup(groupQuantifier);
    }

    public override object GetValueToSet(TokenUnit parentTokenUnit, Group namedGroup)
    {
        var optionalItemCaptureType = typeof(PolyItemCapture<>).MakeGenericType(BaseType);

        // In Optional captures, "namedGroup" is the parent capture (at the Optional container level),
        // but the actual item captures reside in the next level down at the prop level.
        var itemContainerCapture = parentTokenUnit.Match[Name + "_" + RegexPropInfo.Prop.Name];

        CaptureGroupPropPath capturePath = parentTokenUnit.Match.CapturePath.Append(RegexPropInfo.Name, _regexProp.Name);
        TokenUnitMatch typeMatch = new(BaseType, parentTokenUnit.Match.RegexMatch, parentTokenUnit.Match.SourceText, capturePath);
        var tokenUnitChild = TokenUnit.InstantiateFromMatch(typeMatch);
        var hydratedItem = Activator.CreateInstance(optionalItemCaptureType, tokenUnitChild, itemContainerCapture, 0, RegexPropInfo);
        var optionalType = typeof(OptionalOf<>).MakeGenericType(BaseType);
        var optionalPropVal = Activator.CreateInstance(optionalType, hydratedItem);

        return optionalPropVal;
    }


    public override string ToString() => base.ToString();
}