namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A zone other than the battlefield, singular or plural: "exile", "graveyard", "hand", "library" ("libraries"), the sideboard as "you own outside the game", and the stack named by "spell" - where cards are, are cast from, or are put into.</summary>
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
