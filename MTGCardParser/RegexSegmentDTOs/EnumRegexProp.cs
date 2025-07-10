namespace MTGCardParser.RegexSegmentDTOs;

/// <summary>
/// Represents a property on a TokenUnit whose property type is some enum. Enums are special in the sense that the
/// Regex pattern emitted by an enum always comprises all enum members as alternatives, but the property value hydrated
/// by a specific text match must be isolated to a single member value.
/// </summary>
public record EnumRegexProp : RegexPropBase
{
    public Dictionary<object, Regex> EnumMemberRegexes { get; private set; } = new();
    public EnumOptionsAttribute Options { get; init; }


    public EnumRegexProp(RegexPropInfo captureProp) : base(captureProp)
    {
        if (RegexPropInfo.RegexPropType != RegexPropType.Enum)
            throw new ArgumentException($"Type '{RegexPropInfo.Name}' isn't an enum");

        Options = RegexPropInfo.UnderlyingType.GetCustomAttribute<EnumOptionsAttribute>() ?? new();
        SetRegex();
    }

    void SetRegex()
    {
        var alternations = GetAlternations();
        RegexString = $@"(?<{RegexPropInfo.Name}>{alternations})";

        if (Options.WrapInWordBoundaries)
            RegexString = $@"\b{RegexString}\b";

        Regex = new Regex(RegexString);
    }

    string GetAlternations()
    {
        List<string> allMemberAlternatives = new();
        var enumRegOptions = RegexPropInfo.UnderlyingType.GetCustomAttribute<EnumOptionsAttribute>() ?? new();
        var enumValues = Enum.GetValues(RegexPropInfo.UnderlyingType).Cast<object>();

        foreach (var enumValue in enumValues)
        {
            List<string> memberAlternatives = new();

            var enumAsString = enumValue.ToString();
            var regexPatternAttribute = RegexPropInfo.UnderlyingType.GetField(enumAsString).GetCustomAttribute<RegexPatternAttribute>();

            if (regexPatternAttribute != null)
                // Add the enum member's specified string[] of patterns
                memberAlternatives.AddRange(regexPatternAttribute.Patterns);
            else
                // Otherwise add the normalized enum iteself
                memberAlternatives.Add(SplitOnCapitalsToLower(enumAsString));

            if (Options.OptionalPlural)
                for (int i = 0; i < memberAlternatives.Count; i++)
                    memberAlternatives[i] = memberAlternatives[i].AddOptionalPluralization();

            EnumMemberRegexes[enumValue] = new Regex($@"\b{string.Join('|', memberAlternatives.OrderByDescending(s => s.Length))}\b");
            allMemberAlternatives.AddRange(memberAlternatives);
        }

        return string.Join("|", allMemberAlternatives.OrderByDescending(s => s.Length));
    }

    string SplitOnCapitalsToLower(string input)
    {
        if (input == null)
            return string.Empty;

        var result = Regex.Replace(input, "(?<!^)([A-Z])", " $1");
        return result.ToLower();
    }
}

