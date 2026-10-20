namespace MTGGlyphs.GlyphDefinitions;

public enum ManaSpendingRule
{
    [RegexPattern("spend this mana only")]
    OnlyFor,

    [RegexPattern("this mana can't be spent")]
    NotFor
}
