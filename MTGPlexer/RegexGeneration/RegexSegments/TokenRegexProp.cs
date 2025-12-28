using System.Collections.Immutable;
namespace MTGPlexer.RegexGeneration.RegexSegments;

/// <summary>
/// Represents a property on a TokenUnit whose property type is also a TokenUnit (i.e. a child TokenUnit). During
/// compilation of a RegexTemplate, thie record simply creates an instance of the child TokenUnit type and gets its
/// rendered Regex string to add it to the parent TokenUnit's own rendered Regex.
/// </summary>
public record TokenRegexProp : CaptureGroupPropBase
{
    public override Regex ManyMatchRegex => TokenTypeRegistry.Templates[RegexPropInfo.BaseType].Regex;
    public ImmutableList<RegexSegmentBase> ChildSegments { get; init; }

    public TokenRegexProp(RegexPropInfo captureProp) : base(captureProp)
    {
        var template = TokenTypeRegistry.GetTypeTemplate(captureProp.BaseType);
        ChildSegments = template.RegexSegments.ToImmutableList();
    }

    public override void ComposeRegexLines(RegexBuilder builder)
    {
        builder.OpenGroup(RegexPropInfo);
        ConcatenatingComposer.Instance.Compose(builder, ChildSegments.ToList());
        var groupIsOptional = RegexPropInfo.Prop.IsDefined(typeof(OptionalComponentAttribute));
        GroupQuantifier? groupQuantifier = groupIsOptional ? GroupQuantifier.Optional : null;
        builder.CloseGroup(groupQuantifier);
    }

    public override bool SetValueFromNamedGroupInMatch(TokenUnit token)
    {
        var capture = token.Match.GetCaptureAtRelativePath(this);

        if (capture == null)
            return false;

        CaptureGroupPropPath ancestorCapturePath = new(token.Match.CapturePath.PropPath.Dot(RegexPropInfo.Name));
        TokenUnitMatch typeMatch = new(RegexPropInfo.BaseType, token.Match.RegexMatch, token.Match.SourceText, ancestorCapturePath, token.Match.CaptureIndex);
        var tokenUnitInstance = TokenUnit.InstantiateFromMatch(typeMatch);
        token.ChildTokenUnits.Add(tokenUnitInstance);
        token.SetPropertyFromCapture(RegexPropInfo, capture, tokenUnitInstance);

        return true;
    }

    public override string ToString() => base.ToString();
}