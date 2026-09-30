namespace Glyphotype.Definitions;

/// <summary>A closed set of terminal words - the portable counterpart of an enum used as a glyph property type.</summary>
public sealed record VocabularyDefinition
{
    public string Name { get; init; }

    /// <summary><see cref="OptionalPluralAttribute"/>: every member also matches with a plural suffix.</summary>
    public bool IsOptionalPlural { get; init; }

    public IReadOnlyList<VocabularyMemberDefinition> Members { get; init; } = [];
}

/// <summary>One member of a <see cref="VocabularyDefinition"/>.</summary>
public sealed record VocabularyMemberDefinition
{
    public string Name { get; init; }

    /// <summary>An explicit enum value, or null where C# would assign it anyway (the previous member's value plus one, starting at zero).</summary>
    public long? Value { get; init; }

    /// <summary><see cref="RegexPatternAttribute"/>: the synonym patterns this member matches. Empty means its friendly-cased name.</summary>
    public IReadOnlyList<string> Patterns { get; init; } = [];

    /// <summary><see cref="ColorAttribute"/>: a display color, as a hex string.</summary>
    public string Color { get; init; }
}
