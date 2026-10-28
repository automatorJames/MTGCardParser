namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What a number of objects did this turn, in conditions such as "three or more creatures died this turn": "died", "entered the battlefield under your control" (celebration), "were put into graveyards from anywhere".</summary>
public enum ObjectTurnEvent
{
    [RegexPattern("died")]
    Died,

    [RegexPattern("entered the battlefield under your control")]
    EnteredUnderYourControl,

    [RegexPattern("were put into graveyards from anywhere", "were put into your graveyard from anywhere")]
    PutIntoGraveyard
}
