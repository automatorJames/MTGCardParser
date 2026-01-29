
namespace MTGPlexer.RegexGeneration.RegexSegments;

/// <summary>
/// Represents a property on a TokenUnit whose property type is some enum. Enums are special in the sense that the
/// Regex pattern emitted by an enum always comprises all enum members as alternatives, but the property value hydrated
/// by a specific text match must be isolated to a single member value.
/// </summary>
public record EnumNode : TerminalNode
{
    bool _isOptional;
    //public EnumScalarAlternateSet EnumSet { get; private set; }

    public EnumNode(Node parentNode, PropertySnippet propertySnippet) : base(parentNode, propertySnippet)
    {
        // If the enum is nullable, we treat it as optional. The exception to this is if the
        // enum is contained in a TokenUnitOneOf, where at least one alternative must be matched,
        // and the "zero or one" optional quantifier would allow zero-width false matches.
        _isOptional =
            Nullable.GetUnderlyingType(propertySnippet.Prop.PropertyType) != null
            && !propertySnippet.Prop.DeclaringType.IsAssignableTo(typeof(TokenUnitOneOf));
    }

    public override void ComposeRegexLines(RegexBuilder builder)
    {
        builder.OpenGroup(new TemplatePropInfo(PropertySnippet.Prop), isOptional: _isOptional);

        if (UnderlyingType.GetCustomAttribute<OptionalPrefix>() is OptionalPrefix attr)
            builder.AddTextLine($"({attr.PrefixSnippet} )?");

        builder.AddAlternateEnumValues((EnumScalarAlternateSet)ScalarAlternateSet);
        builder.CloseGroup(_isOptional ? GroupQuantifier.Optional : null);
    }

    protected override ScalarAlternateSet GetScalarAlternateSet()
    {
        List<EnumScalarAlternate> enumAlternates = new();

        // get enum values in declared order
        var enumValues = UnderlyingType
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .OrderBy(x => x.MetadataToken)
            .Select(x => x.GetValue(null)!)
            .ToList();

        foreach (var enumValue in enumValues)
            enumAlternates.Add(new(UnderlyingType, enumValue));

        return new EnumScalarAlternateSet(enumAlternates);
    }

    protected override object GetPropertyValue(Capture capture)
    {
        //foreach (var enumAlternative in EnumSet.EnumAlternates)
        //    if (enumAlternative.ItemRegex.IsMatch(scopedCapture.Value))
        //    {
        //        result = ValueResult.Success;
        //        return enumAlternative.EnumValue;
        //    }
        //
        //throw new Exception($"Found no matching values for enum '{TemplatePropInfo.Name}' from match string '{scopedCapture.Value}'");

        throw new NotImplementedException();
    }

    //public static EnumScalarAlternateSet EnumTypetoScalarSet(Type enumType)
    //{
    //    if (!enumType.IsEnum)
    //        throw new Exception($"'{enumType.Name}' is not an enum type");
    //
    //    List<EnumScalarAlternate> enumAlternates = new();
    //
    //    // get enum values in declared order
    //    var enumValues = enumType
    //        .GetFields(BindingFlags.Public | BindingFlags.Static)
    //        .OrderBy(x => x.MetadataToken)
    //        .Select(x => x.GetValue(null)!)
    //        .ToList();
    //
    //    foreach (var enumValue in enumValues)
    //        enumAlternates.Add(new(enumType, enumValue));
    //
    //    return new(enumAlternates);
    //}
}
