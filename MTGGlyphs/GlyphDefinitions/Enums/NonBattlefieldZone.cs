namespace MTGGlyphs.GlyphDefinitions;

public enum NonBattlefieldZone
{
    [RegexPattern("exile(d)?")]
    Exile,

    [RegexPattern("graveyards?")]
    Graveyard,

    [RegexPattern("hands?")]
    Hand,

    [RegexPattern("library", "libraries")]
    Library,

    [RegexPattern("you own outside the game")]
    Sideboard,

    [RegexPattern("spell")]
    Stack
}
