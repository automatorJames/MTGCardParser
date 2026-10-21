namespace MTGGlyphs.GlyphDefinitions;

public enum TargetingSource
{
    [RegexPattern("a spell or ability")]
    SpellOrAbility,

    [RegexPattern("a spell")]
    Spell
}
