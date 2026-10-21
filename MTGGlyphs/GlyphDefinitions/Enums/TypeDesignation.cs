namespace MTGGlyphs.GlyphDefinitions;

public enum TypeDesignation
{
    [RegexPattern("the chosen (creature )?type")]
    ChosenType,

    [RegexPattern("the chosen color")]
    ChosenColor,

    [RegexPattern("every basic land type")]
    EveryBasicLandType
}
