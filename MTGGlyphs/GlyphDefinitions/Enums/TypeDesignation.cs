namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A type or color referred to by an earlier choice rather than by name: "the chosen type", "that creature type" or "that land type", "the chosen color", or "every basic land type", as in "is the chosen type in addition to its other types" and "becomes that type until end of turn".</summary>
public enum TypeDesignation
{
    [RegexPattern("the chosen (creature )?type", "that (creature |land )?type")]
    ChosenType,

    [RegexPattern("the chosen color")]
    ChosenColor,

    [RegexPattern("every basic land type")]
    EveryBasicLandType
}
