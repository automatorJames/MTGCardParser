using Glyphotype.GlyphPrimitives.Internal;

namespace Glyphotype.NibHelpers;

public record PropertyNib : Nib
{
    public PropertyInfo Prop { get; }
    public Type Type { get; }
    public string Name { get; }

    /// <summary>A better name than <see cref="Name"/> for "XOf" wrapper properties (FirstItem, Item, ...) - based on the wrapped type T instead. Null outside that hierarchy.</summary>
    public string DescriptiveName { get; }

    /// <summary>This property's own <see cref="Navigation"/>, cached so every graph position sharing this <see cref="PropertyNib"/> reuses the same instance.</summary>
    public Navigation Navigation { get; }

    public PropertyNib(string text, PropertyInfo prop)
        : base(text)
    {
        Prop = prop;
        Type = prop.PropertyType;
        Name = prop.Name;
        DescriptiveName = ComputeDescriptiveName();

        Navigation = new Navigation(this, DescriptiveName);
    }

    string ComputeDescriptiveName()
    {
        var declaringType = Prop.DeclaringType;
        var safeTypeName = Navigation.GetRegexSafeTypeName(Nullable.GetUnderlyingType(Type) ?? Type);

        // A generic OneOf's alternatives are only named Alternative1..N, so its type arguments are the meaningful
        // labels (unambiguous: a generic OneOf may not repeat a type - see Glyph.GetOneOfTypeArgumentError). A
        // GlyphOneOf names its alternatives itself, so it keeps those names.
        if (IsClosedGeneric(declaringType, typeof(OneOf<,>)) || IsClosedGeneric(declaringType, typeof(OneOf<,,>)))
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
            .Select(x => new PropertyNib(x.Name, x))
            .ToArray();

    /// <summary>
    /// Whether a property belongs among a type's nib-bound properties: its nullable-unwrapped type is a
    /// <see cref="Glyph"/>, an enum, a bool, or a supported primitive (see <see cref="PrimitiveTerminal"/>) -
    /// or it's a <see cref="List{T}"/> of Glyphs.
    /// </summary>
    static bool IsRelevantPropertyType(Type propertyType)
    {
        // A List<> only ever belongs to an internal primitive (see Glyph.GetListPropertyError), and those only
        // ever hold lists of Glyphs - their repeated items (e.g. CompoundOf<T>.SecondPlus).
        if (Navigation.IsListType(propertyType))
            return propertyType.GetGenericArguments()[0].IsAssignableTo(typeof(Glyph));

        var underlyingElementType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

        return
            underlyingElementType.IsAssignableTo(typeof(Glyph)) // Glyphs are the building blocks of non-terminal regex graphs
            || underlyingElementType.IsEnum // Enums are the primary terminals of regex graphs
            || underlyingElementType == typeof(bool) // Bools are also allowable terminals in regex graphs
            || PrimitiveTerminal.IsSupported(underlyingElementType); // As are parsed primitives, e.g. int
    }
}