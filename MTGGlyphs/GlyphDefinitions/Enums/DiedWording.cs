namespace MTGGlyphs.GlyphDefinitions;

/// <summary>How dying is written: "that died", or the older "put into your graveyard from the battlefield", in "creature that died this turn", "creature put into your graveyard from the battlefield this turn".</summary>
public enum DiedWording
{
    [RegexPattern("that died")]
    Died,

    [RegexPattern("put into your graveyard from the battlefield")]
    PutIntoYourGraveyard
}
