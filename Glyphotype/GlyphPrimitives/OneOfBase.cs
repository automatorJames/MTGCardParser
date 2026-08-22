namespace Glyphotype.GlyphPrimitives;

public abstract class OneOfBase : Glyph
{
    public override Joiner Joiner => Joiner.Pipe;

    /// <summary>
    /// The CLR type of whichever alternative actually resolved on this instance (e.g. <c>CardType</c>
    /// for a <see cref="OneOf{T1,T2}"/> whose first slot matched) — never the nullable wrapper, and
    /// never this instance's own (often generic-mangled, e.g. "OneOf`2") type name. This isn't a
    /// proper data-path citizen; it's a display-time fact about which choice a one-of resolved to,
    /// surfaced by callers like the property table alongside the path step itself.
    /// Found by scanning this exact instance's declared properties for the single one holding a
    /// non-null value — the one alternative that matched — which is the same lookup whether the
    /// slots are named Alternative1/2/3 (<see cref="OneOf{T1,T2}"/>) or given proper names by a
    /// concrete <see cref="GlyphOneOf"/>.
    /// </summary>
    public Type GetResolvedType()
    {
        var winningProp = GetType().GetProps().FirstOrDefault(p => p.GetValue(this) != null);
        return winningProp == null ? null : Nullable.GetUnderlyingType(winningProp.PropertyType) ?? winningProp.PropertyType;
    }

    public override string ValidateStructure()
    {
        var graph = GlyphTypeRegistry.RegexGraphs[Type];
        var props = GetType().GetOwnProps();

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