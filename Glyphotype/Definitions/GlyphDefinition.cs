namespace Glyphotype.Definitions;

/// <summary>What a <see cref="GlyphDefinition"/> derives from, which decides how the engine handles it.</summary>
public enum GlyphKind
{
    /// <summary>Derives from <see cref="GlyphPrimitives.Glyph"/>: its nibs are matched in sequence.</summary>
    Glyph,

    /// <summary>Derives from <see cref="GlyphPrimitives.GlyphOneOf"/>: its properties are named alternatives, exactly one of which matches.</summary>
    GlyphOneOf,

    /// <summary>A pure alias of a generic primitive (see <see cref="GlyphDefinition.AliasOf"/>): a name and class-level attributes, nothing more.</summary>
    Alias,
}

/// <summary>How much of a line a top-level glyph's match must cover - the three span rules.</summary>
public enum SpanRule
{
    /// <summary>No attribute: governed by <see cref="GlobalSettings.AllowPartialSegmentMatches"/>.</summary>
    Default,

    /// <summary><see cref="MustMatchWholeLineAttribute"/>.</summary>
    WholeLine,

    /// <summary><see cref="AllowPartialSegmentMatchAttribute"/>.</summary>
    PartialSegment,
}

/// <summary>
/// A Glyph type, independent of any CLR <see cref="Type"/>: everything the engine reads from a Glyph class - its
/// base, nibs, joiner, nib-bound properties and class-level attributes - as plain, serializable data that refers
/// to other definitions by name. Read from existing types with <see cref="GrammarDefinition.FromTypes"/>; turned
/// back into types with <see cref="GrammarEmitter"/> or into C# source with <see cref="GlyphSourceWriter"/>.
/// <para>
/// Mirrors what's declared, not what the engine derives from it: an absent <see cref="Nibs"/> or
/// <see cref="Joiner"/> means "not overridden", leaving the engine to apply its usual defaults. Holds nothing
/// about any particular corpus (e.g. what a <see cref="DynamicGlyph"/> property resolved to), and nothing that
/// isn't grammar - computed get-only properties, for one, aren't part of a definition.
/// </para>
/// </summary>
public sealed record GlyphDefinition
{
    public string Name { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public GlyphKind Kind { get; init; }

    /// <summary>The generic primitive an <see cref="GlyphKind.Alias"/> aliases (e.g. <c>CompoundOf&lt;Trait&gt;</c>). Null for other kinds.</summary>
    public TypeReference AliasOf { get; init; }

    /// <summary>The <see cref="Glyph.Nibs"/> override. Empty when not overridden: the engine then derives nibs from the properties in order, else <see cref="Patterns"/>, else the friendly-cased name.</summary>
    public IReadOnlyList<NibDefinition> Nibs { get; init; } = [];

    /// <summary>The <see cref="Glyph.Joiner"/> override, or null for the kind's default.</summary>
    public Joiner? Joiner { get; init; }

    /// <summary>The nib-bound properties, in declaration order - which is the nib order when <see cref="Nibs"/> is empty, and the alternative order of a <see cref="GlyphKind.GlyphOneOf"/>.</summary>
    public IReadOnlyList<PropertyDefinition> Properties { get; init; } = [];

    /// <summary>The markers (see <see cref="GrammarDefinition.Markers"/>) this glyph carries, which a <see cref="PropertyDefinition.TypeFilter"/> can select.</summary>
    public IReadOnlyList<string> Markers { get; init; } = [];

    /// <summary><see cref="DependentAttribute"/>: only ever matched nested in another glyph (or through a dynamic), never top-level.</summary>
    public bool IsDependent { get; init; }

    public SpanRule SpanRule { get; init; }

    /// <summary><see cref="TokenizationOrderAttribute"/>.</summary>
    public int? TokenizationOrder { get; init; }

    /// <summary>Class-level <see cref="RegexPatternAttribute"/>: the glyph's patterns when it has neither nibs nor properties.</summary>
    public IReadOnlyList<string> Patterns { get; init; } = [];

    /// <summary>Class-level <see cref="JoinedByAttribute"/>, for a <see cref="CompoundOf{T}"/> alias.</summary>
    public Joiner? JoinedBy { get; init; }

    /// <summary>The names of the glyphs this one refers to directly - through its properties or the primitive it aliases.</summary>
    public IEnumerable<string> GetReferencedGlyphNames() =>
        Properties.Select(x => x.Type)
            .Append(AliasOf)
            .Where(x => x is not null)
            .SelectMany(x => x.SelfAndDescendants())
            .Where(x => x.Kind == TypeReferenceKind.Glyph)
            .Select(x => x.Name)
            .Distinct();
}
