using System.Collections.Immutable;
namespace MTGPlexer.RegexGeneration.RegexSegments;

/// <summary>
/// Represents a property on a TokenUnit whose property type is also a TokenUnit (i.e. a child TokenUnit). During
/// compilation of a RegexTemplate, thie record simply creates an instance of the child TokenUnit type and gets its
/// rendered Regex string to add it to the parent TokenUnit's own rendered Regex.
/// </summary>
public record TokenRegexProp : CaptureGroupPropBase
{
    public override Regex ManyMatchRegex => TokenTypeRegistry.Templates[TemplatePropInfo.BaseType].Regex;
    public ImmutableList<RegexSegmentBase> ChildSegments { get; init; }

    public TokenRegexProp(TemplatePropInfo captureProp) : base(captureProp)
    {
        var template = TokenTypeRegistry.GetTypeTemplate(captureProp.BaseType);
        ChildSegments = template.RegexSegments.ToImmutableList();
    }

    public override void ComposeRegexLines(RegexBuilder builder)
    {
        builder.OpenGroup(TemplatePropInfo);
        ConcatenatingComposer.Instance.Compose(builder, ChildSegments.ToList());
        var groupIsOptional = TemplatePropInfo.Prop.IsDefined(typeof(OptionalComponentAttribute));
        GroupQuantifier? groupQuantifier = groupIsOptional ? GroupQuantifier.Optional : null;
        builder.CloseGroup(groupQuantifier);
    }

    public override object GetValueToSet(TokenUnit parentTokenUnit, Group namedGroup)
    {
        CaptureGroupPropPath ancestorCapturePath = new(parentTokenUnit.Match.CapturePath.PropPath.Dot(TemplatePropInfo.Name));
        TokenUnitMatch typeMatch = new(TemplatePropInfo.BaseType, parentTokenUnit.Match.RegexMatch, parentTokenUnit.Match.SourceText, ancestorCapturePath);
        var tokenUnitInstance = TokenUnit.InstantiateFromMatch(typeMatch);
        parentTokenUnit.ChildTokenUnits.Add(tokenUnitInstance);

        return tokenUnitInstance;
    }

    public override string ToString() => base.ToString();
}