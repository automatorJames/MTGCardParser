namespace Glyphotype.GlyphPrimitives;

public abstract class OneOfBase : Glyph
{
    public override Joiner Joiner => Joiner.Pipe;

    public override string ValidateStructure()
    {
        var graph = GlyphTypeRegistry.RegexGraphs[Type];
        var props = GetType().GetProps();

        if (props.Count() < 2)
            return $"Nibs for {Type.Name} must contain at least two property references";

        if (!graph.RootNode.ValidateCapturePropertiesAreContiguous())
            return $"Nibs for {Type.Name} contains more than one contiguous run of property references interspersed by text";

        // Any enums must be nullable
        var nonNullableEnumTypeNames = graph.RootNode.Children.OfType<EnumNode>()
            .Where(x => Nullable.GetUnderlyingType(x.Navigation.Type) == null)
            .Select(x => x.Navigation.Type.Name)
            .ToList();

        if (nonNullableEnumTypeNames.Count > 0)
            return $"All enum properties in {nameof(OneOfBase)} must be nullable, but {DescribeUsageSite(Type)} contains " +
                $"{nonNullableEnumTypeNames.Count} non-nullable type{(nonNullableEnumTypeNames.Count == 1 ? "" : "s")}: {string.Join(", ", nonNullableEnumTypeNames)}";

        return base.ValidateStructure();
    }

    /// <summary>
    /// Finds the first "OwnerType.PropertyName" in the assembly whose property is declared as this exact
    /// type, so a closed generic like <c>OneOf&lt;CardType, CreatureType&gt;</c> - whose own <see cref="Type.Name"/>
    /// is the unhelpful "OneOf`2" - can be reported to the user via wherever it's actually referenced.
    /// Falls back to the type's own name if it isn't used as a property anywhere (e.g. a standalone
    /// concrete OneOf subclass).
    /// </summary>
    static string DescribeUsageSite(Type oneOfType)
    {
        var usage = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => typeof(Glyph).IsAssignableFrom(t) && !t.IsAbstract)
            .Select(t => (owner: t, prop: t.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .FirstOrDefault(p => p.PropertyType == oneOfType)))
            .FirstOrDefault(x => x.prop != null);

        return usage.prop != null ? $"{usage.owner.Name}.{usage.prop.Name}" : oneOfType.Name;
    }
}