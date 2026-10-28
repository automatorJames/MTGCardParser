namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The verb for putting a card into play, singular or plural: "play" or "plays" (lands, or any card) and "cast" or "casts" (spells), in "play that card", "cast it without paying its mana cost", "can't cast creature spells".</summary>
public enum PlayVerb
{
    [RegexPattern("play", "plays")]
    Play,

    [RegexPattern("cast", "casts")]
    Cast
}
