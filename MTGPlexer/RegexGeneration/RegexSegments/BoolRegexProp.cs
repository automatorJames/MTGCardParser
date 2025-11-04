namespace MTGPlexer.RegexGeneration.RegexSegments;

/// <summary>
/// Represents a bool property on a TokenUnit. Bool property Regexes typically check for the optional presence
/// of some matching pattern. Such properties are usually expected to have a RegexPattern attribute that defines
/// its pattern(s), but in the absence of this the normalized property name is matched.
/// </summary>
public record BoolRegexProp : ScalarCapturePropBase
{
    public BoolRegexProp(RegexPropInfo captureProp) : base(captureProp)
    {
    }

    public override void ComposeRegexLines(RegexBuilder builder)
    {
        builder.OpenGroup(RegexPropInfo);
        builder.AddAlternateValues(ScalarAlternativeSet.Alternates);
        builder.CloseGroup(GroupQuantifier.Optional);
    }

    public override bool SetValueFromMatch(TokenUnit token, Match match, int captureIndex, string distinguishingAppendix = null)
    {
        var group = match.Groups[Name + distinguishingAppendix];

        if (!group.Success || group.Captures.Count - 1 < captureIndex)
            return false;

        var capture = group.Captures[captureIndex];
        token.SetPropertyFromCapture(RegexPropInfo, capture, true);

        return true;
    }
}