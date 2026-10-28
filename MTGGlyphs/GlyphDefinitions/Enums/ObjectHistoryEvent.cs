namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Something that happened to an object a countable number of times, after "the number of times this creature": "was kicked" (multikicker), "has mutated" (mutate).</summary>
public enum ObjectHistoryEvent
{
    [RegexPattern("was kicked")]
    WasKicked,

    [RegexPattern("has mutated")]
    HasMutated
}
