namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What happened to the damage or life being counted: "prevented", "lost", "gained", "paid", "dealt", in "1 damage prevented this way", "1 life your opponents have lost this turn", "1 life they paid this way".</summary>
public enum DamageOrLifeChange
{
    Prevented,
    Lost,
    Gained,
    Paid,
    Dealt
}
