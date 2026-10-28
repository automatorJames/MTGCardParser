namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Which of a creature's two stats are meant: "power", "toughness", "power and toughness", "power or toughness" ("base power and toughness 0/2", "has greater power or toughness than this creature").</summary>
public enum PowerAndOrToughness
{
    Power,
    Toughness,
    PowerAndToughness,
    PowerOrToughness
}
