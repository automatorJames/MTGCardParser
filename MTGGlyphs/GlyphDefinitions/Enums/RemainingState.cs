namespace MTGGlyphs.GlyphDefinitions;

public enum RemainingState
{
    Tapped,

    [RegexPattern("on the battlefield")]
    OnTheBattlefield,

    Exiled
}
