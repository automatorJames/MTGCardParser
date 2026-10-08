namespace Glyphotype.BackReferences;

/// <summary>
/// Something a <see cref="BackReference"/> can refer back to: a captured value marked <see cref="ReferentAttribute"/>
/// (or a <c>{this}</c>), where it was captured, and what a back-reference has to agree with to refer to it.
/// </summary>
/// <param name="Value">The captured value - a Glyph, or an enum captured by a referent property.</param>
/// <param name="Trace">Where <paramref name="Value"/> was captured.</param>
/// <param name="Number">Its number, from <see cref="SingularAttribute"/> or <see cref="PluralAttribute"/>.</param>
/// <param name="Kind">The kind of thing it is (see <see cref="KindOf"/>): what a <see cref="BackReference{T}"/>'s <c>T</c> must match.</param>
public sealed record ReferentCapture(object Value, CaptureTrace Trace, GrammaticalNumber Number, Type Kind)
{
    /// <summary>The source text the referent was captured from.</summary>
    public string Text { get; init; } = Trace.CaptureValue;

    /// <summary>Whether this is the document's reference to itself (see <see cref="This"/>).</summary>
    public bool IsSelf => Value is This;

    /// <summary>
    /// The referent <paramref name="value"/>, captured at <paramref name="trace"/>, with <paramref name="number"/>. A
    /// value that's itself a resolved back-reference stands for whatever it refers to, so a later pronoun joins the same
    /// chain - "destroy it. it can't be regenerated" - rather than stopping at the first pronoun. A <see cref="This"/>
    /// is always the document itself.
    /// </summary>
    public static ReferentCapture Of(object value, CaptureTrace trace, GrammaticalNumber number) =>
        value switch
        {
            BackReference { Antecedent: { } antecedent } => antecedent,
            This self => Self(self, trace),
            _ => new(value, trace, number, KindOf(value)),
        };

    /// <summary>The document itself, as referred to by <paramref name="self"/>, captured at <paramref name="trace"/>: always singular.</summary>
    public static ReferentCapture Self(This self, CaptureTrace trace) =>
        new(self, trace, GrammaticalNumber.Singular, typeof(This));

    /// <summary>
    /// The kind of thing <paramref name="value"/> is: its type, looking through the primitives that only wrap it - a
    /// one-of's kind is its matched alternative's, an optional's its item's, a list's its first item's.
    /// </summary>
    public static Type KindOf(object value) =>
        value switch
        {
            null => null,
            OneOfBase oneOf => KindOf(oneOf.GetResolvedValue()),
            _ when GetWrappedItems(value) is { } items => KindOf(items.FirstOrDefault()),
            _ => value.GetType(),
        };

    /// <summary>
    /// Every kind a value of <paramref name="type"/> can be (see <see cref="KindOf"/>) - empty when it can't be a referent
    /// at all (a bool, a number, text, or a <see cref="DynamicGlyph"/>, which could be anything).
    /// </summary>
    public static IEnumerable<Type> PossibleKinds(Type type)
    {
        type = Nullable.GetUnderlyingType(type) ?? type;

        if (type.IsEnum)
            return [type];

        if (!type.IsAssignableTo(typeof(Glyph)) || type.IsAssignableTo(typeof(DynamicGlyph)))
            return [];

        if (type.IsAssignableTo(typeof(OneOfBase)))
            return OneOfBase.GetAlternativeProps(type).SelectMany(x => PossibleKinds(x.PropertyType)).Distinct();

        if (GetWrappedItemType(type) is { } itemType)
            return PossibleKinds(itemType);

        return [type];
    }

    /// <summary>The items of an <see cref="OptionalOf{T}"/>, <see cref="ManyOf{T}"/> or <see cref="CompoundOf{T}"/> - null for anything else.</summary>
    static IEnumerable<object> GetWrappedItems(object value) =>
        GetWrappedItemType(value.GetType()) is null ? null
        : value.GetType().GetProperty(nameof(OptionalOf<Glyph>.Item)) is { } item ? [item.GetValue(value)]
        : ((System.Collections.IEnumerable)value.GetType().GetProperty(nameof(ManyOf<Glyph>.Items)).GetValue(value)).Cast<object>();

    /// <summary>The <c>T</c> of an <see cref="OptionalOf{T}"/>, <see cref="ManyOf{T}"/> or <see cref="CompoundOf{T}"/> <paramref name="type"/> is or derives from - null for anything else.</summary>
    static Type GetWrappedItemType(Type type)
    {
        for (var current = type; current is not null && current != typeof(Glyph); current = current.BaseType)
            if (current.IsGenericType && current.GetGenericTypeDefinition() is var definition
                && (definition == typeof(OptionalOf<>) || definition == typeof(ManyOf<>) || definition == typeof(CompoundOf<>)))
                return current.GetGenericArguments()[0];

        return null;
    }

    public override string ToString() => Text;
}
