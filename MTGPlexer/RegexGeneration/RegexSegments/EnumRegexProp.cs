namespace MTGPlexer.RegexGeneration.RegexSegments;

/// <summary>
/// Represents a property on a TokenUnit whose property type is some enum. Enums are special in the sense that the
/// Regex pattern emitted by an enum always comprises all enum members as alternatives, but the property value hydrated
/// by a specific text match must be isolated to a single member value.
/// </summary>
public class EnumRegexProp : ScalarCapturePropBase
{
    public Dictionary<object, Regex> EnumMemberRegexes { get; private set; } = new();

    public EnumRegexProp(RegexPropInfo captureProp) : base(captureProp)
    {
        
    }

    public override void ComposeRegexLines(RegexLineCollector collector)
    {
        collector.OpenGroup(RegexPropInfo);
        collector.AddAlternatiingValues(ScalarAlternativeSet.Alternatives);
        collector.CloseGroup();
    }

    protected override void SetScalarAlternativeSet() => SetAlternativesAndMemberRegexes();

    void SetAlternativesAndMemberRegexes()
    {
        var enumType = RegexPropInfo.UnderlyingType;

        if (TokenTypeRegistry.EnumMemberRegexes.TryGetValue(enumType, out var enumMemberRegexes))
        {
            EnumMemberRegexes = enumMemberRegexes;
            
            // if the registry has the enum's member regexes, it should have its scalar alternatives too
            ScalarAlternativeSet = TokenTypeRegistry.EnumScalarAlternativeSets[enumType];

            return;
        }

        // if not already registered: 
        var enumOptions = RegexPropInfo.UnderlyingType.GetCustomAttribute<RegexEnumAttribute>() ?? new();
        List<string> allMemberAlternatives = new();
        var enumRegOptions = enumType.GetCustomAttribute<RegexEnumAttribute>() ?? new();
        var enumValues = Enum.GetValues(enumType).Cast<object>();

        foreach (var enumValue in enumValues)
        {
            List<string> memberAlternatives = new();
            var enumAsString = enumValue.ToString();
            var regexPatternAttribute = enumType.GetField(enumAsString).GetCustomAttribute<RegexPatternAttribute>();

            if (regexPatternAttribute != null)
                memberAlternatives.AddRange(regexPatternAttribute.Patterns);
            else
                memberAlternatives.Add(enumAsString.ToFriendlyCase());

            if (enumOptions.OptionalPlural)
                for (int i = 0; i < memberAlternatives.Count; i++)
                    memberAlternatives[i] = memberAlternatives[i].AddOptionalPluralization();

            var memberRenderedString = $@"{string.Join('|', memberAlternatives.OrderByDescending(s => s.Length))}";

            EnumMemberRegexes[enumValue] = new Regex("^" + memberRenderedString + "$");
            allMemberAlternatives.AddRange(memberAlternatives);
        }

        var alternatives = allMemberAlternatives.OrderByDescending(s => s.Length).ToList();
        ScalarAlternativeSet = new(alternatives);
    }

    public override bool SetValueFromMatch(TokenUnit token, Match match)
    {
        var group = match.Groups[Name];

        if (!group.Success) 
            return false;

        var capture = match.Groups[Name].Captures.First();
        var valueToSet = GetEnumMatchValue(capture.Value);
        token.SetPropertyFromCapture(RegexPropInfo, capture, valueToSet);
        return true;
    }

    object GetEnumMatchValue(string matchString)
    {
        if (!TokenTypeRegistry.EnumMemberRegexes.ContainsKey(RegexPropInfo.UnderlyingType))
            throw new Exception($"Enum type {RegexPropInfo.UnderlyingType.Name} is not registered in {nameof(TokenTypeRegistry)}");

        foreach (var enumMemberRegex in TokenTypeRegistry.EnumMemberRegexes[RegexPropInfo.UnderlyingType])
            if (enumMemberRegex.Value.IsMatch(matchString))
                return enumMemberRegex.Key;

        return null;
    }
}

