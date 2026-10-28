namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The opening of a predefined token's reminder text, singular or plural: "it's an artifact with "…"" after one Treasure, Food or Clue, or "they're artifacts with "…"" after several.</summary>
public enum TokenPronounIsArtifact
{
    [RegexPattern("it's an artifact with \"")]
    It,

    [RegexPattern("they're artifacts with \"")]
    They
}
