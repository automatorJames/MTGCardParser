namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The bare noun "card", where the object's kind isn't a permanent type: "card revealed this way", "the card you exiled" (beside "creature tapped this way", "the creature that died").</summary>
public enum CardNoun
{
    [RegexPattern("card")]
    Card
}
