namespace MTGGlyphs.GlyphDefinitions;

public enum CardZoneEvent
{
    [RegexPattern("leaves your graveyard", "leave your graveyard")]
    LeaveYourGraveyard,

    [RegexPattern("leaves an opponent's graveyard", "leave an opponent's graveyard")]
    LeaveAnOpponentsGraveyard,

    [RegexPattern("(is|are) put into your graveyard from anywhere")]
    PutIntoYourGraveyardFromAnywhere,

    [RegexPattern("(is|are) put into an opponent's graveyard from anywhere")]
    PutIntoAnOpponentsGraveyardFromAnywhere,

    [RegexPattern("(is|are) put into a graveyard from anywhere")]
    PutIntoAGraveyardFromAnywhere,

    [RegexPattern("(is|are) put into your graveyard from your library")]
    PutIntoYourGraveyardFromYourLibrary,

    [RegexPattern("(is|are) put into your hand from your graveyard")]
    PutIntoYourHandFromYourGraveyard,

    [RegexPattern("(is|are) put into exile from your graveyard")]
    ExiledFromYourGraveyard
}
