namespace MTGGlyphs.GlyphDefinitions;

public enum TapAction
{
    [RegexPattern("tap or untap", "taps or untaps")]
    TapOrUntap,

    [RegexPattern("tap", "taps")]
    Tap,

    [RegexPattern("untap", "untaps")]
    Untap
}
