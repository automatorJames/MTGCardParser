namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The verb of a tap effect: "tap" ("tap target wall"), "untap" ("untap all creatures you control"), or "tap or untap" ("tap or untap target permanent"), with the third-person "taps", "untaps", "taps or untaps".</summary>
public enum TapAction
{
    [RegexPattern("tap or untap", "taps or untaps")]
    TapOrUntap,

    [RegexPattern("tap", "taps")]
    Tap,

    [RegexPattern("untap", "untaps")]
    Untap
}
