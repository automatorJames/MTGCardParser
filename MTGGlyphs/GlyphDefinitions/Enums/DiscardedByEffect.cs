namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What an opponent's effect made you discard: "this card" (madness-style punishers), "a card", "cards" - "a spell or ability an opponent controls causes you to discard this card".</summary>
public enum DiscardedByEffect
{
    [RegexPattern("this card")]
    ThisCard,

    [RegexPattern("a card")]
    ACard,

    [RegexPattern("cards")]
    Cards
}
