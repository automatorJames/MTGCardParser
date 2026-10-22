namespace MTGGlyphs.GlyphDefinitions;

public enum NamedTypeReference
{
    [RegexPattern("of that type")]
    ThatType,

    [RegexPattern("of the chosen type")]
    ChosenType,

    [RegexPattern("of any of those types")]
    AnyOfThoseTypes
}
