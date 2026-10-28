namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Whether a player did what was just offered or required, picking players out after "who": "does", "doesn't", "can't" ("each player who doesn't", "each opponent who can't").</summary>
public enum PlayerResponse
{
    Does,

    [RegexPattern("doesn't")]
    Doesnt,

    [RegexPattern("can't")]
    Cant
}
