namespace MTGPlexer.RegexGeneration.GraphNodes;

/// <summary>
/// Represents a property on a TokenUnit whose property type is some enum. Enums are special in the sense that the
/// Regex pattern emitted by an enum always comprises all enum members as alternatives, but the property value hydrated
/// by a specific text match must be isolated to a single member value.
/// </summary>
public class EnumNode : TerminalNode
{
    public EnumNode(Node parentNode, INavigable navigable) : base(parentNode, navigable)
    {
    }

    public override void ComposeRegexLines(RegexBuilder builder)
    {
        builder.OpenNamedGroup(this);

        if (UnderlyingType.GetCustomAttribute<OptionalPrefix>() is OptionalPrefix attr)
            builder.AddTextLine($"({attr.PrefixSnippet} )?");

        builder.AddAlternateEnumValues((EnumScalarAlternateSet)ScalarAlternateSet);
        builder.CloseGroup(IsOptional ? GroupQuantifier.Optional : null);
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
