namespace MTGGlyphs.GlyphDefinitions;

public enum SkippedEvent
{
    [RegexPattern("that draw")]
    ThatDraw,

    [RegexPattern("that turn")]
    ThatTurn
}
