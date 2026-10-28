namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The verb joining a permanent to a buff or ability, singular or plural: "gets" ("gets +1/+1"), "has" or "have" ("has flying", "has all activated abilities of ..."), "gains" ("gains flying until end of turn"), "loses" ("loses first strike").</summary>
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
