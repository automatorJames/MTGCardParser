namespace MTGPlexer.RegexGeneration.GraphNodes;

/// <summary>
/// Represents a property on a TokenUnit whose property type is some enum. Enums are special in the sense that the
/// Regex pattern emitted by an enum always comprises all enum members as alternatives, but the property value hydrated
/// by a specific text match must be isolated to a single member value.
/// </summary>
public class EnumNode : TerminalNode
{
    bool _isOptional;

    public EnumNode(Node parentNode, INavigable navigable) : base(parentNode, navigable)
    {
        // If the enum is nullable, we treat it as optional. The exception to this is if the
        // enum is contained in a TokenUnitOneOf, where at least one alternative must be matched,
        // and the "zero or one" optional quantifier would allow zero-width false matches.
        _isOptional =
            Nullable.GetUnderlyingType(Navigable.Type) != null
            && ConcreteProperty is PropertyInfo prop
            && !prop.DeclaringType.IsAssignableTo(typeof(TokenUnitOneOf));

        _isOptional |= IsOptional;
    }

    public override void ComposeRegexLines(RegexBuilder builder)
    {
        builder.OpenNamedGroup(this, isOptional: _isOptional);

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

    public override object GetValue(Capture capture)
    {
        var enumSet = (EnumScalarAlternateSet)ScalarAlternateSet;

        foreach (var enumAlternative in enumSet.EnumAlternates)
            if (enumAlternative.ItemRegex.IsMatch(capture.Value))
                return enumAlternative.EnumValue;

        throw new Exception($"Found no matching values for enum '{Navigable.Type.Name}' from match string '{capture.Value}'");
    }
}
