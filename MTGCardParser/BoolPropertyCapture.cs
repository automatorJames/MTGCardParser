namespace MTGCardParser;

public record BoolPropertyCapture(PropertyInfo Prop) : PropertyCapture(Prop)
{
    public override string RegexPattern { get; } = BuildBoolPattern(Prop);

    private static string BuildBoolPattern(PropertyInfo prop)
    {
        var patterns = prop.GetCustomAttribute<RegexPatternAttribute>()!.Patterns;
        // CORRECTED: Force patterns to lowercase
        var lowerCasePatterns = patterns.Select(p => p.ToLower());
        // This was already correctly optional
        return $"(?<{prop.Name}>{string.Join("|", lowerCasePatterns)})?";
    }

    public override void HydrateProperty(TokenUnit instance, Match match)
    {
        var group = match.Groups[CaptureGroupName];
        if (group.Success)
        {
            var subSpan = GetSubSpanFromGroup(instance.MatchSpan, group)!.Value;
            Prop.SetValue(instance, true);
            instance.PropMatches[this] = subSpan;
        }
    }
}