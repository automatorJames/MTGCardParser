namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What protection is from when it's a quality chosen or referred to rather than listed: "the chosen color", "the color of your choice", "the color of its controller's choice", "that color", "each of its colors", "each color", "everything", "the chosen name".</summary>
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
