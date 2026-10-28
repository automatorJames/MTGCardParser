namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The negation before a game-result verb: "can't" in "you can't lose the game this turn", "don't" or "doesn't" in "you don't lose the game for having 0 or less life".</summary>
public enum Negation
{
    [RegexPattern("can't")]
    Cant,

    [RegexPattern("don't", "doesn't")]
    Dont
}
