namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A land type chosen or named earlier, as an "of" phrase: "of that type", "of the chosen type", "of any of those types" ("landwalk of the chosen type", "a land of that type").</summary>
public enum NamedTypeReference
{
    [RegexPattern("of that type")]
    ThatType,

    [RegexPattern("of the chosen type")]
    ChosenType,

    [RegexPattern("of any of those types")]
    AnyOfThoseTypes
}
