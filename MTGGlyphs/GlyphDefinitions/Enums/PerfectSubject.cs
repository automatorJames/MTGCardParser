namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A player subject contracted with the perfect auxiliary, before a past participle: "you've" ("you've gained 3 or more life", "spells you've cast this turn"), "they've" ("noncreature spells they've cast this turn").</summary>
public enum PerfectSubject
{
    [RegexPattern("you've")]
    You,

    [RegexPattern("they've")]
    They
}
