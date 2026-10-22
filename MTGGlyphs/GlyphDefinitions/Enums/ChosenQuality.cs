namespace MTGGlyphs.GlyphDefinitions;

public enum ChosenQuality
{
    [RegexPattern("the chosen color")]
    ChosenColor,

    [RegexPattern("the color of your choice")]
    ColorOfYourChoice,

    [RegexPattern("the color of its controller's choice")]
    ColorOfControllersChoice,

    [RegexPattern("that color")]
    ThatColor,

    [RegexPattern("each of its colors")]
    EachOfItsColors,

    [RegexPattern("each color")]
    EachColor,

    [RegexPattern("everything")]
    Everything,

    [RegexPattern("the chosen (card )?name")]
    ChosenName
}
