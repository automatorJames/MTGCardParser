namespace MTGGlyphs.GlyphDefinitions;

public enum PermanentVerb
{
    [RegexPattern("get(s)?")]
    Get,

    [RegexPattern("have", "has")]
    Have,

    [RegexPattern("gain(s)?")]
    Gain,

    [RegexPattern("lose(s)?")]
    Lose,
}
