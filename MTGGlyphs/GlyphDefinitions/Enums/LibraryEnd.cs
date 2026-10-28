namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Which end of a library cards go to, after "on": "the bottom" ("on the bottom of your library"), "top" ("on top of their library"), or the player's pick "their choice of the top or bottom".</summary>
public enum LibraryEnd
{
    [RegexPattern("the bottom")]
    Bottom,

    Top,

    [RegexPattern("(their|your) choice of the top or bottom")]
    TopOrBottom
}
