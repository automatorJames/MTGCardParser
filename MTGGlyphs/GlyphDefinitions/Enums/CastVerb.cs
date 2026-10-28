namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The verb of a spell-cast trigger, with the magecraft widening: "whenever you cast a spell", "whenever a player casts a red spell", "whenever you cast or copy an instant or sorcery spell".</summary>
public enum CastVerb
{
    [RegexPattern("cast or copy", "casts or copies")]
    CastOrCopy,

    [RegexPattern("casts?")]
    Cast
}
