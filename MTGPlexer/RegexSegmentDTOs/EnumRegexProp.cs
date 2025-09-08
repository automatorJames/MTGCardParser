namespace MTGPlexer.RegexSegmentDTOs;

/// <summary>
/// Represents a property on a TokenUnit whose property type is some enum. Enums are special in the sense that the
/// Regex pattern emitted by an enum always comprises all enum members as alternatives, but the property value hydrated
/// by a specific text match must be isolated to a single member value.
/// </summary>
public class EnumRegexProp : CaptureGroupPropBase
{
    public Dictionary<object, Regex> EnumMemberRegexes { get; private set; } = new();
    public RegexEnumAttribute Options { get; private set; }

    public EnumRegexProp(RegexPropInfo captureProp) : base(captureProp)
    {
        Options = captureProp.UnderlyingType.GetCustomAttribute<RegexEnumAttribute>() ?? new();

        if (captureProp.RegexPropType != RegexPropType.Enum)
            throw new ArgumentException($"Type '{captureProp.Name}' isn't an enum");
    }

    //protected override void SetRegex(RegexPropInfo regexPropInfo)
    //{
    //    Options = regexPropInfo.UnderlyingType.GetCustomAttribute<RegexEnumAttribute>() ?? new();
    //    CaptureAlternatives = GetAlternations();
    //    CaptureAlternativesString = string.Join("|", CaptureAlternatives);
    //    RegexString = $@"(?<{regexPropInfo.Name}>{CaptureAlternativesString})";
    //}

    public override void ComposeRegexLines(RegexLineCollector collector)
    {
        collector.OpenGroup(RegexPropInfo);
        var alternations = GetAlternations();
        collector.AddAlternateValues(alternations);
        collector.CloseGroup();
    }

    List<string> GetAlternations()
    {
        List<string> allMemberAlternatives = new();
        var enumRegOptions = RegexPropInfo.UnderlyingType.GetCustomAttribute<RegexEnumAttribute>() ?? new();
        var enumValues = Enum.GetValues(RegexPropInfo.UnderlyingType).Cast<object>();

        foreach (var enumValue in enumValues)
        {
            List<string> memberAlternatives = new();
            var enumAsString = enumValue.ToString();
            var regexPatternAttribute = RegexPropInfo.UnderlyingType.GetField(enumAsString).GetCustomAttribute<RegexPatternAttribute>();

            if (regexPatternAttribute != null)
                memberAlternatives.AddRange(regexPatternAttribute.Patterns);
            else
                memberAlternatives.Add(enumAsString.ToFriendlyCase());

            if (Options.OptionalPlural)
                for (int i = 0; i < memberAlternatives.Count; i++)
                    memberAlternatives[i] = memberAlternatives[i].AddOptionalPluralization();

            var memberRenderedString = $@"{string.Join('|', memberAlternatives.OrderByDescending(s => s.Length))}";

            EnumMemberRegexes[enumValue] = new Regex("^" + memberRenderedString + "$");
            allMemberAlternatives.AddRange(memberAlternatives);
        }

        return allMemberAlternatives.OrderByDescending(s => s.Length).ToList();
    }
}

