using MTGPlexer.RegexGeneration.Composers;

namespace MTGPlexer.RegexGeneration.RegexSegments;

/// <summary>
/// Represents a property on a TokenUnit whose property type is also a TokenUnit (i.e. a child TokenUnit). During
/// compilation of a RegexTemplate, thie record simply creates an instance of the child TokenUnit type and gets its
/// rendered Regex string to add it to the parent TokenUnit's own rendered Regex.
/// </summary>
public class TokenRegexProp : CaptureGroupPropBase
{
    public override Regex MatchRegex => TokenTypeRegistry.Templates[RegexPropInfo.UnderlyingType].Regex;

    public List<RegexSegmentBase> ChildSegments { get; set; }

    public TokenRegexProp(RegexPropInfo captureProp) : base(captureProp)
    {
        var template = TokenTypeRegistry.GetTypeTemplate(captureProp.UnderlyingType);
        ChildSegments = template.RegexSegments;
    }

    public override void ComposeRegexLines(RegexLineCollector collector)
    {
        collector.OpenGroup(RegexPropInfo);
        ConcatenatingComposer.Instance.Compose(collector, ChildSegments);
        collector.CloseGroup();
    }

    public override bool SetValueFromMatch(TokenUnit token, Match match)
    {
        var capture = match.Groups[Name];

        if (capture == null)
            return false;

        var ancestorCapturePath = token.CapturePath.Dot(RegexPropInfo.Name);
        var tokenUnitInstance = TokenUnit.HydrateAsChildFromCapture(RegexPropInfo.BaseType, match, capture, ancestorCapturePath);
        token.SetPropertyFromCapture(RegexPropInfo, capture, tokenUnitInstance);

        return true;
    }

    public override string ToString() => base.ToString();
}