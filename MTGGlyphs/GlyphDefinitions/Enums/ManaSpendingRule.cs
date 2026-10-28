namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Whether a mana-spending restriction allows the named uses only or forbids them: "spend this mana only" (to cast a creature spell), "this mana can't be spent" (to cast a nonartifact spell).</summary>
public enum ManaSpendingRule
{
    [RegexPattern("spend this mana only")]
    OnlyFor,

    [RegexPattern("this mana can't be spent")]
    NotFor
}
