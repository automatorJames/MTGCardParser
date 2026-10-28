namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The span of an extra activation, in "twice … rather than only once": "twice each turn", "twice this turn".</summary>
public enum ActivationPeriod
{
    [RegexPattern("each turn")]
    EachTurn,

    [RegexPattern("this turn")]
    ThisTurn
}
