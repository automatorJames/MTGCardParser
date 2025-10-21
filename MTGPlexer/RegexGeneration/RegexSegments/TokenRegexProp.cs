namespace MTGPlexer.RegexGeneration.RegexSegments;

/// <summary>
/// Represents a property on a TokenUnit whose property type is also a TokenUnit (i.e. a child TokenUnit). During
/// compilation of a RegexTemplate, thie record simply creates an instance of the child TokenUnit type and gets its
/// rendered Regex string to add it to the parent TokenUnit's own rendered Regex.
/// </summary>
public class TokenRegexProp : CaptureGroupPropBase
{
    public override Regex MatchRegex => TokenTypeRegistry.Templates[RegexPropInfo.BaseType].Regex;

    public List<RegexSegmentBase> ChildSegments { get; set; }

    public TokenRegexProp(RegexPropInfo captureProp) : base(captureProp)
    {
        var template = TokenTypeRegistry.GetTypeTemplate(captureProp.BaseType);
        ChildSegments = template.RegexSegments;
    }

    public override void ComposeRegexLines(RegexBuilder builder)
    {
        builder.OpenGroup(RegexPropInfo);
        ConcatenatingComposer.Instance.Compose(builder, ChildSegments);
        builder.CloseGroup();
    }

    public override bool SetValueFromMatch(TokenUnit token, Match match)
    {
        var capture = match.Groups[Name];

        if (capture == null)
            return false;

        var ancestorCapturePath = token.CapturePath.Dot(RegexPropInfo.Name);
        var tokenUnitInstance = token.HydrateAsChildFromCapture(RegexPropInfo.BaseType, match, capture, ancestorCapturePath);
        token.SetPropertyFromCapture(RegexPropInfo, capture, tokenUnitInstance);

        return true;
    }

    public override string ToString() => base.ToString();
}