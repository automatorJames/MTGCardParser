namespace MTGPlexer.RegexGeneration.RegexSegments;

/// <summary>
/// Represents a property on a TokenUnit whose property type is some enum. Enums are special in the sense that the
/// Regex pattern emitted by an enum always comprises all enum members as alternatives, but the property value hydrated
/// by a specific text match must be isolated to a single member value.
/// </summary>
public class EnumRegexProp : ScalarCapturePropBase
{
    public override Regex MatchRegex => TokenTypeRegistry.EnumScalarAlternativeSets[RegexPropInfo.BaseType].CollectiveRegex;
    public EnumScalarAlternativeSet EnumSet { get; private set; }

    public EnumRegexProp(RegexPropInfo captureProp) : base(captureProp)
    {
    }

    public override void ComposeRegexLines(RegexLineCollector collector)
    {
        collector.OpenGroup(RegexPropInfo, nameOverride: Name);
        collector.AddAlternatingEnumValues(EnumSet);
        collector.CloseGroup();
    }

    protected override void SetScalarAlternativeSet(RegexPropInfo captureProp)
    {
        var enumType = captureProp.BaseType;

        if (TokenTypeRegistry.EnumScalarAlternativeSets.TryGetValue(enumType, out var enumSet))
        {
            EnumSet = enumSet;
            ScalarAlternativeSet = EnumSet;
            return;
        }

        // if not already registered: 
        var enumOptions = enumType.GetCustomAttribute<RegexEnumAttribute>() ?? new();
        List<EnumScalarAlternative> enumAlternatives = new();

        // get enum values in declared order
        var enumValues = enumType
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .OrderBy(x => x.MetadataToken)
            .Select(x => x.GetValue(null)!)
            .ToList();

        for (int i = 0; i < enumValues.Count; i++)
        {
            var enumValue = enumValues[i];
            string memberAlternativeStringOrSynonymSet = null;
            var enumAsString = enumValue.ToString();
            var regexPatternAttribute = enumType.GetField(enumAsString).GetCustomAttribute<RegexPatternAttribute>();

            if (regexPatternAttribute != null)
            {
                var spaceEscapedPatterns = regexPatternAttribute.Patterns.Select(x => x.Replace(" ", "[ ]")).ToList();

                if (spaceEscapedPatterns.Count == 1)
                    memberAlternativeStringOrSynonymSet = spaceEscapedPatterns[0];
                else
                    memberAlternativeStringOrSynonymSet = string.Join(" | ", spaceEscapedPatterns);
            }
            else
                memberAlternativeStringOrSynonymSet = enumAsString.ToFriendlyCase().Replace(" ", "[ ]");

            if (enumOptions.OptionalPlural)
                memberAlternativeStringOrSynonymSet = memberAlternativeStringOrSynonymSet.AddOptionalPluralization();

            enumAlternatives.Add(new(enumType, enumValue, memberAlternativeStringOrSynonymSet, i));
        }

        EnumSet = new(enumAlternatives);
        ScalarAlternativeSet = EnumSet;
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
        foreach (var enumAlternative in EnumSet.EnumAlternatives)
            if (enumAlternative.ItemRegex.IsMatch(matchString))
                return enumAlternative.EnumValue;

        throw new Exception($"Found no matching values for enum '{RegexPropInfo.Name}' from match string '{matchString}'");
    }
}
