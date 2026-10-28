namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The verb of drawing or discarding cards, singular or plural: "draw two cards", "discards their hand", "discard a card" (as a cost), "discarding".</summary>
[OptionalPlural]
public enum CardVerb
{
    Draw,

    [RegexPattern("discard", "discarding")]
    Discard
}
