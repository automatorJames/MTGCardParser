namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What a player did a countable number of times, in the past: "drew" ("drawn", "has drawn"), "cast" ("has cast"), "sacrificed" ("you drew two or more cards", "a player cast two or more spells").</summary>
public enum PlayerTurnAction
{
    [RegexPattern("drew", "drawn", "has drawn")]
    Drew,

    [RegexPattern("cast", "has cast")]
    Cast,

    [RegexPattern("sacrificed")]
    Sacrificed
}
