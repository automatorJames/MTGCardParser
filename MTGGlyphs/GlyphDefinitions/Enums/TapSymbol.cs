namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The tap and untap symbols, "{t}" and "{q}", as a cost ("{t}: add {g}") or as reminder text explains one ("{q} is the untap symbol").</summary>
public enum TapSymbol
{
    [RegexPattern(@"\{t\}")]
    Tap,

    [RegexPattern(@"\{q\}")]
    Untap,
}
