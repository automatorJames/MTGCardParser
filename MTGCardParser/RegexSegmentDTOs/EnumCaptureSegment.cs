namespace MTGCardParser.RegexSegmentDTOs;

public record EnumCaptureSegment : PropSegmentBase
{
    public Dictionary<object, Regex> EnumMemberRegexes { get; private set; } = new();
    public EnumOptionsAttribute Options { get; init; }


    public EnumCaptureSegment(CaptureProp captureProp) : base(captureProp)
    {
        if (CaptureProp.CapturePropType != CapturePropType.Enum)
            throw new ArgumentException($"Type '{CaptureProp.Name}' isn't an enum");

        Options = CaptureProp.UnderlyingType.GetCustomAttribute<EnumOptionsAttribute>() ?? new();
        SetRegex();
    }

    void SetRegex()
    {
        var alternations = GetAlternations();
        RegexString = $@"(?<{CaptureProp.Name}>{alternations})";

        if (Options.WrapInWordBoundaries)
            RegexString = $@"\b{RegexString}\b";

        Regex = new Regex(RegexString);
    }

    string GetAlternations()
    {
        List<string> allMemberAlternatives = new();
        var enumRegOptions = CaptureProp.UnderlyingType.GetCustomAttribute<EnumOptionsAttribute>() ?? new();
        var enumValues = Enum.GetValues(CaptureProp.UnderlyingType).Cast<object>();

        foreach (var enumValue in enumValues)
        {
            List<string> memberAlternatives = new();

            var enumAsString = enumValue.ToString();
            var regexPatternAttribute = CaptureProp.UnderlyingType.GetField(enumAsString).GetCustomAttribute<RegexPatternAttribute>();

            if (regexPatternAttribute != null)
                // Add the enum member's specified string[] of patterns
                memberAlternatives.AddRange(regexPatternAttribute.Patterns);
            else
                // Otherwise add the stringified enum iteself
                memberAlternatives.Add(enumAsString.ToLower());

            if (Options.OptionalPlural)
                for (int i = 0; i < memberAlternatives.Count; i++)
                    memberAlternatives[i] = memberAlternatives[i].AddOptionalPluralization();

            EnumMemberRegexes[enumValue] = new Regex($@"\b{string.Join('|', memberAlternatives.OrderByDescending(s => s.Length))}\b");
            allMemberAlternatives.AddRange(memberAlternatives);
        }

        return string.Join("|", allMemberAlternatives.OrderByDescending(s => s.Length));
    }
}

