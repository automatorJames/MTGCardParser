namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The bare noun "card" or "cards", for cards of no stated kind: "cards in graveyards", "all cards exiled with this artifact", "a card exiled with this creature".</summary>
public enum CardsNoun
{
    [RegexPattern("cards?")]
    Card
}
