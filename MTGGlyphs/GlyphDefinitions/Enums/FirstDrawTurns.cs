namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Which turns' first draw is revealed: "each turn" or "on each of your turns", in "reveal the first card you draw each turn".</summary>
public enum FirstDrawTurns
{
    [RegexPattern("each turn")]
    EachTurn,

    [RegexPattern("on each of your turns")]
    EachOfYourTurns
}
