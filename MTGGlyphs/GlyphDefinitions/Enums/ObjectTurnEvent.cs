namespace MTGGlyphs.GlyphDefinitions;

public enum ObjectTurnEvent
{
    [RegexPattern("died")]
    Died,

    [RegexPattern("entered the battlefield under your control")]
    EnteredUnderYourControl,

    [RegexPattern("were put into graveyards from anywhere", "were put into your graveyard from anywhere")]
    PutIntoGraveyard
}
