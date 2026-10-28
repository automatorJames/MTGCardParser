namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A designation a player gets and keeps for the rest of the game: "the city's blessing" (ascend), "an enduring story" (storied), in "you get the city's blessing for the rest of the game", "you have the city's blessing".</summary>
public enum PlayerDesignation
{
    [RegexPattern("the city's blessing")]
    CitysBlessing,

    [RegexPattern("an enduring story")]
    EnduringStory
}
