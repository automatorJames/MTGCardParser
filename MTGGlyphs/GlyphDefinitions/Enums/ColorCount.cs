namespace MTGGlyphs.GlyphDefinitions;

/// <summary>How many colors a permanent or spell has, as a narrowing after its kind: "that's one or more colors", "that's colorless", "that's monocolored", "that are multicolored".</summary>
public enum ColorCount
{
    [RegexPattern("one or more colors")]
    OneOrMoreColors,

    Colorless,

    Monocolored,

    Multicolored
}
