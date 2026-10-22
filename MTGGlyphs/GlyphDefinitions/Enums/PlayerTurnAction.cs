namespace MTGGlyphs.GlyphDefinitions;

public enum PlayerTurnAction
{
    [RegexPattern("drew", "drawn", "has drawn")]
    Drew,

    [RegexPattern("cast", "has cast")]
    Cast,

    [RegexPattern("sacrificed")]
    Sacrificed
}
