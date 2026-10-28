namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A Saga's chapter number as a lowercase roman numeral before the dash of its chapter ability: "i — scry 2", "iii, iv — add {r}", up to "vii".</summary>
public enum ChapterNumber
{
    [RegexPattern("i")]
    I,

    [RegexPattern("ii")]
    II,

    [RegexPattern("iii")]
    III,

    [RegexPattern("iv")]
    IV,

    [RegexPattern("v")]
    V,

    [RegexPattern("vi")]
    VI,

    [RegexPattern("vii")]
    VII
}
