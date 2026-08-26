using Glyphotype.GlyphPrimitives.Internal;

namespace Glyphotype.NibHelpers;

public record PropertyNib : Nib
{
    public PropertyInfo Prop { get; }
    public Type Type { get; }
    public string Name { get; }
    public Proptions Proptions { get; }
    public Quantifier? Quantifier { get; }

    /// <summary>A better name than <see cref="Name"/> for "XOf" wrapper properties (FirstItem, Item, ...) - based on the wrapped type T instead. Null outside that hierarchy.</summary>
    public string DescriptiveName { get; }

    /// <summary>This property's own <see cref="Navigation"/>, cached so every graph position sharing this <see cref="PropertyNib"/> reuses the same instance. Must be built last - its constructor snapshots <see cref="Proptions"/>.</summary>
    public Navigation Navigation { get; }

    public PropertyNib(string text, PropertyInfo prop, Proptions proptions, Quantifier? quantifier = null)
        : base(text)
    {
        Prop = prop;
        Proptions = proptions;
        Type = prop.PropertyType;
        Name = prop.Name;
        Quantifier = quantifier;
        DescriptiveName = ComputeDescriptiveName();

        // Extract metadata info from property attributes
        // Todo: we should be using Quantifier to express quantifiers, not Proptions

        if (Prop.IsDefined(typeof(OneOrMoreAttribute)))
            Proptions |= Proptions.OneOrMore;

        if (Prop.IsDefined(typeof(OptionalAttribute)))
            Proptions |= Proptions.Optional;

        Navigation = new Navigation(this, DescriptiveName);
    }

    string ComputeDescriptiveName()
    {
        var declaringType = Prop.DeclaringType;
        var safeTypeName = Navigation.GetRegexSafeTypeName(Nullable.GetUnderlyingType(Type) ?? Type);

        if (typeof(OneOfBase).IsAssignableFrom(declaringType) || IsClosedGeneric(declaringType, typeof(GlyphFused<>)))
            return safeTypeName;

        if (IsClosedGeneric(declaringType, typeof(CompoundOf<>)) && Name == nameof(CompoundOf<object>.FirstItem))
            return $"{safeTypeName}Primary";

        if (IsClosedGeneric(declaringType, typeof(CompoundOfSecondItem<>)) && Name == nameof(CompoundOfSecondItem<object>.Item))
            return $"{safeTypeName}Secondary";

        if (IsClosedGeneric(declaringType, typeof(ManyOf<>)) && Name == nameof(ManyOf<object>.FirstItem))
            return $"{safeTypeName}First";

        if (IsClosedGeneric(declaringType, typeof(ManyOf<>)) && Name == nameof(ManyOf<object>.LastItem))
            return $"{safeTypeName}Last";

        if (IsClosedGeneric(declaringType, typeof(ManyOfSecondItem<>)) && Name == nameof(ManyOfSecondItem<object>.Item))
            return $"{safeTypeName}Middle";

        return null;
    }

    static bool IsClosedGeneric(Type type, Type openGeneric) =>
        type.IsGenericType && type.GetGenericTypeDefinition() == openGeneric;

    public static PropertyNib[] GetPropertyNibs(Type type) =>
        type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(x => x.GetSetMethod() != null) // Ignore get-only props like Joiner overrides
            .Where(x => IsRelevantPropertyType(x.PropertyType))
            .Select(x => new PropertyNib(x.Name, x, Proptions.None))
            .ToArray();

    /// <summary>
    /// Whether a property belongs among a type's nib-bound properties: its type - or, for a
    /// <see cref="List{T}"/>, its element type, nullable-unwrapped either way - is a <see cref="Glyph"/>
    /// or an enum.
    /// </summary>
    static bool IsRelevantPropertyType(Type propertyType)
    {
        var elementType = Navigation.IsListType(propertyType) ? propertyType.GetGenericArguments()[0] : propertyType;
        var underlyingElementType = Nullable.GetUnderlyingType(elementType) ?? elementType;

        return underlyingElementType.IsAssignableTo(typeof(Glyph)) || underlyingElementType.IsEnum;
    }
}