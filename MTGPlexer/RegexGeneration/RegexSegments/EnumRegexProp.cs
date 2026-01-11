namespace MTGPlexer.RegexGeneration.RegexSegments;

/// <summary>
/// Represents a property on a TokenUnit whose property type is some enum. Enums are special in the sense that the
/// Regex pattern emitted by an enum always comprises all enum members as alternatives, but the property value hydrated
/// by a specific text match must be isolated to a single member value.
/// </summary>
public record EnumRegexProp : ScalarCapturePropBase
{
    public override Regex ManyMatchRegex => TokenTypeRegistry.EnumScalarAlternativeSets[RegexPropInfo.BaseType].CollectiveRegex;
    public EnumScalarAlternateSet EnumSet { get; private set; }

    public EnumRegexProp(RegexPropInfo captureProp) : base(captureProp)
    {
    }

    public override void ComposeRegexLines(RegexBuilder builder)
    {
        builder.OpenGroup(RegexPropInfo);

        if (RegexPropInfo.BaseType.GetCustomAttribute<OptionalPrefix>() is OptionalPrefix attr)
            builder.AddTextLine($"({attr.PrefixSnippet} )?");

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
        EnumSet = EnumTypetoScalarSet(enumType);
        ScalarAlternativeSet = EnumSet;
    }

    public override object GetValueToSet(TokenUnit parentTokenUnit, Group namedGroup) => GetEnumMatchValue(namedGroup.Value);

    object GetEnumMatchValue(string matchString)
    {
        foreach (var enumAlternative in EnumSet.EnumAlternates)
            if (enumAlternative.ItemRegex.IsMatch(matchString))
                return enumAlternative.EnumValue;

        throw new Exception($"Found no matching values for enum '{RegexPropInfo.Name}' from match string '{matchString}'");
    }

    public static EnumScalarAlternateSet EnumTypetoScalarSet(Type enumType)
    {
        if (!enumType.IsEnum)
            throw new Exception($"'{enumType.Name}' is not an enum type");

        List<EnumScalarAlternate> enumAlternates = new();

        // get enum values in declared order
        var enumValues = enumType
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .OrderBy(x => x.MetadataToken)
            .Select(x => x.GetValue(null)!)
            .ToList();

        foreach (var enumValue in enumValues)
            enumAlternates.Add(new(enumType, enumValue));

        return new(enumAlternates);
    }
}
