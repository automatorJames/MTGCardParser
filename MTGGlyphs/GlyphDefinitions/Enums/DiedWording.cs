namespace MTGGlyphs.GlyphDefinitions;

public enum DiedWording
{
    [RegexPattern("that died")]
    Died,

    [RegexPattern("put into your graveyard from the battlefield")]
    PutIntoYourGraveyard
}
