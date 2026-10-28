namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A qualifier tying an object to a color or type chosen earlier in the ability, or chosen as the effect applies: "creature spells of the chosen type", "a card of the chosen color", "permanents of that color", "a source of your choice of the chosen color", "of the creature type of your choice".</summary>
public enum ChosenAspect
{
    [RegexPattern("of the chosen color")]
    OfTheChosenColor,

    [RegexPattern("of the chosen type")]
    OfTheChosenType,

    [RegexPattern("of the chosen kind")]
    OfTheChosenKind,

    [RegexPattern("of that color")]
    OfThatColor,

    [RegexPattern("of that (creature |land |card )?type")]
    OfThatType,

    [RegexPattern("of the color of your choice")]
    OfTheColorOfYourChoice,

    [RegexPattern("of the creature type of your choice")]
    OfTheCreatureTypeOfYourChoice
}
