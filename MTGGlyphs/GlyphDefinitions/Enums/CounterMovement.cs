namespace MTGGlyphs.GlyphDefinitions;

public enum CounterMovement
{
    [RegexPattern("is put on", "are put on")]
    PutOn,

    [RegexPattern("is removed from", "are removed from")]
    RemovedFrom
}
