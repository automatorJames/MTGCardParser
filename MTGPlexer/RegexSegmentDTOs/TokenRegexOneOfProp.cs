using MTGPlexer.RegexSegmentDTOs.Composers;

namespace MTGPlexer.RegexSegmentDTOs;

/// <summary>
/// Represents a property on a TokenUnit whose property type is also some TokenUnit (i.e. a child TokenUnit). During
/// compilation of a RegexTemplate, thie record simply creates an instance of the child TokenUnit type and gets its
/// rendered Regex string to add it to the parent TokenUnit's own rendered Regex.
/// </summary>
public class TokenRegexOneOfProp : TokenRegexProp
{
    public TokenRegexOneOfProp(RegexPropInfo captureProp) : base(captureProp)
    {
        var template = TokenTypeRegistry.GetTypeTemplate(captureProp.UnderlyingType);
        ChildSegments = template.RegexSegments;
    }

    public override bool SetValueFromMatchSpan(TokenUnit parentToken, TextSpan matchSpan)
    {
        var subMatchSpan = GetPropSubMatch(matchSpan);

        if (subMatchSpan is null)
            throw new Exception($"No alternative for {nameof(TokenUnitOneOf)} type '{RegexPropInfo.UnderlyingType.Name}' matched '{matchSpan.ToStringValue()}'");

        var oneOfPropInstance = TokenUnit.InstantiateFromMatchString(RegexPropInfo.UnderlyingType, subMatchSpan.Value, parentToken, RegexPropInfo);
        parentToken.SetPropertyCapture(RegexPropInfo, subMatchSpan.Value, oneOfPropInstance);
        return true;
    }

    public override void ComposeRegexLines(RegexLineCollector collector)
    {
        // If there are no text sgements to render, the OneOf container itself doesn't need spaces between its alternating members
        var neverAddSpacesToGroupMembers = !ChildSegments.Any(x => x is TextSegment);

        collector.OpenGroup(RegexPropInfo, neverAddSpacesToGroupMembers);
        AlternatingComposer.Instance.Compose(collector, ChildSegments);
        collector.CloseGroup();
    }


    public override string ToString() => base.ToString();
}