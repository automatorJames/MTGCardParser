namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A modal head's restriction that no mode is chosen twice: "choose one that hasn't been chosen —" or, over a turn, "choose one that hasn't been chosen this turn —".</summary>
public enum UnchosenMode
{
    [RegexPattern("that hasn't been chosen this turn")]
    NotChosenThisTurn,

    [RegexPattern("that hasn't been chosen")]
    NeverChosen
}
