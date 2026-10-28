namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An amount measured by the effect just described, between "the" and "this way": "life lost", "life gained", "damage prevented", or "damage dealt" ("the damage dealt this way").</summary>
public enum ThisWayAmount
{
    [RegexPattern("life lost")]
    LifeLost,

    [RegexPattern("life gained")]
    LifeGained,

    [RegexPattern("damage prevented")]
    DamagePrevented,

    [RegexPattern("damage dealt")]
    DamageDealt
}
