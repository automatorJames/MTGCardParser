namespace MTGPlexer.RegexGeneration.RegexSegments;

/// <summary>
/// Represents a bool property on a TokenUnit. Bool property Regexes typically check for the optional presence
/// of some matching pattern. Such properties are usually expected to have a RegexPattern attribute that defines
/// its pattern(s), but in the absence of this the normalized property name is matched.
/// </summary>
public class BoolRegexProp : CaptureGroupPropBase
{
    public BoolRegexProp(RegexPropInfo captureProp) : base(captureProp)
    {
    }

    public override void ComposeRegexLines(RegexLineCollector collector)
    {
        collector.OpenGroup(RegexPropInfo);

        var captureAlternatives = (RegexPropInfo.Prop.GetCustomAttribute<RegexPatternAttribute>()?.Patterns ?? [RegexPropInfo.Name])
            .OrderByDescending(s => s.Length).ToList();

        collector.AddAlternateValues(captureAlternatives);
        collector.CloseGroup(GroupQuantifier.Optional);
    }

    public override bool SetValueFromMatchSpan(TokenUnit parentToken, TextSpan matchSpan)
    {
        var subMatchSpan = GetGroupSubMatch(parentToken, matchSpan);
        var valueToSet = subMatchSpan != null;
        TextSpan span = subMatchSpan ?? new TextSpan("");
        parentToken.SetPropertyCapture(RegexPropInfo, span, valueToSet);
        return true;
    }
}