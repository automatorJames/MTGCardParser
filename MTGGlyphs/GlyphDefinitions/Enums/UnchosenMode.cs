namespace MTGGlyphs.GlyphDefinitions;

public enum UnchosenMode
{
    [RegexPattern("that hasn't been chosen this turn")]
    NotChosenThisTurn,

    [RegexPattern("that hasn't been chosen")]
    NeverChosen
}
