namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A spell's own cost that an alternative cost is paid in place of, after "rather than pay": "this spell's mana cost" ("you may pay {1}{w} rather than pay this spell's mana cost") or "its mana cost".</summary>
public enum ReplacedOwnCost
{
    [RegexPattern("this spell's mana cost")]
    ThisSpellsManaCost,

    [RegexPattern("its mana cost")]
    ItsManaCost
}
