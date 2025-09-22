namespace MTGPlexer.RegexGeneration.RegexSegments;

/// <summary>
/// Represents a property on a TokenUnit whose property type is some enum. Enums are special in the sense that the
/// Regex pattern emitted by an enum always comprises all enum members as alternatives, but the property value hydrated
/// by a specific text match must be isolated to a single member value.
/// </summary>
public class EnumRegexProp : ScalarCapturePropBase
{
    public override Regex MatchRegex => TokenTypeRegistry.EnumScalarAlternativeSets[RegexPropInfo.BaseType].Regex;
    public Dictionary<object, Regex> EnumMemberRegexes { get; private set; } = new();

    public EnumRegexProp(RegexPropInfo captureProp) : base(captureProp)
    {
    }

    public override void ComposeRegexLines(RegexLineCollector collector)
    {
        collector.OpenGroup(RegexPropInfo, nameOverride: Name);
        collector.AddAlternatingValues(ScalarAlternativeSet.Alternatives);
        collector.CloseGroup();
    }

    protected override void SetScalarAlternativeSet(RegexPropInfo captureProp)
    {
        var enumType = captureProp.BaseType;

        if (TokenTypeRegistry.EnumMemberRegexes.TryGetValue(enumType, out var enumMemberRegexes))
        {
            EnumMemberRegexes = enumMemberRegexes;
            
            // if the registry has the enum's member regexes, it should have its scalar alternatives too
            ScalarAlternativeSet = TokenTypeRegistry.EnumScalarAlternativeSets[enumType];

            return;
        }

        // if not already registered: 
        var enumOptions = captureProp.BaseType.GetCustomAttribute<RegexEnumAttribute>() ?? new();
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
        var capture = match.Groups[Name];

        if (!capture.Success)
        {
            if (!RegexPropInfo.MayBeNull)
                throw new Exception($"{RegexPropInfo.Name} is not a nullable enum, but no match was found");

            return false;
        }

        var valueToSet = GetEnumMatchValue(capture.Value);
        token.SetPropertyFromCapture(RegexPropInfo, capture, valueToSet);
        return true;
    }

    object GetEnumMatchValue(string matchString)
    {
        foreach (var enumMemberRegex in TokenTypeRegistry.EnumMemberRegexes[RegexPropInfo.UnderlyingType])
            if (enumMemberRegex.Value.IsMatch(matchString))
                return enumMemberRegex.Key;

        throw new Exception($"Found no matching values for enum '{RegexPropInfo.Name}' from match string '{matchString}'");
    }
}

