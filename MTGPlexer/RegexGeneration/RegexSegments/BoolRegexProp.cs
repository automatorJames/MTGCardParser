namespace MTGPlexer.RegexGeneration.RegexSegments;

/// <summary>
/// Represents a bool property on a TokenUnit. Bool property Regexes typically check for the optional presence
/// of some matching pattern. Such properties are usually expected to have a RegexPattern attribute that defines
/// its pattern(s), but in the absence of this the normalized property name is matched.
/// </summary>
public class BoolRegexProp : ScalarCapturePropBase
{
    public BoolRegexProp(RegexPropInfo captureProp) : base(captureProp)
    {
    }

    public override void ComposeRegexLines(RegexLineCollector collector)
    {
        collector.OpenGroup(RegexPropInfo);
        collector.AddAlternatiingValues(ScalarAlternativeSet.Alternatives);
        collector.CloseGroup(GroupQuantifier.Optional);
    }

    //public override bool SetValueFromMatchSpan(TokenUnit parentToken, TextSpan matchSpan)
    //{
    //    var subMatchSpan = GetGroupSubMatch(parentToken, matchSpan);
    //    var valueToSet = subMatchSpan != null;
    //    TextSpan span = subMatchSpan ?? new TextSpan("");
    //    parentToken.SetPropertyCapture(RegexPropInfo, span, valueToSet);
    //    return true;
    //}

    public override bool SetValueFromMatch(TokenUnit token, Match match)
    {
        var capture = match.Groups[Name].Captures.FirstOrDefault();
        var valueToSet = match.Groups[Name].Success;
        token.SetPropertyFromCapture(RegexPropInfo, capture, valueToSet);
        return true;
    }
}