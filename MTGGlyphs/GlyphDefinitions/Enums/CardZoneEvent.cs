namespace MTGGlyphs.GlyphDefinitions;

public enum CardZoneEvent
{
    [RegexPattern("leaves your graveyard", "leave your graveyard")]
    LeaveYourGraveyard,

    [RegexPattern("(is|are) put into your graveyard from anywhere")]
    PutIntoYourGraveyardFromAnywhere,

    [RegexPattern("(is|are) put into your graveyard from your library")]
    PutIntoYourGraveyardFromYourLibrary,

    [RegexPattern("(is|are) put into exile from your graveyard")]
    ExiledFromYourGraveyard
}
