namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Counters named by whose or how many they are rather than by a number: "its +1/+1 counters", "that many charge counters", "those counters".</summary>
public enum CounterSet
{
    [RegexPattern("its")]
    Its,

    [RegexPattern("that many")]
    ThatMany,

    [RegexPattern("those")]
    Those
}
