namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A bound before a number of cards in hand: "exactly" ("exactly seven cards in hand"), "fewer than"/"less than", "more than".</summary>
public enum HandCountBound
{
    Exactly,

    [RegexPattern("fewer than", "less than")]
    FewerThan,

    MoreThan
}
