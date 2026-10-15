namespace MTGGlyphs.GlyphDefinitions;

public enum CounterSize
{
    [RegexPattern("-1/-1")]
    MinusOne,

    [RegexPattern(@"\+1/\+1")]
    PlusOne
}
