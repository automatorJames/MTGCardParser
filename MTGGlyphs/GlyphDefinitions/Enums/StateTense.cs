namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The tense of a state condition, "is" or "was": "this creature's power is 4 or greater", "its mana value was 3 or less", "the discarded card was".</summary>
public enum StateTense
{
    [RegexPattern("is")]
    Present,

    [RegexPattern("was")]
    Past
}
