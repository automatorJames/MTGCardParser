namespace Glyphotype.Definitions;

/// <summary>One nib-bound property of a <see cref="GlyphDefinition"/>, with the property-level attributes the engine reads.</summary>
public sealed record PropertyDefinition
{
    public string Name { get; init; }

    public TypeReference Type { get; init; }

    /// <summary><see cref="OptionalAttribute"/>: the property may be absent from a match.</summary>
    public bool IsOptional { get; init; }

    /// <summary><see cref="AllowUnmatchedAttribute"/>: a <see cref="DynamicGlyph"/> property that keeps text nothing resolves as unmatched text, rather than failing the match.</summary>
    public bool AllowsUnmatched { get; init; }

    /// <summary><see cref="RegexPatternAttribute"/>: overrides the default pattern (a bool's or primitive's).</summary>
    public IReadOnlyList<string> Patterns { get; init; } = [];

    /// <summary><see cref="OptionalPluralAttribute"/>: an enum property matches each of its members singular or plural.</summary>
    public bool IsOptionalPlural { get; init; }

    /// <summary><see cref="JoinedByAttribute"/>: the separator between a <see cref="CompoundOf{T}"/>'s items at this usage site.</summary>
    public Joiner? JoinedBy { get; init; }

    /// <summary><see cref="TypeFilterAttribute"/>: the marker (see <see cref="GrammarDefinition.Markers"/>) a <see cref="DynamicGlyph"/> may resolve to.</summary>
    public string TypeFilter { get; init; }

    /// <summary><see cref="IntroducesAttribute"/>: the captured value is a referent, with these features. Null when it isn't one.</summary>
    public AgreementDefinition Introduces { get; init; }

    /// <summary><see cref="RefersToAttribute"/>: the sibling property a back-reference property refers to outright.</summary>
    public string RefersTo { get; init; }
}
