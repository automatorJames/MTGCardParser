
namespace MTGCardParser;

public record EnumCaptureGroup : IRegexSegment
{
    public string Name { get; init; }
    public string RegexString { get; private set; }
    public Regex Regex { get; private set; }
    public Type EnumType { get; init; }
    public RegexOptionsAttribute Options { get; init; }
    public Dictionary<object, Regex> EnumMemberRegexes { get; private set; } = new();


    public EnumCaptureGroup(Type enumType)
    {
        var underlyingEnumType = Nullable.GetUnderlyingType(enumType) ?? enumType;

        if (!underlyingEnumType.IsEnum)
            throw new ArgumentException($"{underlyingEnumType} isn't an enum type");

        Name = enumType.Name;
        EnumType = underlyingEnumType;
        Options = underlyingEnumType.GetCustomAttribute<RegexOptionsAttribute>() ?? new();
        SetRegex();
    }

    void SetRegex()
    {
        var alternations = GetAlternations(EnumType);
        RegexString = $@"(?<{Name}>{alternations})";

        if (Options.WrapInWordBoundaries)
            RegexString = $@"\b{RegexString}\b";

        Regex = new Regex(RegexString);
    }

    string GetAlternations(Type underlyingEnumType)
    {
        List<string> allMemberAlternatives = new();
        var enumRegOptions = underlyingEnumType.GetCustomAttribute<RegexOptionsAttribute>() ?? new();
        var enumValues = Enum.GetValues(underlyingEnumType).Cast<object>();

        foreach (var enumValue in enumValues)
        {
            List<string> memberAlternatives = new();

            var enumAsString = enumValue.ToString();
            var regexPatternAttribute = underlyingEnumType.GetField(enumAsString).GetCustomAttribute<RegexPatternAttribute>();

            if (regexPatternAttribute != null)
                // Add the enum member's specified string[] of patterns
                memberAlternatives.AddRange(regexPatternAttribute.Patterns);
            else
                // Otherwise add the stringified enum iteself
                memberAlternatives.Add(enumAsString.ToLower());

            if (Options.OptionalPlural)
                for (int i = 0; i < memberAlternatives.Count; i++)
                    memberAlternatives[i] = IRegexSegment.AddOptionalPluralization(memberAlternatives[i]);

            EnumMemberRegexes[enumValue] = new Regex($@"\b{string.Join('|', memberAlternatives.OrderByDescending(s => s.Length))}\b");
            allMemberAlternatives.AddRange(memberAlternatives);
        }

        return string.Join("|", allMemberAlternatives.OrderByDescending(s => s.Length));
    }
}

