namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The verb relating a player to the spells meant, "cast" ("spells you cast", "spells your opponents cast") or "control" ("spells you control").</summary>
public enum SpellRelation
{
    [RegexPattern("casts?")]
    Cast,

    [RegexPattern("controls?")]
    Control
}
