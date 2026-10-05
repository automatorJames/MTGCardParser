using System.Collections;
using System.Globalization;

namespace Glyphotype.Tests.Infrastructure;

/// <summary>
/// Renders what a line was tokenized into as one deterministic, human-readable string - the unit every
/// corpus expectation is written in. Owned by the tests (rather than reusing something like
/// <c>DebugSerializer</c>) so that no display change elsewhere can break them.
/// <para>
/// The notation, token by token, space-separated:
/// <list type="bullet">
/// <item><c>AnimalRests{Animal=Dog, Place=Kitchen}</c> - a glyph and its property values, in nib order.
/// Null, false and empty-list values are omitted; a glyph with none left is just its name.</item>
/// <item><c>«the dog snores»</c> - unmatched text.</item>
/// <item><c>.</c> - a clause-separating period, a clause's own or one nested in an enclosure.</item>
/// <item><c>(</c>, <c>)</c>, <c>"</c> - the delimiters of an enclosure no glyph matched whole, around its tokenized inside.</item>
/// </list>
/// Values: enums by member name, lists as <c>[A, B]</c>, nested glyphs recursively. Primitives render
/// compactly: a <see cref="OneOf{T1,T2}"/> as the one alternative that matched, a <see cref="CompoundOf{T}"/>
/// as <c>[A, B]</c>, a <see cref="ManyOf{T}"/> as <c>[A, B, C | And]</c>, an <see cref="OptionalOf{T}"/> as
/// its item, and a <see cref="DynamicGlyph"/> as <c>Dynamic(ResolvedGlyph{...})</c>. A named subclass of a
/// primitive keeps its name in front: <c>TraitList[Big, Old]</c>, <c>DayHeading{Monday}</c>.
/// </para>
/// </summary>
public static class GlyphSignature
{
    static readonly Type[] _compactPrimitives = [typeof(OneOf<,>), typeof(OneOf<,,>), typeof(CompoundOf<>), typeof(ManyOf<>), typeof(OptionalOf<>)];

    public static string Of(ProcessedLine line) =>
        string.Join(" ", line.Glyphs.Select(Of));

    public static string Of(CaptureUnit unit) =>
        unit switch
        {
            ClauseBreak => ".",
            EnclosureMark mark => mark.CaptureValue,
            UnmatchedString unmatched => $"«{unmatched.CaptureValue}»",
            Glyph glyph => OfGlyph(glyph),
            _ => throw new ArgumentException($"Unexpected {nameof(CaptureUnit)} type {unit.GetType().Name}"),
        };

    /// <summary>Every glyph type in <paramref name="unit"/>'s hydrated tree, including itself.</summary>
    public static IEnumerable<Type> GlyphTypesIn(CaptureUnit unit) =>
        unit is Glyph glyph ? GlyphTypesIn(glyph) : [];

    static IEnumerable<Type> GlyphTypesIn(Glyph glyph) =>
        new[] { glyph.GetType() }.Concat(Children(glyph).SelectMany(x => Flatten(x.Value)).OfType<Glyph>().SelectMany(GlyphTypesIn));

    static IEnumerable<object> Flatten(object value) =>
        value is IEnumerable list and not string ? list.Cast<object>() : [value];

    static string OfGlyph(Glyph glyph)
    {
        var type = glyph.GetType();
        var name = IsCompactPrimitive(type) ? "" : type.Name;

        switch (glyph)
        {
            case DynamicGlyph dynamic:
                return $"Dynamic({OfValue(dynamic.Item)})";

            case OneOfBase:
                return $"{name}{{{OfValue(Children(glyph).Single().Value)}}}";

            case CompoundOfBase:
                return $"{name}[{string.Join(", ", Flatten(Children(glyph).Single().Value).Select(OfValue))}]";

            case not null when IsClosedGeneric(type, typeof(ManyOf<>)) || IsClosedGeneric(type.BaseType, typeof(ManyOf<>)):
                var items = string.Join(", ", Flatten(type.GetProperty(nameof(ManyOf<object>.Items)).GetValue(glyph)).Select(OfValue));
                var conjunction = type.GetProperty(nameof(ManyOf<object>.Conjunction)).GetValue(glyph);
                return $"{name}[{items}{(conjunction is null ? "" : $" | {conjunction}")}]";

            case not null when IsClosedGeneric(type, typeof(OptionalOf<>)):
                return OfValue(Children(glyph).Single().Value);
        }

        var properties = Children(glyph)
            .Where(x => !IsOmitted(x.Value))
            .Select(x => $"{x.Name}={OfValue(x.Value)}")
            .ToList();

        return properties.Count == 0 ? name : $"{name}{{{string.Join(", ", properties)}}}";
    }

    /// <summary>
    /// A glyph's meaningful property values, in reading order: the matched alternative for a one-of, the
    /// items for a compound, the item for an optional-of or dynamic, and otherwise every nib-bound
    /// property in <see cref="Glyph.Nibs"/> order.
    /// </summary>
    static IEnumerable<(string Name, object Value)> Children(Glyph glyph)
    {
        var type = glyph.GetType();

        if (glyph is DynamicGlyph dynamic)
            return [(nameof(DynamicGlyph.Item), dynamic.Item)];

        if (glyph is OneOfBase)
            return OneOfBase.GetAlternativeProps(type)
                .Select(x => (x.Name, x.GetValue(glyph)))
                .Where(x => x.Item2 is not null)
                .Take(1);

        if (glyph is CompoundOfBase)
            return [(nameof(CompoundOf<object>.Items), type.GetProperty(nameof(CompoundOf<object>.Items)).GetValue(glyph))];

        if (IsClosedGeneric(type, typeof(OptionalOf<>)))
            return [(nameof(OptionalOf<Glyph>.Item), type.GetProperty(nameof(OptionalOf<Glyph>.Item)).GetValue(glyph))];

        return GlyphTypeCache.GetConfiguration(type).Nibs
            .OfType<PropertyNib>()
            .Select(x => (x.Name, type.GetProperty(x.Name).GetValue(glyph)));
    }

    static string OfValue(object value) =>
        value switch
        {
            Glyph glyph => OfGlyph(glyph),
            Enum member => member.ToString(),
            IEnumerable list and not string => $"[{string.Join(", ", list.Cast<object>().Select(OfValue))}]",
            _ => Convert.ToString(value, CultureInfo.InvariantCulture),
        };

    static bool IsOmitted(object value) =>
        value is null or false || (value is IEnumerable list and not string && !list.Cast<object>().Any());

    static bool IsCompactPrimitive(Type type) =>
        type.IsGenericType && _compactPrimitives.Contains(type.GetGenericTypeDefinition());

    static bool IsClosedGeneric(Type type, Type openGeneric) =>
        type is { IsGenericType: true } && type.GetGenericTypeDefinition() == openGeneric;
}
