namespace MTGCardParser;

public record TextSegmentPropertyCapture(PropertyInfo Prop) : PropertyCapture(Prop)
{
    public override string RegexPattern { get; } = BuildTextSegmentPattern(Prop);

    private static string BuildTextSegmentPattern(PropertyInfo prop)
    {
        var pattern = prop.GetCustomAttribute<RegexPatternAttribute>()!.Patterns.First();
        // CORRECTED: Make the group optional
        return $"(?<{prop.Name}>{pattern})?";
    }

    public override void HydrateProperty(TokenUnit instance, Match match)
    {
        var group = match.Groups[CaptureGroupName];
        if (!group.Success) return;

        var subSpan = GetSubSpanFromGroup(instance.MatchSpan, group)!.Value;
        var textSegment = new CapturedTextSegment(subSpan.ToStringValue());
        Prop.SetValue(instance, textSegment);
        instance.PropMatches[this] = subSpan;
    }
}