namespace MTGGlyphs.GlyphDefinitions;

public enum DiscardedByEffect
{
    [RegexPattern("this card")]
    ThisCard,

    [RegexPattern("a card")]
    ACard,

    [RegexPattern("cards")]
    Cards
}
