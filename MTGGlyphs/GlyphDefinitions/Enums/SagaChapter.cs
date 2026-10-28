namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A Saga's chapter number as a roman numeral, "i" through "vi", as the reminder text names the last one: "sacrifice after iii".</summary>
public enum SagaChapter
{
    [RegexPattern("i")]
    One,

    [RegexPattern("ii")]
    Two,

    [RegexPattern("iii")]
    Three,

    [RegexPattern("iv")]
    Four,

    [RegexPattern("v")]
    Five,

    [RegexPattern("vi")]
    Six
}
