namespace MTGPlexer.RegexGeneration.RegexSegments;

/// <summary>
/// Represents a property on a TokenUnit whose property type is some enum. Enums are special in the sense that the
/// Regex pattern emitted by an enum always comprises all enum members as alternatives, but the property value hydrated
/// by a specific text match must be isolated to a single member value.
/// </summary>
public class EnumRegexProp : ScalarCapturePropBase
{
    public override Regex MatchRegex => TokenTypeRegistry.EnumScalarAlternativeSets[RegexPropInfo.BaseType].CollectiveRegex;
    public EnumScalarAlternateSet EnumSet { get; private set; }

    public EnumRegexProp(RegexPropInfo captureProp) : base(captureProp)
    {
    }

    public override void ComposeRegexLines(RegexBuilder builder)
    {
        builder.OpenGroup(RegexPropInfo, nameOverride: Name);
        builder.AddAlternateEnumValues(EnumSet);
        builder.CloseGroup();
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
        List<EnumScalarAlternate> enumAlternatives = new();

        // get enum values in declared order
        var enumValues = enumType
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .OrderBy(x => x.MetadataToken)
            .Select(x => x.GetValue(null)!)
            .ToList();

        for (int i = 0; i < enumValues.Count; i++)
        {
            var enumValue = enumValues[i];
            List<string> synonyms = [];
            var enumAsString = enumValue.ToString();
            var regexPatternAttribute = enumType.GetField(enumAsString).GetCustomAttribute<RegexPatternAttribute>();

            if (regexPatternAttribute != null)
                synonyms.AddRange(regexPatternAttribute.Patterns.Select(x => x.Replace(" ", "[ ]")));
            else
                synonyms.Add(enumAsString.ToFriendlyCase().Replace(" ", "[ ]"));

            enumAlternatives.Add(new(enumType, enumValue, synonyms, i));
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
        foreach (var enumAlternative in EnumSet.EnumAlternates)
            if (enumAlternative.ItemRegex.IsMatch(matchString))
                return enumAlternative.EnumValue;

        throw new Exception($"Found no matching values for enum '{RegexPropInfo.Name}' from match string '{matchString}'");
    }
}
