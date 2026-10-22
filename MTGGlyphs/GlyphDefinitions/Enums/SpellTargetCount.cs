namespace MTGGlyphs.GlyphDefinitions;

public enum SpellTargetCount
{
    [RegexPattern("only a single")]
    OnlyASingle,

    Only,

    [RegexPattern("one or more")]
    OneOrMore
}
