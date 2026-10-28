namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The Role token names, "wicked", "cursed", "royal", "monster", "sorcerer", "young hero", "virtuous", between "create a" and "role token": "create a wicked role token attached to up to one target creature you control".</summary>
public enum RoleKind
{
    Wicked,

    Cursed,

    Royal,

    Monster,

    Sorcerer,

    [RegexPattern("young hero")]
    YoungHero,

    Virtuous
}
