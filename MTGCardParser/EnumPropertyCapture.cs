namespace MTGCardParser;

public record EnumPropertyCapture(PropertyInfo Prop) : PropertyCapture(Prop)
{
    public override string RegexPattern { get; } = BuildEnumPattern(Prop);

    private static string BuildEnumPattern(PropertyInfo prop)
    {
        var enumType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
        var memberPatterns = Enum.GetNames(enumType)
            .Select(name =>
            {
                var memberInfo = enumType.GetMember(name).First();
                var attribute = memberInfo.GetCustomAttribute<RegexPatternAttribute>();
                return attribute?.Patterns.First() ?? name;
            });

        // CORRECTED: Force patterns to lowercase as per original system's behavior
        var lowerCasePatterns = memberPatterns.Select(p => p.ToLower());

        // CORRECTED: Make the entire group optional
        return $"(?<{prop.Name}>({string.Join("|", lowerCasePatterns)}))?";
    }

    public override void HydrateProperty(TokenUnit instance, Match match)
    {
        var group = match.Groups[CaptureGroupName];
        if (!group.Success) return;

        var subSpan = GetSubSpanFromGroup(instance.MatchSpan, group)!.Value;
        var matchText = subSpan.ToStringValue(); // Already lowercased by the tokenizer

        var enumType = Nullable.GetUnderlyingType(Prop.PropertyType) ?? Prop.PropertyType;
        foreach (var memberName in Enum.GetNames(enumType))
        {
            var memberInfo = enumType.GetMember(memberName).First();
            var pattern = (memberInfo.GetCustomAttribute<RegexPatternAttribute>()?.Patterns.First() ?? memberName).ToLower();
            if (matchText == pattern) // Simple string comparison is now sufficient
            {
                var enumValue = Enum.Parse(enumType, memberName);
                Prop.SetValue(instance, enumValue);
                instance.PropMatches[this] = subSpan;
                return;
            }
        }
    }
}