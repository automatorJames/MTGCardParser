namespace MTGGlyphs.GlyphDefinitions;

public enum ReplacedOwnCost
{
    [RegexPattern("this spell's mana cost")]
    ThisSpellsManaCost,

    [RegexPattern("its mana cost")]
    ItsManaCost
}
