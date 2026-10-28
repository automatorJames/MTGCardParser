namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A card name chosen or named earlier, after "with": "that name", "the chosen name" ("cards with that name").</summary>
public enum NamedReference
{
    [RegexPattern("that name")]
    ThatName,

    [RegexPattern("the chosen name")]
    ChosenName
}
