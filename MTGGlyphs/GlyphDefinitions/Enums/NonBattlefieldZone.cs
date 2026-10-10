namespace MTGGlyphs.GlyphDefinitions;

public enum NonBattlefieldZone
{
    [RegexPattern("exile(d)?")]
    Exile,

    Graveyard,
    Hand,
    Library,

    [RegexPattern("you own outside the game")]
    Sideboard,

    [RegexPattern("spell")]
    Stack
}
