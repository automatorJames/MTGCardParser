namespace MTGGlyphs.GlyphDefinitions;

public enum ChosenAspect
{
    [RegexPattern("of the chosen color")]
    OfTheChosenColor,

    [RegexPattern("of the chosen type")]
    OfTheChosenType,

    [RegexPattern("of that color")]
    OfThatColor,

    [RegexPattern("of that (creature |land |card )?type")]
    OfThatType,

    [RegexPattern("of the color of your choice")]
    OfTheColorOfYourChoice,

    [RegexPattern("of the creature type of your choice")]
    OfTheCreatureTypeOfYourChoice
}
