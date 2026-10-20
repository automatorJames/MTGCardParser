namespace MTGGlyphs.GlyphDefinitions;

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
