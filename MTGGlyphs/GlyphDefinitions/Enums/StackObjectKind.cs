namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The noun of a stack object referred back to, after "that", "the" or "those": "spell", "ability", or "spell or ability" ("that spell or ability", "those spells").</summary>
public enum StackObjectKind
{
    [RegexPattern("spell or ability")]
    SpellOrAbility,

    Spell,

    Ability
}
